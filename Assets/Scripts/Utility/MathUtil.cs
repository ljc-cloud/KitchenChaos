
public static class MathUtil
{
    private const int MULTIPLIER = 1000;

    /// <summary>
    /// 获取浮点数（精度为小数点后三位）
    /// </summary>
    /// <param name="v"></param>
    public static float GetFloat(float v)
    {
        int a = (int)(v * MULTIPLIER);
        float result = (float)a / MULTIPLIER;
        return result;
    }
}