using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Forum.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Forum.Services;

namespace Forum
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


            services.AddDbContext<ForumDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddIdentity<AppUser, IdentityRole>(options => {
                //Password
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                //options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                //Email
                options.User.RequireUniqueEmail = true;


            })
                .AddEntityFrameworkStores<ForumDbContext>()
                .AddDefaultTokenProviders();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["JwtIssuer"],
                    ValidAudience = Configuration["JwtIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                
                cfg.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                           // context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };

            });


            services.AddSingleton<IDatabaseCache>(sp =>
            (IDatabaseCache)new DataBaseCache(Convert.ToInt32(Configuration["PostsInCache"])));
            services.AddScoped<ITokenFactory>(sp =>
            (ITokenFactory)new TokenGenerator(Configuration["JwtKey"], Convert.ToDouble(Configuration["JwtExpireDays"]),
            Configuration["JwtIssuer"], Convert.ToDouble(Configuration["RefreshExpireDays"])));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        context.HttpContext.Response.StatusCode = 400;
                        var xD = context.HttpContext.Response.Body;
                        return JsonFormatter.ValidationProblemResponse(context.ModelState
                            );
                    };
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ForumDbContext DbContext, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //DbContext.Database.EnsureDeleted(); // DO NOT TRY THIS AT HOME
                DbContext.Database.EnsureCreated();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            ConfigureRolesAsync(serviceProvider).Wait();
            (serviceProvider.GetRequiredService<IDatabaseCache>() as DataBaseCache).InitForumThreads(DbContext);
            (serviceProvider.GetRequiredService<IDatabaseCache>() as DataBaseCache).RefreshSubForums(DbContext);
        }
        /// <summary>
        /// Configure Roles For application and create Superuser based on configuration file
        /// </summary>
        private async Task ConfigureRolesAsync(IServiceProvider serviceProvider)
        {
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            //Create Admin
            if (!(await _roleManager.RoleExistsAsync("Admin")))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            //Create normal User
            if (!(await _roleManager.RoleExistsAsync("NormalUser")))
            {
                await _roleManager.CreateAsync(new IdentityRole("NormalUser"));
            }

            //create super user
            var superUser = await _userManager.FindByEmailAsync(Configuration["AdminEmail"]);
            if (superUser == null)
            {
                var result = await _userManager.CreateAsync(new AppUser()
                {
                    UserName = Configuration["AdminName"],
                    Email = Configuration["AdminEmail"]
                }, Configuration["AdminPass"]);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync("super@user.wp"), "Admin");
                }
            }
        }
    }
}
