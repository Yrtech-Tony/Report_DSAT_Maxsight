using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.DSATReportServer;

namespace Vgic.Report.Web.Attributes
{
    public class LogFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string urlAbsolutePath = filterContext.HttpContext.Request.Url.AbsolutePath;
            HttpRequestBase Request = filterContext.HttpContext.Request;
            ServiceSoapClient client = new ServiceSoapClient();
            if (urlAbsolutePath.Contains("Login") && filterContext.HttpContext.Request.HttpMethod == "POST")
            {//POST 登陆方法
                string userName = filterContext.HttpContext.Request.Form["sUsername"];
                client.DSATReport_SystemLog("Account", "Login", "", userName);
            }

            var user = (UserInfoDto)filterContext.HttpContext.Session["LoginUser"];
            if (user == null) return;

            if (urlAbsolutePath.Contains("DownloadReport"))
            {//下载文件
                client.DSATReport_SystemLog("Person", "DownloadReport", "", user.UserId);
            }

            if (urlAbsolutePath.Contains("DownloadFiles"))
            {
                //打包下载报告
                string type = Request.QueryString["type"];
                object controller = filterContext.RouteData.Values["controller"];
                object action = filterContext.RouteData.Values["action"];
                if(controller != null)
                {
                    client.DSATReport_SystemLog(controller.ToString(), "DownloadFiles", type, user.UserId);
                }
            }
            else if (urlAbsolutePath.Contains("DownloadFile"))
            {//下载报告
                string file = Request.QueryString["file"];                
                if (file != null)
                {
                    string[] routes = file.Split('/');
                    if (routes.Length > 3)
                    {
                        client.DSATReport_SystemLog(routes[2], "DownloadFile", routes[3], user.UserId);
                    }
                }
            }
        }
    }
}