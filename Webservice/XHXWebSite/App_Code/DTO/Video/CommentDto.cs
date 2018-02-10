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
/// Summary description for CommentDto
/// </summary>
public class CommentDto
{
    public CommentDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string FileId { get; set; }
    public string CommentId { get; set; }
    public string CommentContent { get; set; }
    public string InUserId{get;set;}
    public DateTime InDateTime{get;set;}
}
