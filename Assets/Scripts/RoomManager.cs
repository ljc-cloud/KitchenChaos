using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RoomManager : BaseManager
{
    public RoomInfo CurrentRoomInfo { get; private set; }
    public bool JoinedRoom { get; private set; }
    public List<RoomPlayerInfo> RoomPlayerList { get; private set; }
    public List<int> RoomClientIdList { get; private set; }

    public event Action<RoomPlayerInfo> OnRoomPlayerJoin;
    public event Action<RoomPlayerInfo> OnRoomPlayerQuit;
    public event Action<RoomPlayerInfo> OnRoomPlayerReadyChanged;

    public event Action OnRoomPlayerAllReady;

    public RoomManager()
    {
        RoomPlayerList = new List<RoomPlayerInfo>();
        RoomClientIdList = new();
    }

    public override void OnInit()
    {
        GameInterface.Interface.TcpClient.OnClientCloseConnection += TcpClientCloseConnection;
        base.OnInit();
    }

    private void TcpClientCloseConnection()
    {
        if (CurrentRoomInfo != null)
        {
            Debug.Log("连接中断, 退出房间！");
            QuitRoomRequest quitRoomRequest = GameInterface.Interface.RequestManager.GetRequest<QuitRoomRequest>();
            quitRoomRequest.SendQuitRoomRequest();
        }
    }


    public void JoinRoom(RoomInfo roomInfo, List<RoomPlayerInfo> roomPlayerList)
    {
        JoinedRoom = true;
        CurrentRoomInfo = roomInfo;
        RoomPlayerList = roomPlayerList;
    }

    public void AddAllRoomPlayer(RoomInfo roomInfo, List<RoomPlayerInfo> roomPlayerList)
    {
        JoinedRoom = true;
        CurrentRoomInfo = roomInfo;
        RoomPlayerList = roomPlayerList;
    }

    public void JoinNewRoomPlayer(RoomPlayerInfo roomPlayerInfo)
    {
        Debug.Log("JoinNewRoomPlayer!!!");
        RoomPlayerList.Add(roomPlayerInfo);

        Debug.Log("触发JoinRoom!!!");
        OnRoomPlayerJoin?.Invoke(roomPlayerInfo);
    }

    public void RoomPlayerReady(int playerId, bool ready)
    {
        RoomPlayerInfo roomPlayerInfo = RoomPlayerList.Find(item => item.id == playerId);
        roomPlayerInfo.ready = ready;
        OnRoomPlayerReadyChanged?.Invoke(roomPlayerInfo);
        bool allReady = RoomPlayerList.All(item => item.ready);
        if (allReady)
        {
            OnRoomPlayerAllReady?.Invoke();
        }
    }

    public void QuitRoom(int playerId)
    {
        int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;
        if (localPlayerId == playerId)
        {
            CurrentRoomInfo = null;
            JoinedRoom = false;
        }

        RoomPlayerInfo roomPlayerInfo = RoomPlayerList.Find(item => item.id == playerId);
        RoomPlayerList.Remove(roomPlayerInfo);

        OnRoomPlayerQuit?.Invoke(roomPlayerInfo);
    }
}