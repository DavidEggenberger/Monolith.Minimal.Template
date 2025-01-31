using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Server.Features.UserIdentity;
using Server.Infrastructure.EFCore;
using Server.Infrastructure.SignalR;

namespace Server
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
            services.AddSignalR();
            services.AddRazorPages();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "",
                        Version = "v1",
                        Description = "Open API",
                        Contact = new OpenApiContact
                        {
                            Name = "David Eggenberger"
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Example License",
                            Url = new Uri("https://example.com/license")
                        }
                    });
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration["SQLConnection"]);
            });

            AuthenticationBuilder authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                options.RequireAuthenticatedSignIn = false;
            })
                .AddMicrosoftAccount(options =>
                {
                    options.ClientId = Configuration["LinkedInClientId"];
                    options.ClientSecret = Configuration["LinkedInClientSecret"];
                    options.Events.OnTicketReceived = async context =>
                    {
                        var kvp = context.Properties.Items[".AuthScheme"];
                        context.Properties.Items.Add("LoginProvider", kvp);
                    };
                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["GoogleClientId"];
                    options.ClientSecret = Configuration["GoogleClientSecret"];
                    options.Events.OnCreatingTicket = (context) =>
                    {
                        var picture = context.User.GetProperty("picture").GetString();
                        context.Identity.AddClaim(new Claim("picture", picture));
                        return Task.CompletedTask;
                    };
                    options.Events.OnTicketReceived = async context =>
                    {
                        var kvp = context.Properties.Items[".AuthScheme"];
                        context.Properties.Items.Add("LoginProvider", kvp);
                    };
                });

            authenticationBuilder.AddIdentityCookies();

            var identityService = services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
                options.User.RequireUniqueEmail = true;
                options.Stores.MaxLengthForKeys = 128;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.InjectStylesheet("/swagerStyles.css");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseBlazorFrameworkFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHub<NotificationHub>("NotificationHub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
