using System.Collections.Generic;
using Net.Request;
using SocketProtocol;
using UnityEngine;

public class RequestManager : BaseManager
{
    private readonly Dictionary<ActionCode, BaseRequest> _mRequestDict = new();

    private List<BaseRequest> _mRequestList = new();

    public override void OnInit()
    {
        base.OnInit();
        InitRequests();

        GameInterface.Interface.TcpClient.OnServerResponse += HandleServerResponse;
    }

    private void InitRequests()
    {
        _mRequestList.Add(new SignInRequest());
        _mRequestList.Add(new SignUpRequest());
        _mRequestList.Add(new SearchRoomRequest());
        _mRequestList.Add(new RoomPlayerReadyRequest());
        _mRequestList.Add(new JoinRoomRequest());
        _mRequestList.Add(new CreateRoomRequest());
        _mRequestList.Add(new QuitRoomRequest());
        _mRequestList.Add(new ReadyStartGameResponse());
        _mRequestList.Add(new LoadGameSceneCompleteRequest());
        _mRequestList.Add(new GameStateChangeResponse());
        _mRequestList.Add(new DeliverRecipeRequest());
        _mRequestList.Add(new UpdateRecipeResponse());

        foreach (var request in _mRequestList)
        {
            _mRequestDict.Add(request.Action, request);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        foreach (var request in _mRequestList)
        {
            RemoveRequest(request.Action);
        }

        GameInterface.Interface.TcpClient.OnServerResponse -= HandleServerResponse;
    }

    private void HandleServerResponse(MainPack pack)
    {
        _mRequestDict[pack.ActionCode].HandleServerResponse(pack);
    }

    public void AddRequest(BaseRequest request)
    {
        _mRequestDict[request.Action] = request;
    }

    public void RemoveRequest(ActionCode actionCode)
    {
        _mRequestDict.Remove(actionCode);
    }

    public T GetRequest<T>() where T : BaseRequest
    {
        foreach (var request in _mRequestList)
        {
            if (request is T baseRequest)
            {
                return baseRequest;
            }
        }

        Debug.LogError("未找到对应Action的请求...");
        return null;
    }
}