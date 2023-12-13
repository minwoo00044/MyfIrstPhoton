using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPunCallbacks
{
    [SerializeField] float forceAmount;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { // Ư�� �Է��� ���� ��
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.forward, out hit, float.MaxValue))
            { // ���տ� ��ü�� �ִ��� �Ǻ�
                Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.forward * float.MaxValue, Color.red);
                Debug.Log(hit.collider.name);
                CharacterController cc = hit.transform.GetComponent<CharacterController>();
                if (cc != null)
                { // ��ü�� CharacterController�� �ִ� ���
                    Vector3 direction = hit.transform.position - transform.position; // �÷��̾�� ��ü ������ ���� ���
                    direction = direction.normalized; // ������ ����

                    // PunRPC�� ����Ͽ� ��Ʈ��ũ�� ����� ��� Ŭ���̾�Ʈ���� �� �޼��带 ȣ���ϵ��� ��û
                    photonView.RPC("ApplyForce", RpcTarget.All, hit.transform.GetComponent<PhotonView>().ViewID, direction, forceAmount);
                }
            }
        }
    }

    // PunRPC ��Ʈ����Ʈ�� ����Ͽ� �� �޼��带 RPC �޼���� ����ϴ�.
    [PunRPC]
    void ApplyForce(int viewID, Vector3 direction, float force)
    {
        // ��Ʈ��ũ�� ����� ��� Ŭ���̾�Ʈ���� �ش� ��ü�� ã��
        CharacterController cc = PhotonView.Find(viewID).GetComponent<CharacterController>();

        // ���� ���ؾ� �� ��
        if (force > 0)
        {
            // ���� �縸ŭ �̵�
            cc.Move(direction * force * Time.deltaTime);
            // ���� ���� ���� (�� �����Ӹ��� ���� ���� �ٿ��� ���� ���߰� ��)
            force -= Time.deltaTime * 100; // 100�� ������ ������, �ʿ信 ���� �����ϼ���.
            if (force < 0) force = 0; // ���� ���� 0���� �۾����� �ʵ��� ��
        }
    }
}
