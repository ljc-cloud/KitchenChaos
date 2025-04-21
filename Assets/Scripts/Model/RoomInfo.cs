using System;
using SocketProtocol;

public enum RoomVisibility
{
    None,
    Public,
    Private,
}

public class RoomInfo
{
    public string roomCode;
    public string roomName;
    public int maxPlayer;
    public int currentPlayers;
    public RoomVisibility roomVisibility;

    public RoomInfo()
    {

    }

    public RoomInfo(RoomInfoPack roomInfoPack)
    {
        roomCode = roomInfoPack.RoomCode;
        roomName = roomInfoPack.RoomName;
        roomVisibility = Enum.Parse<RoomVisibility>(roomInfoPack.RoomVisibility.ToString());
        maxPlayer = roomInfoPack.MaxPlayer;
        currentPlayers = roomInfoPack.CurrentPlayers;
    }

    public override string ToString()
    {
        return $"roomCode: {roomCode}, roomName: {roomName}, roomVisibility: {roomVisibility}" +
               $", currentPlayers: {currentPlayers}, maxPlayer: {maxPlayer}";
    }
}