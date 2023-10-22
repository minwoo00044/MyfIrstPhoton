using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class EnterManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField nameInputField;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickConnectButton()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("������ ����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }

        string nickname = nameInputField.text;

        if (!string.IsNullOrEmpty(nickname))
        {
            PhotonNetwork.NickName = nickname;
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.Log("�г����� �Է����ּ���.");
        }
    }


    //������ ����Ǿ��� �� ȣ��Ǵ� �ݹ� �Լ��Դϴ�.
    public override void OnConnectedToMaster()
    {
        Debug.Log("������ ����Ǿ����ϴ�.");
    }

    //�κ� �������� �� ȣ��Ǵ� �ݹ� �Լ��Դϴ�.
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� �����Ͽ����ϴ�.");

        //���Ӿ����� �̵��մϴ�.
        SceneManager.LoadScene("GameScene");

        // Join or create a room after entering the lobby
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;  // Set max players for the room

        PhotonNetwork.JoinOrCreateRoom("Room2", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined a room");

        StartCoroutine(this.CreatePlayer());
    }

    IEnumerator CreatePlayer()
    {
        PhotonNetwork.Instantiate("Player", new Vector3(0, 10, 0), Quaternion.identity, 0);

        yield return null;
    }
}
