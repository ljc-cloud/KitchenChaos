using System;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomVisual : MonoBehaviour
{
        [SerializeField] private Color[] roomPlayerMaterialColorArray;
        [SerializeField] private GameObject roomPlayerPrefab;
        [SerializeField] private Transform[] roomPlayerPositionArray;

        private Dictionary<RoomPlayerInfo, RoomPlayer> _mRoomPlayerInfoToRoomPlayerDict = new();

        private bool[] _mRoomPlayerPositionAvailable;

        private void Start()
        {
            _mRoomPlayerPositionAvailable = new bool[roomPlayerPositionArray.Length];
            Array.Fill(_mRoomPlayerPositionAvailable, true);
            
            GameInterface.Interface.RoomManager.OnRoomPlayerJoin += OnRoomPlayerJoin;
            GameInterface.Interface.RoomManager.OnRoomPlayerQuit += OnRoomPlayerQuit;
            GameInterface.Interface.RoomManager.OnRoomPlayerReadyChanged += OnRoomPlayerReadyChanged;
            SpawnRoomPlayers();
        }
        private void OnDestroy()
        {
            GameInterface.Interface.RoomManager.OnRoomPlayerJoin -= OnRoomPlayerJoin;
            GameInterface.Interface.RoomManager.OnRoomPlayerQuit -= OnRoomPlayerQuit;
            GameInterface.Interface.RoomManager.OnRoomPlayerReadyChanged -= OnRoomPlayerReadyChanged;
        }

        private void OnRoomPlayerJoin(RoomPlayerInfo roomPlayerInfo)
        {
            Invoker.Instance.DelegateList.Add(() =>
            {
                Debug.Log("OnRoomPlayerJoin");

                int availableIndex = -1;
                for (int i = 0; i < _mRoomPlayerPositionAvailable.Length; i++)
                {
                    if (_mRoomPlayerPositionAvailable[i])
                    {
                        availableIndex = i;
                        break;
                    }
                }
                
                RoomPlayer roomPlayer = SpawnRoomPlayer(availableIndex, roomPlayerInfo);
                _mRoomPlayerInfoToRoomPlayerDict.Add(roomPlayerInfo, roomPlayer);
            });
        }
        private void OnRoomPlayerQuit(RoomPlayerInfo roomPlayerInfo)
        {
            Invoker.Instance.DelegateList.Add(() =>
            {
                RoomPlayer roomPlayer = _mRoomPlayerInfoToRoomPlayerDict[roomPlayerInfo];
                _mRoomPlayerInfoToRoomPlayerDict.Remove(roomPlayerInfo);
                int playerIndex = roomPlayer.RoomIndex;
                _mRoomPlayerPositionAvailable[playerIndex] = true;
                Destroy(roomPlayer.gameObject);
            });
        }
        private void OnRoomPlayerReadyChanged(RoomPlayerInfo roomPlayerInfo)
        {
            Invoker.Instance.DelegateList.Add(() =>
            {
                RoomPlayer roomPlayer = _mRoomPlayerInfoToRoomPlayerDict[roomPlayerInfo];
                roomPlayer.SetReady(roomPlayerInfo.ready);
            });
        }

        private void SpawnRoomPlayers()
        {
            List<RoomPlayerInfo> roomPlayerInfoList = GameInterface.Interface.RoomManager.RoomPlayerList;
            for (int i = 0; i < roomPlayerInfoList.Count; i++)
            {
                RoomPlayer roomPlayer = SpawnRoomPlayer(i, roomPlayerInfoList[i]);
                _mRoomPlayerInfoToRoomPlayerDict.Add(roomPlayerInfoList[i], roomPlayer);
                _mRoomPlayerPositionAvailable[i] = false;
            }
        }

        private RoomPlayer SpawnRoomPlayer(int index, RoomPlayerInfo roomPlayerInfo)
        {
            Transform roomPlayerPosition = roomPlayerPositionArray[index];
            GameObject roomPlayerGameObject = Instantiate(roomPlayerPrefab, roomPlayerPosition.position, roomPlayerPosition.rotation);
            RoomPlayer roomPlayer = roomPlayerGameObject.GetComponent<RoomPlayer>();
            roomPlayer.SetRoomPlayer(index, roomPlayerInfo.nickname, roomPlayerMaterialColorArray[index]);
            roomPlayer.SetReady(roomPlayerInfo.ready);
            return roomPlayer;
        }
    }