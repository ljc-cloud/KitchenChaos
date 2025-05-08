using System.Linq;
using SocketProtocol;
using UnityEngine;

namespace Net.Request
{
    public class UpdateRecipeResponse : BaseRequest
    {
        public UpdateRecipeResponse()
        {
            Request = RequestCode.Game;
            Action = ActionCode.UpdateRecipe;
        }

        protected override void HandleServerSuccessResponse(MainPack pack)
        {
            Debug.Log("接收到 UpdateRecipeResponse");
            int[] recipeIdArray = pack.RecipeIdArray.ToArray();
            DeliveryManager.Instance.UpdateRecipe(recipeIdArray);
            base.HandleServerSuccessResponse(pack);
        }
    }
}