using System.Text;

public static class CharsetUtil
{
    public static string DefaultToUTF8(string s)
    {
        return Encoding.UTF8.GetString(Encoding.Default.GetBytes(s));
    }
}
