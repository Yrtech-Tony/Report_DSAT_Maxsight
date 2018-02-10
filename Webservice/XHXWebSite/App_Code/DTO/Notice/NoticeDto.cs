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
/// Summary description for NoticeDto
/// </summary>
public class NoticeDto
{
    public NoticeDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int NoticeId { get; set; }
    public string NoticeContent { get; set; }
    public string InUserId { get; set; }
    public string InDateTime { get; set; }
}
