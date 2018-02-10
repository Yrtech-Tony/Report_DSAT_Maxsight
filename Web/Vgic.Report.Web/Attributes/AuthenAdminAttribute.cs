using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Vgic.Report.Web.DSATReportServer;

namespace VgicReport.Filter
{
    public class AuthenAdminAttribute : FilterAttribute, IAuthenticationFilter
    {
        protected ServiceSoapClient _client1;
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //这个方法是在Action执行之前调用
            var user = filterContext.HttpContext.Session["LoginUser"];
            if (user == null && !filterContext.HttpContext.Request.Url.AbsolutePath.Contains("Login"))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var acceptTypes = filterContext.HttpContext.Request.AcceptTypes;
                    if (acceptTypes.Contains("*/*") || acceptTypes.Contains("application/json"))
                    {
                        filterContext.Result = new JsonResult
                        {
                            Data = new
                            {
                                Status = 401,
                                Message = "用户登陆过期，请重新登陆！",
                                ReturnUrl = filterContext.HttpContext.Request.Url.PathAndQuery
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                }
                else
                {
                    var Url = new UrlHelper(filterContext.RequestContext);
                    var url = Url.Action("Login", "Account", new { ReturnUrl = filterContext.HttpContext.Request.Url.PathAndQuery });
                    filterContext.Result = new RedirectResult(url);
                }
            }
            

        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //这个方法是在Action执行之后调用
        }
    }
}