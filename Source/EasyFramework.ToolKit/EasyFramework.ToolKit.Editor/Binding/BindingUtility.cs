using EasyFramework.Core;

namespace EasyFramework.ToolKit.Editor
{
    public static class BindingUtility
    {
        public static bool IsValidIdentifier(string identifier)
        {
            identifier = identifier.Trim();

            if (identifier.IsNullOrEmpty())
                return false;

            if (!char.IsLetter(identifier[0]) && identifier[0] != '_')
            {
                return false;
            }
            if (identifier.Length == 1)
                return true;

            var other = identifier[1..];
            foreach (var ch in other)
            {
                if (!char.IsLetterOrDigit(ch) && ch != '_')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
