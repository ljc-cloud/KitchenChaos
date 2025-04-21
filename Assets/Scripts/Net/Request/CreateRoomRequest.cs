using System;
using System.Collections.Generic;
using SocketProtocol;
using UnityEngine;

public class CreateRoomRequest : BaseRequest
{
    public CreateRoomRequest()
    {
        Request = RequestCode.Room;
        Action = ActionCode.CreateRoom;
    }

    protected override void HandleServerSuccessResponse(MainPack pack)
    {
        Debug.Log("房间创建成功！");

        RoomInfoPack roomInfoPack = pack.RoomInfoPack;
        RoomInfo currentRoomInfo = new RoomInfo
        {
            roomCode = roomInfoPack.RoomCode,
            roomName = roomInfoPack.RoomName,
            roomVisibility = Enum.Parse<RoomVisibility>(roomInfoPack.RoomVisibility.ToString()),
            currentPlayers = roomInfoPack.CurrentPlayers,
            maxPlayer = roomInfoPack.MaxPlayer,
        };

        PlayerInfo playerInfo = GameInterface.Interface.LocalPlayerInfo;

        RoomPlayerInfo roomPlayerInfo = new RoomPlayerInfo
        {
            id = playerInfo.id,
            username = playerInfo.username,
            nickname = playerInfo.nickname,
            ready = false,
        };

        GameInterface.Interface.RoomManager.JoinRoom(currentRoomInfo, new List<RoomPlayerInfo> { roomPlayerInfo });

        Invoker.Instance.DelegateList.Add(() => GameInterface.Interface.SceneLoader.LoadScene(Scene.RoomScene));

        base.HandleServerSuccessResponse(pack);
    }

    protected override void HandleServerFailResponse(MainPack pack)
    {
        Debug.Log("房间创建失败");
        base.HandleServerFailResponse(pack);
    }


    public void SendCreateRoomRequest(Action<RoomInfo> condition, Action onComplete = null)
    {
        RoomInfo roomInfo = new RoomInfo();
        condition.Invoke(roomInfo);
        Debug.Log("创建房间，参数" + roomInfo);

        CreateRoomPack createRoomPack = new CreateRoomPack
        {
            RoomName = CharsetUtil.DefaultToUTF8(roomInfo.roomName),
            RoomVisibility = Enum.Parse<SocketProtocol.RoomVisibility>(roomInfo.roomVisibility.ToString()),
            MaxPlayer = roomInfo.maxPlayer,
        };

        MainPack mainPack = new MainPack
        {
            RequestCode = Request,
            ActionCode = Action,
            CreateRoomPack = createRoomPack
        };

        OnServerSuccessResponse += onComplete;

        SendRequest(mainPack);
    }
}