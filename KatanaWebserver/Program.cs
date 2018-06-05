using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace KatanaWebserver
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:6678";
            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Started!");
                Console.ReadKey();
                Console.WriteLine("Stopping!");
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            /*  A - using app.Run*/
            //app.Run(environment => 
            //{
            //    return environment.Response.WriteAsync("Hello app.Run");
            //});

            /* B 
             1 - Register all components with appBuilder
             2 - Katana looks for Invoke method (it matches AppFunc signature) via Reflection
             3 - Instantiates the component and gives the next component in the pipeline
             4 - Starts using it to process requests.**/
            //app.Use<HelloWorldComponent>();

            /*C - using AppBuilderExtension*/
            app.UseHelloWorld();

        }
    }

    public static class AppBuilderExtensions
    {
        public static void UseHelloWorld(this IAppBuilder app)
        {
            app.Use<HelloWorldComponent>();
        }
    }

    public class HelloWorldComponent
    {
        public HelloWorldComponent(AppFunc next)
        {
            nextComponent = next;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            //either return a task or throw an exception
            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                return writer.WriteLineAsync("Hello AppBuilder Extension!!");
            }
        }

        private readonly AppFunc nextComponent;
    }
}
