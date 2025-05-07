using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CreateRoomUI : BaseUIPanel
{
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_Dropdown roomVisibilityDropdown;
    [SerializeField] private TMP_Dropdown maxPlayerDropdown;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    private CreateRoomRequest _mCreateRoomRequest;

    public override void OnInit()
    {
        _mCreateRoomRequest = GameInterface.Interface.RequestManager.GetRequest<CreateRoomRequest>();
        base.OnInit();
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(CreateRoom);
        closeButton.onClick.AddListener(() => GameInterface.Interface.UIManager.PopUIPanel());
    }

    private void CreateRoom()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("房间名为空...");
            GameInterface.Interface.UIManager.ShowMessage("房间名为空");
            return;
        }

        string roomVisibilityText = roomVisibilityDropdown.options[roomVisibilityDropdown.value].text;
        string maxPlayerText = maxPlayerDropdown.options[maxPlayerDropdown.value].text;

        _mCreateRoomRequest.SendCreateRoomRequest(roomInfo =>
        {
            roomInfo.roomName = roomName;
            roomInfo.roomVisibility = Enum.Parse<RoomVisibility>(roomVisibilityText);
            roomInfo.maxPlayer = Convert.ToInt32(maxPlayerText);
        }, () =>
        {
            GameInterface.Interface.UIManager.HideUIPanel(UIPanelType.CreateRoomUI);
            GameInterface.Interface.UIManager.HideUIPanel(UIPanelType.RoomListUI);
            GameInterface.Interface.UIManager.HideUIPanel(UIPanelType.MainMenuUI);
            
            GameInterface.Interface.EventSystem.Publish<PlayerEnterRoomEvent>();
        });
    }
}