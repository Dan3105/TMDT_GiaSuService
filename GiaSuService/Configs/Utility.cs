using GiaSuService.Models;
using Newtonsoft.Json;

namespace GiaSuService.Configs
{
    public static class Utility
    {
        //Function can return null
        public static UserAuthenticationModel GetUserSession(HttpContext context) {
            var userJson = context.Session.GetString(AppConfig.SESSION_USER);
            if (userJson is not null && userJson.Length > 0)
            {
                try
                {
                    UserAuthenticationModel userRetrival = JsonConvert.DeserializeObject<UserAuthenticationModel>(userJson)!;
                    return userRetrival;
                }
                catch {
                    return null!;
                }
            }
            return null!;
        }
    }
}
