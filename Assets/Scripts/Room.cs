using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] playersPositionArray;
    [SerializeField] private Color[] playerMaterialColorArray;
    
    private void Awake()
    {
        List<RoomPlayerInfo> roomPlayerInfoList = GameInterface.Interface.RoomManager.RoomPlayerList;
        int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;

        for (int i = 0; i < roomPlayerInfoList.Count; i++)
        {
            Vector3 position = playersPositionArray[i].position;
            GameObject playerGameObject = Instantiate(playerPrefab, position, Quaternion.identity);
            Entity entity = playerGameObject.GetComponent<Entity>();
            if (roomPlayerInfoList[i].id == localPlayerId)
            {
                entity.playerType = Entity.PlayerType.Local;
                entity.playerId = localPlayerId;
            }
            else
            {
                entity.playerType = Entity.PlayerType.Remote;
                entity.playerId = roomPlayerInfoList[i].id;
            }
            
            playerGameObject.GetComponentInChildren<PlayerVisual>().SetPlayerVisual(playerMaterialColorArray[i]);
            playerGameObject.GetComponent<Player>().SetPlayerId(roomPlayerInfoList[i].id);
            
            GameInterface.Interface.GameFrameSyncManager.AddEntity(entity);
        }
    }
}