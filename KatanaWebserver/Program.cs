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
            /*Dumps Environment KeyValue Pairs*/
            //app.Use(async (environment, next) =>
            //{
            //    foreach (var pair in environment.Environment)
            //    {
            //        Console.WriteLine("{0} : {1}", pair.Key, pair.Value);
            //    }

            //    await next();
            //});


            /****
             * Dictionary<T1, T2> vs List<KeyValuePair<<T1, T2>>
             * -------------------------------------------------
             * Dictionary = :) fast look up
             *            = :) Ensure unique keys
             *            = :( Hashing adds overhead
             *            
             * List = :) allows duplicate keys
             *      = :) list insertion faster
             *      = :) can be sorted
             ****/


            app.Use(async (environment, next) =>
            {
                // add stuff to the request.
                // many web browsers send second request along the first time to get the favicon.ico
                Console.WriteLine($"Requesting Path :  {environment.Request.Path}");

                await next();

                // Can use this to inject info into the response
                Console.WriteLine($"Response Status Code :  {environment.Response.StatusCode}");
            });


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
            Console.WriteLine("Preparing the response");
            //either return a task or throw an exception
            
            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                return writer.WriteLineAsync("Middleware Injection = This shit it SO fkn cool.");
            }
        }

        private readonly AppFunc nextComponent;
    }
}
