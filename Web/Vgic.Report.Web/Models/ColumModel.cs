using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vgic.Report.Web.Models
{
    public class ColumModel
    {
        public bool hidden { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public string align { get; set; }
        public int width { get; set; }
    }
}