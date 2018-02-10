using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vgic.Report.Web.Controllers
{
    public class OperationStandardController : BaseController
    {
        //
        // GET: /OperationStandard/
        public ActionResult Sales(int chapter=0)
        {
            if (chapter > 0)
            {
                return View("SalesChapter" + chapter);
            }
            return View();
        }

        //
        // GET: /OperationStandard/
        public ActionResult Aftersales(int chapter = 0)
        {
            if (chapter > 0)
            {
                return View("AfterSalesChapter" + chapter);
            }
            return View();
        }
	}
}