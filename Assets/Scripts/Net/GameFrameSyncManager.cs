using System;
using System.Collections.Generic;
using System.Linq;
using GameFrameSync;
using UnityEngine;

public class GameFrameSyncManager : BaseManager
{
    public enum PlayerInputType
    {
        None = 0,
        MoveLeft = 1,
        MoveRight = 2,
        Ready = 3,
        Shoot = 4,
        QuitReady = 5,
        Move = 6,
        Interact = 7,
        InteractAlt = 8,
    }

    struct PlayerInputEvent
    {
        public bool available;
        public PlayerInputType playerInputType;
    }

    private PlayerInputEvent _mLocalPlayerInputEvent;
    // private Vector3 _mLocalCurrentPosition;
    private readonly SortedList<int, ResFrameSyncData> _mHistoryFrameSyncData = new();
    private readonly List<Entity> _mEntities = new();
    private int _mSyncedFrameId;

    private ObjectPool<ReqFrameInputData> _mReqFrameInputDataPool;
    private ObjectPool<Vector2D> _mVector2DPool;


    public event Action<List<FrameInputData>> OnFrameSync;

    public override void OnInit()
    {
        _mReqFrameInputDataPool = new ObjectPool<ReqFrameInputData>(() => new ReqFrameInputData());
        _mVector2DPool = new ObjectPool<Vector2D>(() => new Vector2D());
        GameInterface.Interface.UdpListener.OnReceiveFrameSync += ServerFrameSyncDataUpdate;
        base.OnInit();
    }

    public override void OnDestroy()
    {
        GameInterface.Interface.UdpListener.OnReceiveFrameSync -= ServerFrameSyncDataUpdate;
        base.OnDestroy();
    }

    private void ServerFrameSyncDataUpdate(ResFrameSyncData resFrameSyncData)
    {
        ReqFrameInputData reqFrameInputData = _mReqFrameInputDataPool.Allocate();
        var position = _mVector2DPool.Allocate();
        var moveVector = _mVector2DPool.Allocate();
        try
        {
            if (resFrameSyncData.FrameId != -1)
            {
                // 缓存上一帧
                _mHistoryFrameSyncData[resFrameSyncData.FrameId] = resFrameSyncData;

                // 同步这一帧
                List<FrameInputData> frameInputDataList = resFrameSyncData.PlayersFrameInputData.ToList();

                List<FrameInputData> currentFrameInputDataList = frameInputDataList.FindAll(item =>
                    item.FrameId == _mSyncedFrameId);
                OnFrameSync?.Invoke(currentFrameInputDataList);
                resFrameSyncData.PlayersFrameInputData.Clear();
            }

            _mSyncedFrameId = resFrameSyncData.FrameId;

            // 上传下一帧
            int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;

            resFrameSyncData.FrameId = _mSyncedFrameId;
            _mLocalPlayerInputEvent.playerInputType = GameInput.Instance.LocalPlayerInputType;
            
            reqFrameInputData.FrameId = _mSyncedFrameId;
            reqFrameInputData.InputType = Enum
                .Parse<GameFrameSync.InputType>(_mLocalPlayerInputEvent.playerInputType.ToString());
            reqFrameInputData.PlayerId = localPlayerId;

            Entity localEntity = _mEntities.Find(item => item.playerType is Entity.PlayerType.Local);
            if (localEntity != null)
            {
                position.X = MathUtil.GetFloat(localEntity.localPlayerPosition.x);
                position.Y = MathUtil.GetFloat(localEntity.localPlayerPosition.z);

                moveVector.X = MathUtil.GetFloat(localEntity.localMoveVector.x);
                moveVector.Y = MathUtil.GetFloat(localEntity.localMoveVector.y);
                
                reqFrameInputData.Position = position;
                reqFrameInputData.MoveVector = moveVector;
                reqFrameInputData.InteractCounter = localEntity.localInteractCounterId;
            }
            
            if (GameInterface.Interface.RoomManager.JoinedRoom)
            {
                string roomCode = GameInterface.Interface.RoomManager.CurrentRoomInfo.roomCode;
                resFrameSyncData.RoomCode = roomCode;
            }

            resFrameSyncData.ReqFrameInputData = reqFrameInputData;

            GameInterface.Interface.UdpListener.Send(resFrameSyncData);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            _mReqFrameInputDataPool.Release(reqFrameInputData);
            _mVector2DPool.Release(position);
            _mVector2DPool.Release(moveVector);
        }
    }

    public void AddEntity(in Entity entity)
    {
        _mEntities.Add(entity);
    }
}