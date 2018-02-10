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
/// Summary description for GroupDto
/// </summary>
public class GroupDto
{
    public GroupDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string GroupCode { get; set; }
    public string GroupName { get; set; }
    public string UserId { get; set; }
    public string Password { get; set; }
}
