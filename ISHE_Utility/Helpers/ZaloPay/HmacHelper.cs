using System.Security.Cryptography;

namespace ISHE_Utility.Helpers.ZaloPay
{
    public enum ZaloPayHMAC
    {
        HMACMD5,
        HMACSHA1,
        HMACSHA256,
        HMACSHA512
    }

    public class HmacHelper
    {
        public static string Compute(ZaloPayHMAC algorithm = ZaloPayHMAC.HMACSHA256, string key = "", string message = "")
        {
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);

            HMAC hmac = algorithm switch
            {
                ZaloPayHMAC.HMACMD5 => new HMACMD5(keyByte),
                ZaloPayHMAC.HMACSHA1 => new HMACSHA1(keyByte),
                ZaloPayHMAC.HMACSHA256 => new HMACSHA256(keyByte),
                ZaloPayHMAC.HMACSHA512 => new HMACSHA512(keyByte),
                _ => new HMACSHA256(keyByte)
            };

            byte[] hashMessage = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
        }
    }
}
