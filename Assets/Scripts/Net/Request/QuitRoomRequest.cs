using SocketProtocol;

public class QuitRoomRequest : BaseRequest
{
    public QuitRoomRequest()
    {
        Request = RequestCode.Room;
        Action = ActionCode.QuitRoom;
    }

    protected override void HandleServerSuccessResponse(MainPack pack)
    {
        int localClientId = GameInterface.Interface.TcpClient.ClientId;
        int clientId = pack.ClientPack.ClientId;

        int playerId = GameInterface.Interface.LocalPlayerInfo.id;
        GameInterface.Interface.RoomManager.QuitRoom(playerId);

        if (localClientId == clientId)
        {

            Invoker.Instance.DelegateList.Add(() =>
            {
                GameInterface.Interface.SceneLoader.LoadSceneAsync(Scene.MainMenuScene,
                    () =>
                    {
                        GameInterface.Interface.UIManager.PushUIPanel(UIPanelType.RoomListUI,
                            ShowUIPanelType.FadeIn);
                    });
            });
        }

        base.HandleServerSuccessResponse(pack);
    }


    public void SendQuitRoomRequest()
    {
        string roomCode = GameInterface.Interface.RoomManager.CurrentRoomInfo.roomCode;

        roomCode = CharsetUtil.DefaultToUTF8(roomCode);

        RoomInfoPack roomInfoPack = new RoomInfoPack { RoomCode = roomCode };

        MainPack mainPack = new MainPack
        {
            RequestCode = Request,
            ActionCode = Action,
            RoomInfoPack = roomInfoPack,
        };

        SendRequest(mainPack);
    }
}