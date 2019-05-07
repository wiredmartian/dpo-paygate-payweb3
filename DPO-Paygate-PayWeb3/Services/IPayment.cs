using DPO_Paygate_PayWeb3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DPO_Paygate_PayWeb3.Services
{
    public interface IPayment
    {
        string ToUrlEncodedString(Dictionary<string, string> request);
        Dictionary<string, string> ToDictionary(string response);
        bool AddTransaction(Dictionary<string, string> request, string payRequestId);
        void AddTransactionReponseMeta(Dictionary<string, string> request, string PayrequestId);
        Transaction GetTransaction(string payRequestId);
        string GetMd5Hash(Dictionary<string, string> data, string encryptionKey);
        bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash);
        MailAddress GetAuthenticatedUser();
        void UpdateTransactionStatus(Transaction transaction);
    }
}
