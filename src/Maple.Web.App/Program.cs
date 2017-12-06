using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Maple.Web.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //注册EncodingProvider实现对中文编码的支持
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
