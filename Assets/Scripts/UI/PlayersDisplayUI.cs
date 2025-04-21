using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UI;
using UnityEngine;

public class PlayersDisplayUI : MonoBehaviour
{
    [SerializeField] private GameObject playerDisplayUIPrefab;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += OnGameStateChanged;
        Debug.Log("PlayersDisplayUI::Start");
    }

    private void OnGameStateChanged(object sender, EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameOver())
        {
            List<RoomPlayerInfo> roomPlayerList = GameInterface.Interface.RoomManager.RoomPlayerList;
            ReadOnlyDictionary<int,int> playerIdToCoinDict = DeliveryManager.Instance.PlayerIdToCoinDict;
            IOrderedEnumerable<KeyValuePair<int, int>> orderedPlayerIdToCoinDict =
                from pair in playerIdToCoinDict orderby pair.Value descending select pair;

            int rank = 0;
            foreach (var playerIdToCoinKeyValuePair in orderedPlayerIdToCoinDict)
            {
                string nickname = roomPlayerList.Find(item => item.id == playerIdToCoinKeyValuePair.Key).nickname;
                GameObject playerDisplayUIGameObject = Instantiate(playerDisplayUIPrefab, transform);
                playerDisplayUIGameObject.GetComponent<PlayerDisplayUI>().SetPlayerDisplayUI(rank, nickname
                    , playerIdToCoinKeyValuePair.Value);
                rank++;
            }
        }
    }
}