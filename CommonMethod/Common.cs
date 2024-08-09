using iAkshar.Models;

namespace iAkshar.Common
{
    public static class Common
    {
        public static string GetYuvakType(AksharUser user)
        {
            string type = "Yuvak";
            if (user.Iskaryakarta==true)
            {
                type = "Karyakarta";

            }
            if (user.IsAmbrish==true)
            {
                type = type + " " + "Ambrish";
            }
            return type;
        }
    }
}
