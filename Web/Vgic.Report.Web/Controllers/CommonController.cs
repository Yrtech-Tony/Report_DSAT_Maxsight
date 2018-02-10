using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.DSATReportServer;

namespace Vgic.Report.Web.Controllers
{
    public class CommonController : BaseController
    {
        /// <summary>
        /// 根据区域、集团获得经销商
        /// </summary>
        /// <param name="nSort">1-区域， 2-集团</param>
        /// <param name="sValue">指定区域、集团的值</param>
        /// <returns></returns>
        public ActionResult SearchShop(string nSort, string sValue)
        {
            List<ShopDto> shopList;
            switch (nSort)
            {
                case "1":
                    shopList = _client.DSATReport_SearchShopByAreaCode(sValue).ToList();
                    return Json(shopList, JsonRequestBehavior.AllowGet);
                case "2":
                    shopList = _client.DSATReport_SearchShopByGroup(sValue).ToList();
                    return Json(shopList, JsonRequestBehavior.AllowGet);
                default:
                    return HttpNotFound();
            }
        }
    }
}