using System;
using SocketProtocol;
using UnityEngine;

public class BaseRequest
{
    protected Action OnServerSuccessResponse;
    protected Action OnServerFailResponse;

    public RequestCode Request { get; protected set; }
    public ActionCode Action { get; protected set; }

    protected BaseRequest()
    {

    }

    public void HandleServerResponse(MainPack pack)
    {
        switch (pack.ReturnCode)
        {
            case ReturnCode.Success:
                HandleServerSuccessResponse(pack);
                break;
            case ReturnCode.Fail:
                HandleServerFailResponse(pack);
                break;
        }
    }

    protected virtual void HandleServerSuccessResponse(MainPack pack)
    {
        Invoker.Instance.DelegateList.Add(() =>
        {
            if (pack.ReturnMessage != null)
            {
                if (!string.IsNullOrEmpty(pack.ReturnMessage.SuccessMessage))
                {
                    GameInterface.Interface.UIManager.ShowMessage(pack.ReturnMessage.SuccessMessage);
                }
            }

            OnServerSuccessResponse?.Invoke();
        });
    }

    protected virtual void HandleServerFailResponse(MainPack pack)
    {
        Invoker.Instance.DelegateList.Add(() =>
        {
            if (pack.ReturnMessage != null)
            {
                if (!string.IsNullOrEmpty(pack.ReturnMessage.ErrorMessage))
                {
                    GameInterface.Interface.UIManager.ShowMessage(pack.ReturnMessage.ErrorMessage);
                }
            }

            OnServerFailResponse?.Invoke();
        });
    }

    protected void SendRequest(MainPack pack)
    {
        GameInterface.Interface.TcpClient.Send(pack);
    }
}