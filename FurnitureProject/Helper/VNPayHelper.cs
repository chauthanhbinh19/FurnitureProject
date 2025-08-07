using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FurnitureProject.Helper
{
    public class VnPayHelper
    {
        public static string CreateRequestUrl(HttpContext httpContext, IConfiguration config, string orderReference, long amount, string orderInfo)
        {
            var vnpParams = new SortedDictionary<string, string>
            {
                ["vnp_Version"] = config["VnPay:Version"],
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = config["VnPay:TmnCode"],
                ["vnp_Amount"] = (amount * 100).ToString(),
                ["vnp_BankCode"] = "VNPAYQR",
                ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"] = "VND",
                ["vnp_IpAddr"] = GetIpAddress(httpContext),
                ["vnp_Locale"] = "vn",
                ["vnp_OrderInfo"] = orderInfo,
                ["vnp_ReturnUrl"] = config["VnPay:ReturnUrl"],
                ["vnp_ExpireDate"] = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"),
                ["vnp_TxnRef"] = orderReference
            };

            // Tạo chuỗi dữ liệu gốc chưa encode
            string rawData = string.Join("&", vnpParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            // Tạo chuỗi query đã encode
            string query = string.Join("&", vnpParams.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            string secretKey = config["VnPay:HashSecret"];
            string secureHash = ComputeHmacSHA256(rawData, secretKey);

            return $"{config["VnPay:PaymentUrl"]}?{query}&vnp_SecureHash={secureHash}";
        }

        public static string ComputeHmacSHA256(string data, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA256(keyBytes);
            byte[] hashBytes = hmac.ComputeHash(dataBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }


        public static string GetIpAddress(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            }
            return ip;
        }

    }

}
