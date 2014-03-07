﻿using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Hudl.Common.Extensions;
using Hudl.Config;
using Hudl.Mjolnir.Command;

namespace TestMvc40WebApp.Controllers
{
    // This is a 4.0-created webapp that's retargeted at 4.5, except hasn't been given <httpRuntime targetFramework="4.5"/>
    // It has, however, been given <add key="aspnet:UseTaskFriendlySynchronizationContext" value="true"/>, which should improve
    // task behavior.

    public class TestController : Controller
    {
        public async Task<ActionResult> Context()
        {
            ConfigurationUtility.Init();

            // This should not throw a NullReferenceException if the following is in Web.config:
            //   <add key="aspnet:UseTaskFriendlySynchronizationContext" value="true"/>

            var path = System.Web.HttpContext.Current.Request.Path;
            var result = await new TestCommand().InvokeAsync();

            // Without the above property, HttpContext.Current will be null.
            var newPath = System.Web.HttpContext.Current.Request.Path;
            var equalPaths = path == newPath;
            return Content((equalPaths ? "Success" : "Failure") + ": " + newPath);
        }

        private class TestCommand : Command<string>
        {
            public TestCommand() : base("test", "test", 5.Seconds()) {}
            protected override async Task<string> ExecuteAsync(CancellationToken cancellationToken)
            {
                await Task.Run(() => Thread.Sleep(1000));
                return "hello";
            }
        }
    }
}
