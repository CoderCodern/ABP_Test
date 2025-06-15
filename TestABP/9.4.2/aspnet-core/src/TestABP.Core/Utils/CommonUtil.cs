using System.Text.RegularExpressions;

namespace TestABP.Utils
{
    public class CommonUtil
    {
        public static string GetNaturalSortKey(string value)
        {
            return Regex.Replace(value, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }
    }
}
