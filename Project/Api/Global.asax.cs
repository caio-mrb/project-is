using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Optimization;
using Api.Messaging;

namespace Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static MqttPublisher MqttPublisher{ get; private set; }
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MqttPublisher = new MqttPublisher("127.0.0.1");
            MqttPublisher.Connect();
        }

        protected void Application_End()
        {
            if (MqttPublisher != null)
            {
                MqttPublisher.Disconnect();
            }
        }
    }
}
