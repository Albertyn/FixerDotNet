using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FixerDotNetCore.Components;
using FixerDotNetCore.Components.Repositories;
using FixerDotNetCore.Domain.Models;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace FixerDotNetCore
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; private set; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }


        public IContainer ApplicationContainer { get; private set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the Configure method, below.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        //public void ConfigureServices(IServiceCollection services)
        {

            services.AddOptions();
            services.Configure<FixerSettings>(Configuration.GetSection("Fixer"));
            //services.Configure<MongoConfiguration>(Configuration.GetSection("MongoDB"));

            // Add framework services.
            services.AddMvc();
            
            // Create the container builder.
            var builder = new ContainerBuilder();
            // Register dependencies, populate the services from the collection, and build the container. 
            // If you want to dispose of the container at the end of the app, be sure to keep a reference to it as a property or field.

            string connectionString = Configuration.GetSection("MongoDB:ConnectionString").Value;
            builder.RegisterInstance(new MongoConfiguration(connectionString))
            .As<IMongoConfiguration>();

            builder.RegisterType<Repository<Fixer>>().As<IRepository<Fixer>>();
            builder.RegisterType<FixerRepository>().As<IFixerRepository>();
            builder.RegisterType<FixerComponent>().As<IFixerComponent>();
            builder.Populate(services);

            this.ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Configure is where you add middleware. This is called after ConfigureServices.
        // You can use IApplicationBuilder.ApplicationServices here if you need to resolve things from the container.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, 
            IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            if (env.IsDevelopment()) app.UseDeveloperExceptionPage(); else app.UseExceptionHandler("/Error"); 

            app.UseStaticFiles();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Default}/{action=Index}");
            });

            // If you want to dispose of resources that have been resolved in the application container, register for the "ApplicationStopped" event.
            appLifetime.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
        }
    }
}
