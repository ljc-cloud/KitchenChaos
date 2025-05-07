using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RoomTabUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI roomPlayers;
    [SerializeField] private Image roomVisibility;

    private JoinRoomRequest _mJoinRoomRequest;
    private string _mRoomCode;

    private void Start()
    {
        _mJoinRoomRequest = GameInterface.Interface.RequestManager.GetRequest<JoinRoomRequest>();
    }

    public void SetRoomTab(RoomInfo roomInfo)
    {
        _mRoomCode = roomInfo.roomCode;
        roomName.text = roomInfo.roomName;
        roomPlayers.text = $"{roomInfo.currentPlayers}/{roomInfo.maxPlayer}";
        roomVisibility.color = roomInfo.roomVisibility
                               == RoomVisibility.Public
            ? Color.green
            : Color.red;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;
        if (clickCount == 2)
        {
            Debug.Log($"进入房间=> {_mRoomCode}");
            _mJoinRoomRequest.SendJoinRoomRequest(_mRoomCode, onJoinRoomSuccess: OnJoinRoomSuccess);
        }
    }

    private void OnJoinRoomSuccess()
    {
        GameInterface.Interface.UIManager.HideUIPanel(UIPanelType.CreateRoomUI);
        GameInterface.Interface.UIManager.HideUIPanel(UIPanelType.RoomListUI);
        GameInterface.Interface.UIManager.HideUIPanel(UIPanelType.MainMenuUI);
        GameInterface.Interface.SceneLoader.LoadScene(Scene.RoomScene);
        
        GameInterface.Interface.EventSystem.Publish<PlayerEnterRoomEvent>();
    }
}