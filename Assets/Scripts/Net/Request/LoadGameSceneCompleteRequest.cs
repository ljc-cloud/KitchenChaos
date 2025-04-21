using System;
using SocketProtocol;
using UnityEngine;

public class LoadGameSceneCompleteRequest : BaseRequest
{
    public LoadGameSceneCompleteRequest()
    {
        Request = RequestCode.Game;
        Action = ActionCode.LoadGameSceneComplete;
    }

    private GameState _mChangeState;

    protected override void HandleServerSuccessResponse(MainPack pack)
    {
        Debug.Log("LoadGameSceneCompleteRequest:LoadGameScene...");
        base.HandleServerSuccessResponse(pack);
    }
        
    protected override void HandleServerFailResponse(MainPack pack)
    {
        base.HandleServerFailResponse(pack);
    }

    public void SendLoadGameSceneCompleteRequest(Action onSuccess = null, Action onFail = null)
    {
        int playerId = GameInterface.Interface.LocalPlayerInfo.id;
        string roomCode = GameInterface.Interface.RoomManager.CurrentRoomInfo.roomCode;

        PlayerInfoPack playerInfoPack = new PlayerInfoPack { Id = playerId };
        RoomInfoPack roomInfoPack = new RoomInfoPack
        {
            RoomCode = CharsetUtil.DefaultToUTF8(roomCode),
        };
        MainPack mainPack = new MainPack
        {
            RequestCode = Request,
            ActionCode = Action,
            PlayerInfoPack = playerInfoPack,
            RoomInfoPack = roomInfoPack,
        };
        OnServerSuccessResponse += onSuccess;
        OnServerFailResponse += onFail;
            
        SendRequest(mainPack);
    }
}
