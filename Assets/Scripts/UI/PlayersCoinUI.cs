using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayersCoinUI : MonoBehaviour
{
    [SerializeField] private GameObject coinUIPrefab;

    private readonly Dictionary<int, PlayerCoinUI> _mPlayerIdToCoinUIDict = new();
    
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeCompleted += OnRecipeCompleted;
        
        var playerList = GameInterface.Interface.RoomManager.RoomPlayerList;
        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject coinUIGameObject = Instantiate(coinUIPrefab, transform);
            PlayerCoinUI playerCoinUI = coinUIGameObject.GetComponent<PlayerCoinUI>();
            _mPlayerIdToCoinUIDict[playerList[i].id] = playerCoinUI;
            playerCoinUI.InitCoinUI(playerList[i].id, playerList[i].nickname);
        }
    }

    private void OnRecipeCompleted(object sender, DeliveryManager.RecipeCompletedEventArgs e)
    {
        PlayerCoinUI playerCoinUI = _mPlayerIdToCoinUIDict[e.playerId];
        playerCoinUI.UpdateCoin(e.coin);
    }
}