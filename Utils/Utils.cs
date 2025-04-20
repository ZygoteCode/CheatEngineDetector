public class Utils
{
    public static string FilterString(string str)
    {
        char[] characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        string result = "";

        foreach (char c in str)
        {
            foreach (char s in characters)
            {
                if (c.Equals(s))
                {
                    result += s;
                    break;
                }
            }
        }

        return result.ToLower();
    }

    public static string GetPathFromFileName(string str)
    {
        string newPath = str.ToLower();
        string fileName = System.IO.Path.GetFileName(newPath);
        return newPath.Substring(0, newPath.Length - fileName.Length - 1);
    }
}