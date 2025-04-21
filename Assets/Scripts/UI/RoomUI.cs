using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button unreadyButton;

    private RoomPlayerReadyRequest _mRoomPlayerReadyRequest;
    private QuitRoomRequest _mQuitRoomRequest;

    private void Start()
    {
        string roomName = GameInterface.Interface.RoomManager.CurrentRoomInfo?.roomName;
        roomNameText.text = roomName;
        
        quitButton.onClick.AddListener(PlayerQuitRoom);
        readyButton.onClick.AddListener(PlayerReady);
        unreadyButton.onClick.AddListener(PlayerUnready);

        _mRoomPlayerReadyRequest = GameInterface.Interface.RequestManager.GetRequest<RoomPlayerReadyRequest>();
        _mQuitRoomRequest = GameInterface.Interface.RequestManager.GetRequest<QuitRoomRequest>();
    }

    private void PlayerQuitRoom()
    {
        _mQuitRoomRequest.SendQuitRoomRequest();
    }

    private void PlayerUnready()
    {
        _mRoomPlayerReadyRequest.SendRoomPlayerReadyRequest(false);
        readyButton.gameObject.SetActive(true);
        unreadyButton.gameObject.SetActive(false);
    }

    private void PlayerReady()
    {
        _mRoomPlayerReadyRequest.SendRoomPlayerReadyRequest(true);
        readyButton.gameObject.SetActive(false);
        unreadyButton.gameObject.SetActive(true);
    }
}