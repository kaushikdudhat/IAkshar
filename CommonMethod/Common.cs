using iAkshar.Dto;
using iAkshar.Models;

namespace iAkshar.Common
{
    public static class Common
    {
        public static string GetYuvakType(AksharUser user)
        {
            string type = "";
            if (user.Iskaryakarta == true)
            {
                type = "Karyakarta";

            }
            if (user.IsAmbrish == true)
            {
                type = type + " " + "Ambrish";
            }
            return type;
        }

        public static Response GenerateResponse(object Data, bool isSucc, string Message = "")
        {
            return new Response
            {
                Data = Data,
                Message = Message,
                IsSuccess = isSucc
            };
        }

        public static Response GenerateSuccResponse(object Data, string Message = "")
        {
            return new Response
            {
                Data = Data,
                Message = Message,
                IsSuccess = true
            };
        }

        public static Response GenerateError(string Message=null)
        {
            return new Response
            {
                Data = null,
                Message = Message??"Not Found",
                IsSuccess = false
            };
        }
    }
}
