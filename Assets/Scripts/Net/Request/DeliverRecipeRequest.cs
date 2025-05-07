using System;
using System.Linq;
using SocketProtocol;

namespace Net.Request
{
    public class DeliverRecipeRequest : BaseRequest
    {
        public DeliverRecipeRequest()
        {
            Request = RequestCode.Game;
            Action = ActionCode.UpdateRecipe;
        }
        
        public void SendDeliverRecipeRequest(int deliverRecipeId, Action onComplete = null)
        {
            RoomInfoPack roomInfoPack = new RoomInfoPack()
            {
                RoomCode = GameInterface.Interface.RoomManager.CurrentRoomInfo.roomCode
            };
            MainPack mainPack = new MainPack
            {
                RequestCode = Request,
                ActionCode = Action,
                RoomInfoPack = roomInfoPack,
                DeliverRecipeId = deliverRecipeId,
            };

            OnServerSuccessResponse = onComplete;
            
            SendRequest(mainPack);
        }
    }
}