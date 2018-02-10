using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for AreaDto
/// </summary>
public class AreaDto
{
    public AreaDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
}
