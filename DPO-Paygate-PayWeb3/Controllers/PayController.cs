using DPO_Paygate_PayWeb3.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DPO_Paygate_PayWeb3.Controllers
{
    public class PayController : Controller
    {
        private IPayment _payment = new Payment();

        // Get Paygate keys from webconfig
        readonly string PayGateID = ConfigurationManager.AppSettings["PAYGATEID"];
        readonly string PayGateKey = ConfigurationManager.AppSettings["PAYGATEKEY"];

        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetResult()
        {
            HttpClient http = new HttpClient();
            Dictionary<string, string> request = new Dictionary<string, string>();
            string paymentAmount = (50 * 100).ToString("00"); // amount int cents e.i 50 rands is 5000 cents

            request.Add("PAYGATE_ID", PayGateID);
            request.Add("REFERENCE", "#45846"); // Payment ref e.g ORDER NUMBER
            request.Add("AMOUNT", paymentAmount);
            request.Add("CURRENCY", "ZAR"); // South Africa
            request.Add("RETURN_URL", $"{Request.Url.Scheme}://{Request.Url.Authority}/paygate/postpayment");
            request.Add("TRANSACTION_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            request.Add("LOCALE", "en-za");
            request.Add("COUNTRY", "ZAF");

            // get authenticated user's email
            // use a valid email, paygate will send a transaction confirmation to it
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                request.Add("EMAIL", _payment.GetAuthenticatedUser().Email);
            } else
            {
                request.Add("EMAIL", "solomzi.jikani@gmail.com");
            }
            request.Add("CHECKSUM", _payment.GetMd5Hash(request, PayGateKey));

            string requestString = _payment.ToUrlEncodedString(request);
            StringContent content = new StringContent(requestString, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await http.PostAsync("https://secure.paygate.co.za/payweb3/initiate.trans", content);
            // if the request did not succeed, this line will make the program crash
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            Dictionary<string, string> results = _payment.ToDictionary(responseContent);

            if (results.Keys.Contains("ERROR"))
            {
                return Json(new
                {
                    success = false,
                    message = "An error occured while initiating your request"
                }, JsonRequestBehavior.AllowGet);
            }

            if (!_payment.VerifyMd5Hash(results, PayGateKey, results["CHECKSUM"]))
            {
                return Json(new
                {
                    success = false,
                    message = "MD5 verification failed"
                }, JsonRequestBehavior.AllowGet);
            }

            bool IsRecorded = _payment.AddTransaction(request, results["PAY_REQUEST_ID"]);
            if (IsRecorded)
            {
                return Json(new
                {
                    success = true,
                    message = "Request completed successfully",
                    results
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                success = false,
                message = "Failed to record a transaction"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}