using System;
using System.Text;
using SocketProtocol;
using UnityEngine;


public class SignInRequest : BaseRequest
{

    public SignInRequest()
    {
        Request = RequestCode.User;
        Action = ActionCode.SignIn;
    }

    protected override void HandleServerSuccessResponse(MainPack pack)
    {
        Debug.Log("Sign In Success!");
        PlayerInfoPack playerInfoPack = pack.PlayerInfoPack;
        PlayerInfo playerInfo = new PlayerInfo(playerInfoPack);
        GameInterface.Interface.LocalPlayerInfo = playerInfo;
        GameInterface.Interface.TcpClient.IsOnline = true;
        Debug.Log(playerInfo);
        base.HandleServerSuccessResponse(pack);
    }

    protected override void HandleServerFailResponse(MainPack pack)
    {
        string errorMessage = pack.ReturnMessage.ErrorMessage;
        Debug.Log(errorMessage);
        Debug.Log("Sign In Fail!");
        base.HandleServerFailResponse(pack);
    }

    public void SendSignInRequest(string username, string password, Action onSignInSuccess, Action onSignInFail = null)
    {
        MainPack mainPack = new MainPack
        {
            RequestCode = Request,
            ActionCode = Action
        };

        username = Encoding.UTF8.GetString(Encoding.Default.GetBytes(username));
        password = Encoding.UTF8.GetString(Encoding.Default.GetBytes(password));

        SignInPack signInPack = new SignInPack
        {
            Username = username,
            Password = password
        };

        mainPack.SignInPack = signInPack;
        OnServerSuccessResponse += onSignInSuccess;
        OnServerFailResponse += onSignInFail;
        SendRequest(mainPack);
    }
}