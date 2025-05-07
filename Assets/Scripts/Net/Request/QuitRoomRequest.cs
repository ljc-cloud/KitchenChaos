using System;
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
        int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;

        int playerId = pack.PlayerInfoPack.Id;
        GameInterface.Interface.RoomManager.QuitRoom(playerId);

        if (localPlayerId == playerId)
        {
            Invoker.Instance.DelegateList.Add(() =>
            {
                GameInterface.Interface.SceneLoader.LoadSceneAsync(Scene.MainMenuScene,
                    () =>
                    {
                        GameInterface.Interface.UIManager.PushUIPanelAppend(UIPanelType.RoomListUI,
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