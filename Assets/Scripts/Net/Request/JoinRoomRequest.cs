using System;
using System.Collections.Generic;
using System.Linq;
using SocketProtocol;
using UnityEngine;

public class JoinRoomRequest : BaseRequest
{
    public JoinRoomRequest()
    {
        Request = RequestCode.Room;
        Action = ActionCode.JoinRoom;
    }

    protected override void HandleServerSuccessResponse(MainPack pack)
    {
        int localClientId = GameInterface.Interface.TcpClient.ClientId;
        int clientId = pack.ClientPack.ClientId;

        if (localClientId == clientId)
        {
            Debug.Log("本地玩家加入房间");
            RoomInfoPack roomInfoPack = pack.RoomInfoPack;
            RoomInfo currentRoomInfo = new RoomInfo
            {
                currentPlayers = roomInfoPack.CurrentPlayers,
                maxPlayer = roomInfoPack.MaxPlayer,
                roomCode = roomInfoPack.RoomCode,
                roomName = roomInfoPack.RoomName,
                roomVisibility = Enum.Parse<RoomVisibility>(roomInfoPack.RoomVisibility.ToString()),
            };
            List<RoomPlayerInfo> roomPlayerInfoList = pack.JoinRoomResultPack.RoomPlayerInfoList.Select(item =>
            {
                RoomPlayerInfo roomPlayerInfo = new RoomPlayerInfo
                {
                    id = item.Id,
                    nickname = item.Nickname,
                    username = item.Username,
                    ready = item.Ready,
                };
                return roomPlayerInfo;
            }).ToList();
            GameInterface.Interface.RoomManager.JoinRoom(currentRoomInfo, roomPlayerInfoList);
        }
        else
        {
            Debug.Log("其他玩家加入房间");
            RoomPlayerInfoPack roomPlayerInfoPack = pack.RoomPlayerInfoPack;
            RoomPlayerInfo roomPlayerInfo = new RoomPlayerInfo
            {
                id = roomPlayerInfoPack.Id,
                nickname = roomPlayerInfoPack.Nickname,
                username = roomPlayerInfoPack.Username,
                ready = roomPlayerInfoPack.Ready,
            };
            GameInterface.Interface.RoomManager.JoinNewRoomPlayer(roomPlayerInfo);
        }

        base.HandleServerSuccessResponse(pack);
    }

    protected override void HandleServerFailResponse(MainPack pack)
    {
        Debug.Log("加入房间失败");
        base.HandleServerFailResponse(pack);
    }


    public void SendJoinRoomRequest(string roomCode, Action onJoinRoomSuccess, Action onJoinRoomFail = null)
    {
        RoomInfoPack roomInfoPack = new RoomInfoPack
        {
            RoomCode = CharsetUtil.DefaultToUTF8(roomCode),
        };

        MainPack mainPack = new MainPack
        {
            RequestCode = Request,
            ActionCode = Action,
            RoomInfoPack = roomInfoPack,
        };

        OnServerSuccessResponse += onJoinRoomSuccess;
        OnServerFailResponse += onJoinRoomFail;

        SendRequest(mainPack);
    }
}