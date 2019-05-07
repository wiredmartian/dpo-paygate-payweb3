using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DPO_Paygate_PayWeb3.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TRANSACTION_ID { get; set; }
        public DateTime? DATE { get; set; }
        //public string PAYGATE_ID { get; set; }
        public string PAY_REQUEST_ID { get; set; }
        public int AMOUNT { get; set; }
        public string REFERENCE { get; set; }
        public string TRANSACTION_STATUS { get; set; }
        public ResultCodes RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string CUSTOMER_EMAIL_ADDRESS { get; set; }
    }

    public enum ResultCodes
    {
        Call_for_Approval = 900001,
        Card_Expired = 900002,
        Insufficient_Funds = 900003,
        Invalid_Card_Number = 900004,
        Bank_Interface_Timeout = 900005,
        Invalid_Card = 900006,
        Declined = 900007,
        Lost_Card = 900009,
        Invalid_Card_Length = 900010,
        Suspected_Fraud = 900011,
        Card_Reported_as_Stolen = 900012,
        Restricted_Card = 900013,
        Excessive_Card_Usage = 900014,
        Card_Blacklisted = 900015,
        Declined_Authentication_failed = 900207,
        Auth_Declined = 990020,
        ThreeD_Secure_Lookup_Timeout = 900210,
        Invalid_expiry_date = 991001,
        Invalid_amount = 991002
    }
}