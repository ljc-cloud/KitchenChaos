
/// <summary>
/// 游戏状态被切换事件
/// </summary>
public struct GameStateChangedEvent : IEvent
{
    public KitchenGameManager.State state;
}