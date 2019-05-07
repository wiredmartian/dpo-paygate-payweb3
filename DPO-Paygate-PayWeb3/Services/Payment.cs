using DPO_Paygate_PayWeb3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DPO_Paygate_PayWeb3.Services
{
    public class Payment : IPayment
    {
        private ApplicationDbContext _db;

        public Payment()
        {
            _db = new ApplicationDbContext();
        }

        #region Utilities
        /** Encode dictionary to Url string */
        public string ToUrlEncodedString(Dictionary<string, string> request)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string key in request.Keys)
            {
                builder.Append("&");
                builder.Append(key);
                builder.Append("=");
                builder.Append(HttpUtility.UrlEncode(request[key]));
            }

            string result = builder.ToString().TrimStart('&');

            return result;
        }

        /** Convert query string to dictionary */
        public Dictionary<string, string> ToDictionary(string response)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            string[] valuePairs = response.Split('&');
            foreach (string valuePair in valuePairs)
            {
                string[] values = valuePair.Split('=');
                result.Add(values[0], HttpUtility.UrlDecode(values[1]));
            }

            return result;
        }
        #endregion Utility

        #region Transactions 
        public bool AddTransaction(Dictionary<string, string> request, string payRequestId)
        {
            try
            {
                Transaction transaction = new Transaction
                {
                    DATE = DateTime.Now,
                    PAY_REQUEST_ID = payRequestId,
                    REFERENCE = request["REFERENCE"],
                    AMOUNT = int.Parse(request["AMOUNT"]),
                    CUSTOMER_EMAIL_ADDRESS = request["EMAIL"]
                };
                _db.Transactions.Add(transaction);
                _db.SaveChanges();
                return true;
            } catch (Exception e)
            {
                // log somewhere
                // at least we tried
                return false;
            }
            
        }
        // get transaction using pay request Id
        public Transaction GetTransaction(string payRequestId)
        {
            Transaction transaction = _db.Transactions.FirstOrDefault(p => p.PAY_REQUEST_ID == payRequestId);
            if (transaction == null)
            {
                return new Transaction();
            }

            return transaction;
        }

        public bool UpdateTransaction(Dictionary<string, string> request, string PayrequestId)
        {
            bool IsUpdated = false;

            Transaction transaction = GetTransaction(PayrequestId);
            if (transaction == null)
                return IsUpdated;

            transaction.TRANSACTION_STATUS = request["TRANSACTION_STATUS"];
            transaction.RESULT_DESC = request["RESULT_DESC"];
            transaction.RESULT_CODE = (ResultCodes)int.Parse(request["RESULT_CODE"]);
            try
            {
                _db.SaveChanges();
                IsUpdated = true;
            } catch (Exception e)
            {
                // Oh well, log it
            }
            return IsUpdated;
        }

        /** Get the authenticated user */
        public ApplicationUser GetAuthenticatedUser()
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return new ApplicationUser(); // empty user

            ApplicationUser user = new ApplicationUser();
            string email = HttpContext.Current.User.Identity.Name;
            user = _db.Users.FirstOrDefault(x => x.Email.Equals(email));
            return user;
        }
        #endregion Transaction

        #region MD5 Hashing
        // Adapted from
        // https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5(v=vs.110).aspx

        public string GetMd5Hash(Dictionary<string, string> data, string encryptionKey)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                StringBuilder input = new StringBuilder();
                foreach (string value in data.Values)
                {
                    input.Append(value);
                }

                input.Append(encryptionKey);

                byte[] hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input.ToString()));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                {
                    sBuilder.Append(hash[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        public bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash)
        {
            Dictionary<string, string> hashDict = new Dictionary<string, string>();

            foreach (string key in data.Keys)
            {
                if (key != "CHECKSUM")
                {
                    hashDict.Add(key, data[key]);
                }
            }

            string hashOfInput = GetMd5Hash(hashDict, encryptionKey);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion MD5 Hash

    }
}