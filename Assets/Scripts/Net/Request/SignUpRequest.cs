using System;
using SocketProtocol;
using UnityEngine;

public class SignUpRequest : BaseRequest
{

    public SignUpRequest()
    {
        Request = RequestCode.User;
        Action = ActionCode.SignUp;
    }

    protected override void HandleServerSuccessResponse(MainPack pack)
    {
        Debug.Log("Sign Up Success!");
        PlayerInfoPack playerInfoPack = pack.PlayerInfoPack;
        PlayerInfo playerInfo = new PlayerInfo(playerInfoPack);
        GameInterface.Interface.LocalPlayerInfo = playerInfo;

        base.HandleServerSuccessResponse(pack);
    }

    protected override void HandleServerFailResponse(MainPack pack)
    {
        Debug.Log("Sign Up Fail!");
        base.HandleServerFailResponse(pack);
    }

    public void SendSignUpRequest(string username, string nickname, string password, Action onResponse,
        Action onSignInFail = null)
    {
        MainPack mainPack = new MainPack
        {
            RequestCode = Request,
            ActionCode = Action
        };

        username = CharsetUtil.DefaultToUTF8(username);
        password = CharsetUtil.DefaultToUTF8(password);
        nickname = CharsetUtil.DefaultToUTF8(nickname);

        SignUpPack signUpPack = new SignUpPack
        {
            Username = username,
            Nickname = nickname,
            Password = password
        };
        mainPack.SignUpPack = signUpPack;
        OnServerSuccessResponse += onResponse;
        OnServerFailResponse += onSignInFail;
        SendRequest(mainPack);
    }
}