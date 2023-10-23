using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable
{
    Vector3 dir;
    Animator animator;
    CharacterController controller;
    [SerializeField] float speed;

    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float velocity = 0f;

    private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 100f;

    private float xRotation = 0f;

    private bool isCursorLocked = true;
    private Quaternion networkRotation = Quaternion.identity;

    [SerializeField] private GameObject chatPanel;
    private void Start()
    {

        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            playerCamera = GameObject.Find("CameraRig").transform;
            animator.SetFloat("MoveSpeed", currentSpeed);
            chatPanel = GameObject.FindGameObjectWithTag("ChatPanel");
            chatPanel.SetActive(false);

            PhotonNetwork.SendRate = 10;  // Default is 20

            // Set the serialization rate (number of times per second that the objects' state is serialized)
            PhotonNetwork.SerializationRate = 5;  // Default is 10
        }

    }

    private void Update()
    {
        if (!photonView.IsMine || chatPanel.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked; // Toggle the flag

            if (isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        PlayerRotate();
        Move();
    }


    private void Move()
    {
        if (controller.isGrounded)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 moveDir = new Vector3(h, 0, v);
            moveDir = transform.TransformDirection(moveDir); // Transform to local coordinates
            dir = moveDir * speed;

            if (dir != Vector3.zero)
            {
                currentSpeed = Mathf.SmoothDamp(currentSpeed, 50f, ref velocity, smoothTime);
            }
            else
            {
                currentSpeed = Mathf.SmoothDamp(currentSpeed, 0f, ref velocity, smoothTime / 4);
            }

            animator.SetFloat("MoveSpeed", currentSpeed);
            //if (Input.GetKeyDown(KeyCode.Space))
            //    dir.y += 7.5f;
        }

        dir.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(dir * Time.deltaTime);
        playerCamera.transform.position = transform.position;
    }

    private void PlayerRotate()
    {
        if (!photonView.IsMine)
        {
            transform.rotation = networkRotation;
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
        playerCamera.transform.rotation = transform.rotation;

        photonView.RPC("SyncRotation", RpcTarget.OthersBuffered, transform.rotation);
    }
    [PunRPC]
    private void SyncRotation(Quaternion rotation)
    {
        networkRotation = rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

}

