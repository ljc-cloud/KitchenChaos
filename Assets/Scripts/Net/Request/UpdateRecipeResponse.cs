using System.Linq;
using SocketProtocol;

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
            int[] recipeIdArray = pack.RecipeIdArray.ToArray();
            DeliveryManager.Instance.UpdateRecipe(recipeIdArray);
            base.HandleServerSuccessResponse(pack);
        }
    }
}