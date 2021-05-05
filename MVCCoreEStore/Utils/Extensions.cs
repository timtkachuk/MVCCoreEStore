using System.Linq;
using System.Text.RegularExpressions;

namespace MVCCoreEStore
{
    public static class Extensions
    {
        public static string ToSafeUrl(this string Text) => Regex.Replace(string.Concat(Text.ToLower().Where(p => char.IsLetterOrDigit(p) || p == '-' || char.IsWhiteSpace(p))), @"\s+", "-");

    }
}
