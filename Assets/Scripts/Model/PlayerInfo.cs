using System;
using SocketProtocol;


public class PlayerInfo
{
    public PlayerInfo()
    {

    }

    public PlayerInfo(PlayerInfoPack pack)
    {
        id = pack.Id;
        username = pack.Username;
        nickname = pack.Nickname;
    }

    public int id;
    public string username;
    public string nickname;
    // public string password;

    public override string ToString()
    {
        return $"id: {id}, username: {username}, nickname: {nickname}";
    }
}