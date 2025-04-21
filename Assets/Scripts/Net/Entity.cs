using System;
using System.Collections.Generic;
using GameFrameSync;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public enum PlayerType
    {
        Local,
        Remote
    }
    /// <summary>
    /// 玩家类型（本地、远程）
    /// </summary>
    public PlayerType playerType;
    /// <summary>
    /// 玩家id
    /// </summary>
    public int playerId;
    /// <summary>
    /// 玩家输入
    /// </summary>
    public GameFrameSyncManager.PlayerInputType playerInputType;
    /// <summary>
    /// 当前位置（远程）
    /// </summary>
    public Vector3 playerPosition;
    /// <summary>
    /// 当前位置（本地）
    /// </summary>
    public Vector3 localPlayerPosition;
    /// <summary>
    /// 移动向量(远程)
    /// </summary>
    public Vector2 moveVector;
    /// <summary>
    /// 移动向量(本地)
    /// </summary>
    public Vector2 localMoveVector;

    public bool IsLocal => playerType is PlayerType.Local;
    
    public class OnPlayerInputChangedEventArgs : EventArgs
    {
        public int playerId;
        public GameFrameSyncManager.PlayerInputType inputType;
    }

    public event EventHandler<OnPlayerInputChangedEventArgs> OnPlayerInputChanged;

    private void Start()
    {
        GameInterface.Interface.GameFrameSyncManager.OnFrameSync += OnFrameSync;
    }

    private void Update()
    {
        if (IsLocal)
        {
            localPlayerPosition = transform.position;
            localMoveVector = GameInput.Instance.MoveVector;
        }
    }

    private void OnDestroy()
    {
        GameInterface.Interface.GameFrameSyncManager.OnFrameSync -= OnFrameSync;
    }

    private void OnFrameSync(List<FrameInputData> frameDataList)
    {
        FrameInputData frameInputData = frameDataList?.Find(item => item.PlayerId == playerId);
        if (frameInputData != null)
        {
            GameFrameSyncManager.PlayerInputType inputType = 
                Enum.Parse<GameFrameSyncManager.PlayerInputType>(frameInputData.InputType.ToString());

            if (inputType != playerInputType) 
            {
                OnPlayerInputChanged?.Invoke(this, new OnPlayerInputChangedEventArgs
                    { playerId = playerId, inputType = inputType });   
            }
            
            if (frameInputData.Position != null)
            {
                playerPosition = new Vector3(frameInputData.Position.X, 0,
                    frameInputData.Position.Y);
            }

            if (frameInputData.MoveVector != null)
            {
                moveVector = new Vector2(frameInputData.MoveVector.X, frameInputData.MoveVector.Y);
            }
            Debug.Log($"player:{playerId}收到同步消息, {frameInputData.InputType}, moveVector:{moveVector}," +
                      $"playerPosition:{playerPosition}");
        }
    }
}