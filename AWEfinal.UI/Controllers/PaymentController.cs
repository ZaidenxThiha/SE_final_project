using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using AWEfinal.BLL.Services;
using AWEfinal.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AWEfinal.UI.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly VnPaySettings _settings;

        public PaymentController(IOrderService orderService, IOptions<VnPaySettings> options)
        {
            _orderService = orderService;
            _settings = options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> VnPayQr(int orderId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                ViewBag.Error = "Please sign in to proceed with payment.";
                return View("VnPayQr");
            }

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.UserId != userId.Value)
            {
                ViewBag.Error = "Order not found or you do not have access to it.";
                return View("VnPayQr");
            }

            if (order.Total <= 0)
            {
                ViewBag.Error = "Order total must be greater than zero to generate a payment request.";
                return View("VnPayQr", order);
            }

            if (string.IsNullOrWhiteSpace(_settings.TmnCode) || string.IsNullOrWhiteSpace(_settings.HashSecret))
            {
                ViewBag.Error = "VNPay is not configured. Please set VNPay:TmnCode and VNPay:HashSecret in appsettings.";
                return View("VnPayQr", order);
            }

            var paymentUrl = BuildPaymentUrl(order);
            if (string.IsNullOrWhiteSpace(paymentUrl))
            {
                ViewBag.Error = "Failed to generate VNPay URL.";
                return View("VnPayQr", order);
            }

            return Redirect(paymentUrl);
        }

        [HttpGet]
        public IActionResult VnPayReturn()
        {
            var responseCode = Request.Query["vnp_ResponseCode"].ToString();
            var txnRef = Request.Query["vnp_TxnRef"].ToString();
            var amount = Request.Query["vnp_Amount"].ToString();
            var message = responseCode == "00" ? "Payment successful" : "Payment was not completed";

            ViewBag.Message = message;
            ViewBag.TxnRef = txnRef;
            ViewBag.Amount = amount;
            return View("VnPayReturn");
        }

        private string BuildPaymentUrl(AWEfinal.DAL.Models.Order order)
        {
            var amount = ((long)(order.Total * 100)).ToString(CultureInfo.InvariantCulture);
            var txnRef = order.OrderNumber.Replace("-", string.Empty);
            txnRef = txnRef.Length > 20 ? txnRef.Substring(0, 20) : txnRef;

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            if (ipAddress == "::1") ipAddress = "127.0.0.1";
            var now = DateTime.UtcNow;

            var parameters = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _settings.TmnCode },
                { "vnp_Amount", amount },
                { "vnp_CreateDate", now.ToString("yyyyMMddHHmmss") },
                { "vnp_ExpireDate", now.AddMinutes(15).ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", ipAddress },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", $"Payment for order {order.OrderNumber}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", string.IsNullOrWhiteSpace(_settings.ReturnUrl) ? BuildReturnUrl() : _settings.ReturnUrl },
                { "vnp_TxnRef", txnRef }
            };

            // Explicitly force VNPay QR flow
            parameters.Add("vnp_BankCode", "VNPAYQR");

            var query = string.Join("&", parameters.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            var secureHash = ComputeHmacSha512(_settings.HashSecret, query);

            return $"{_settings.BaseUrl}?{query}&vnp_SecureHash={secureHash}";
        }

        private string BuildReturnUrl()
        {
            var request = HttpContext.Request;
            var host = $"{request.Scheme}://{request.Host}";
            return $"{host}/Payment/VnPayReturn";
        }

        private static string ComputeHmacSha512(string key, string input)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(input);
            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(inputBytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }
    }
}
