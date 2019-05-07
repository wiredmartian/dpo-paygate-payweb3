using System.Web;
using System.Web.Mvc;

namespace DPO_Paygate_PayWeb3
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
