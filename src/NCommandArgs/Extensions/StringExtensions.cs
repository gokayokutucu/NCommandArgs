using System.Runtime.CompilerServices;

namespace NCommandArgs
{
    public static class StringExtensions
    {
        public static string FillColumnSpace(this string str, int longestWord)
        {
            string whiteSpaces = "  ";//double white space

            if (str.Length > longestWord)
                return whiteSpaces;

            for (int i = 0; i < longestWord - str.Length; i++)
            {
                whiteSpaces += " "; //single white space
            }
            return whiteSpaces;
        }
    }
}