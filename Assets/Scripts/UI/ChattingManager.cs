using Photon.Pun;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class ChattingManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] GameObject chatPanel;
    [SerializeField] TMP_Text chatLog;
    [SerializeField] Button sendChatBtn;
    [SerializeField] Button chatToggleBtn;

    private bool inputEnter => Input.GetKeyDown(KeyCode.Return);

    private void Update()
    {
        if (inputEnter)
        {
            SendChatting($"{PhotonNetwork.LocalPlayer.NickName} : {chatInput.text} \n");
            photonView.RPC("RPC_Chat", RpcTarget.Others, $"{PhotonNetwork.LocalPlayer.NickName} : {chatInput.text} \n");
        }
    }
    private void Start()
    {

        chatToggleBtn.onClick.AddListener(() => { ToggleChatLog(); });
        sendChatBtn.onClick.AddListener(() => { SendChatting($"{PhotonNetwork.LocalPlayer.NickName} : {chatInput.text} \n"); });
        sendChatBtn.onClick.AddListener(() => { photonView.RPC("RPC_Chat", RpcTarget.Others, $"{PhotonNetwork.LocalPlayer.NickName} : {chatInput.text} \n"); });
        chatLog.text = "";
        chatInput.text = "";
        PhotonNetwork.SendRate = 10;  // Default is 20
        // Set the serialization rate (number of times per second that the objects' state is serialized)
        PhotonNetwork.SerializationRate = 5;  // Default is 10
    }
    private void ToggleChatLog()
    {

        chatPanel.SetActive(!chatPanel.activeInHierarchy);
    }
    private void SendChatting(string message)
    {
        if (message.IsNullOrEmpty() || !chatPanel.activeInHierarchy)
            return;
        chatLog.text += message;
    }
    [PunRPC]
    void RPC_Chat(string message)
    {
        SendChatting(message);
    }
}
