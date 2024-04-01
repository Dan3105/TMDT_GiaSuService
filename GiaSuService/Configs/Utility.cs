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

    }
}
