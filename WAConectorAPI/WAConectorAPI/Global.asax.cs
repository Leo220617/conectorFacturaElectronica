﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace WAConectorAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public override void Init() { this.PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest; base.Init(); }
        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e) { System.Web.HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required); }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
