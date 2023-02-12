using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FileManagerClassLibrary.Services.CosmoDbService;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace FileManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAdB2C"));
            //services.AddControllersWithViews();
            //services.AddRazorPages()
            //     .AddMicrosoftIdentityUI();

            services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAdB2C");

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();
            services.AddRazorPages();
            services.AddTransient<ICosmosDbService>(x => InitializaCosmoClientInstanceAsync(Configuration).GetAwaiter().GetResult());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(options => {
                options.AllowAnyOrigin();
                options.AllowAnyMethod();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        private static async Task<CosmosDbService> InitializaCosmoClientInstanceAsync(IConfiguration configuration)
        {
            IConfigurationSection configurationSection = configuration.GetSection("CosmoDb");
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string Key = configurationSection.GetSection("Key").Value;

            Microsoft.Azure.Cosmos.Fluent.CosmosClientBuilder clientBuilder = new Microsoft.Azure.Cosmos.Fluent.CosmosClientBuilder(account, Key);
            Microsoft.Azure.Cosmos.CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            CosmosDbService cosmoDbService = new CosmosDbService(client, databaseName, containerName);
            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmoDbService;

        }
    }
}
