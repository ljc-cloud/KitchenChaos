using Google.Protobuf.WellKnownTypes;
using SocketProtocol;
using Enum = System.Enum;

public class GameStateChangeResponse : BaseRequest
{
    public GameStateChangeResponse()
    {
        Request = RequestCode.Game;
        Action = ActionCode.ChangeGameState;
    }

    protected override void HandleServerSuccessResponse(MainPack pack)
    {
        SocketProtocol.GameState packGameState = pack.CurrentGameState;
        KitchenGameManager.State gameState =
            Enum.Parse<KitchenGameManager.State>(packGameState.ToString());
        GameInterface.Interface.EventSystem.Publish(new ChangeGameStateEvent
        {
            newState = gameState
        });

        base.HandleServerSuccessResponse(pack);
    }

    protected override void HandleServerFailResponse(MainPack pack)
    {
        base.HandleServerFailResponse(pack);
    }
}