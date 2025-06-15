using System.Text.RegularExpressions;

namespace YK.Socket.Extensions
{
    public static class ArrayHelpers
    {
        /// <summary>
        /// Is valid hex value test.
        /// </summary>
        /// <param name="input">Hex string.</param>
        /// <returns>Whether the hex is valid or not.</returns>
        public static bool IsValidHex(this string input)
        {
            input = input.Replace(" ", string.Empty);
            if (input.Length % 2 != 0)
            {
                return false;
            }

            return Regex.IsMatch(input, "^[0-9a-fA-F]*$");
        }

        public static byte[] HexToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                return [];
            }   

            hex = hex.Replace(" ", string.Empty);

            return [.. Enumerable.Range(0, hex.Length).
                Where(x => x % 2 == 0).
                Select(x => Convert.ToByte(hex.Substring(x, 2), 16))];
        }
    }
}
