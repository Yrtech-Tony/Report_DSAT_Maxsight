using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.Attributes;

namespace Vgic.Report.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogFilter());
        }
    }
}
