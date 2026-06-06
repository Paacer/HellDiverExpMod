namespace Assets.Scripts.Components
{
    public enum StrategyState
    {
        Normal, // 正常状态，未激活
        Activating, // 激活中
        InBound, // 到达中
        Impact, // 命中
        CoolDown // 冷却
    }
}
