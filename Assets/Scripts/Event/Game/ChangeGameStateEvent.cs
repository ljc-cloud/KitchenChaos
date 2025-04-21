
/// <summary>
/// 切换当前游戏状态事件（服务端响应）
/// </summary>
public struct ChangeGameStateEvent : IEvent
{
    public KitchenGameManager.State newState;
}