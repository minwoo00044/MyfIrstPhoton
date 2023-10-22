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
            Debug.Log("서버에 연결되어 있지 않습니다.");
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
            Debug.Log("닉네임을 입력해주세요.");
        }
    }


    //서버에 연결되었을 때 호출되는 콜백 함수입니다.
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버에 연결되었습니다.");
    }

    //로비에 입장했을 때 호출되는 콜백 함수입니다.
    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 입장하였습니다.");

        //게임씬으로 이동합니다.
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
