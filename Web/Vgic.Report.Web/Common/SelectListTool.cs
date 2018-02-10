using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.DSATReportServer;

namespace Vgic.Report.Web.Common
{
    public class SelectListTool
    {
        private static SelectList _years;
        public static SelectList Years
        {
            get
            {
                if (_years == null)
                {
                    List<int> years = new List<int>();
                    for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 10; i--)
                    {
                        years.Add(i);
                    }
                    _years = new SelectList(years);
                }
                return _years;
            }
        }
        private static SelectList _months;
        public static SelectList Months
        {
            get
            {
                if (_months == null)
                {
                    List<int> months = new List<int>();
                    for (int i = 1; i <= 12; i++)
                    {
                        months.Add(i);
                    }
                    _months = new SelectList(months);
                }
                return _months;
            }
        }

        private static SelectList _acScores;
        public static SelectList ACScores
        {
            get
            {
                if (_acScores == null)
                {
                    ServiceSoapClient client = new ServiceSoapClient();
                    _acScores = new SelectList(client.DSATReport_SearchACStatus(), "Code", "Name");
                }
                return _acScores;
            }
        }

        private static SelectList _positions;
        public static SelectList Positions
        {
            get
            {
                if (_positions == null)
                {
                    ServiceSoapClient client = new ServiceSoapClient();
                    _positions = new SelectList(client.DSATReport_SearchPositionByAc(), "Code", "Name");
                }
                return _positions;
            }
        }
    }
}