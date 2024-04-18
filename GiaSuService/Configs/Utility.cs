using GiaSuService.EntityModel;
using GiaSuService.Models;
using GiaSuService.Models.UtilityViewModel;
using Newtonsoft.Json;

namespace GiaSuService.Configs
{
    public static class Utility
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static string FormatToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Split the input string into words and capitalize the first letter of each word
            string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }

            // Join the words into a single string
            return string.Join(" ", words);
        }
    }
}
