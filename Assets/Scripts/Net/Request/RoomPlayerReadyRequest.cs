using System;
using SocketProtocol;

public class RoomPlayerReadyRequest : BaseRequest
{

    // public event Action<int> OnPlayerReadyChanged;

    public RoomPlayerReadyRequest()
    {
        Request = RequestCode.Room;
        Action = ActionCode.PlayerReady;
    }

    protected override void HandleServerSuccessResponse(MainPack pack)
    {
        // int clientId = pack.ClientPack.ClientId;
        bool ready = pack.RoomPlayerReadyPack.Ready;
        int playerId = pack.PlayerInfoPack.Id;

        GameInterface.Interface.RoomManager.RoomPlayerReady(playerId, ready);


        base.HandleServerSuccessResponse(pack);
    }

    protected override void HandleServerFailResponse(MainPack pack)
    {
        base.HandleServerFailResponse(pack);
    }

    public void SendRoomPlayerReadyRequest(bool ready, Action onSuccess = null, Action onFail = null)
    {
        RoomPlayerReadyPack roomPlayerReadyPack = new RoomPlayerReadyPack { Ready = ready };

        string roomCode = GameInterface.Interface.RoomManager.CurrentRoomInfo.roomCode;
        roomCode = CharsetUtil.DefaultToUTF8(roomCode);

        RoomInfoPack roomInfoPack = new RoomInfoPack
        {
            RoomCode = roomCode,
        };

        MainPack mainPack = new MainPack
        {
            RequestCode = Request,
            ActionCode = Action,
            RoomPlayerReadyPack = roomPlayerReadyPack,
            RoomInfoPack = roomInfoPack
        };

        SendRequest(mainPack);
    }
}