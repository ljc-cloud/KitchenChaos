using System;
using System.Collections.Generic;
using SocketProtocol;
using UnityEngine;

public class SearchRoomRequest : BaseRequest
{
    private Action<List<RoomInfo>> _mOnSearchRoomSuccess;

    public SearchRoomRequest()
    {
        Request = RequestCode.Room;
        Action = ActionCode.SearchRoom;
    }

    protected override void HandleServerSuccessResponse(MainPack pack)
    {
        Debug.Log("搜索房间成功!");
        List<RoomInfo> roomInfoList = new List<RoomInfo>();
        foreach (var roomInfoPack in pack.SearchRoomResultPack.RoomInfoList)
        {
            roomInfoList.Add(new RoomInfo(roomInfoPack));
        }

        Invoker.Instance.DelegateList.Add(() => _mOnSearchRoomSuccess?.Invoke(roomInfoList));

        base.HandleServerSuccessResponse(pack);
    }

    protected override void HandleServerFailResponse(MainPack pack)
    {
        Debug.Log("搜索房间失败!");
        base.HandleServerFailResponse(pack);
    }


    public void SendSearchRoomRequest(Action<RoomInfo> condition, Action<List<RoomInfo>> onComplete = null)
    {
        RoomInfo roomInfo = new RoomInfo();
        condition?.Invoke(roomInfo);

        Debug.Log("搜索房间条件为：" + roomInfo);

        RoomInfoPack roomInfoPack = new RoomInfoPack
        {
            RoomName = CharsetUtil.DefaultToUTF8(roomInfo.roomName),
            RoomVisibility = Enum.Parse<SocketProtocol.RoomVisibility>(roomInfo.roomVisibility.ToString()),
        };
        MainPack mainPack = new MainPack
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.SearchRoom,
            RoomInfoPack = roomInfoPack,
        };

        _mOnSearchRoomSuccess = onComplete;

        SendRequest(mainPack);
    }
}