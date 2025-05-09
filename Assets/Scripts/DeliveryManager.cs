using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Net.Request;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {
    public event EventHandler OnRecipeSpawned;
    public event EventHandler<RecipeCompletedEventArgs> OnRecipeCompleted;
    // public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    
    public class RecipeCompletedEventArgs : EventArgs
    {
        public int playerId;
        public int coin;
    }

    public static DeliveryManager Instance { get; private set; }


    [SerializeField] private RecipeListSO recipeListSO;


    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipesAmount;
    
    private DeliverRecipeRequest _mDeliverRecipeRequest;

    private readonly Dictionary<int, int> _mPlayerIdToCoinDict = new();
    public ReadOnlyDictionary<int,int> PlayerIdToCoinDict => new(_mPlayerIdToCoinDict);

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
        _mDeliverRecipeRequest = GameInterface.Interface.RequestManager.GetRequest<DeliverRecipeRequest>();
    }

    private void Start()
    {
        List<RoomPlayerInfo> playerList = GameInterface.Interface.RoomManager.RoomPlayerList;
        for (int i = 0; i < playerList.Count; i++)
        {
            _mPlayerIdToCoinDict[playerList[i].id] = 0;
        }
    }

    private void Update()
    {
        // SpawnLocalRecipe();
        
    }

    private void SpawnLocalRecipe()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(int playerId, bool isLocal, PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // Has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    // Cycling through all ingredients in the Recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // Cycling through all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            // Ingredient matches!
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound)
                    {
                        // This Recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    // Player delivered the correct recipe!
                    successfulRecipesAmount++;
                    if (isLocal) 
                        _mDeliverRecipeRequest.SendDeliverRecipeRequest(waitingRecipeSO.id);
                    // waitingRecipeSOList.RemoveAt(i);

                    int tip = waitingRecipeSO.tip;

                    _mPlayerIdToCoinDict[playerId] += tip;

                    OnRecipeCompleted?.Invoke(this, new RecipeCompletedEventArgs
                    {
                        playerId = playerId,
                        coin = _mPlayerIdToCoinDict[playerId]
                    });
                    return;
                }
            }
        }

        // No matches found!
        // Player did not deliver a correct recipe
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }

    public void UpdateRecipe(int[] recipeIdArray)
    {
        List<RecipeSO> recipeSOList = recipeIdArray.Select(id => recipeListSO.GetRecipeSOFromId(id)).ToList();
        waitingRecipeSOList = recipeSOList;
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }
}
