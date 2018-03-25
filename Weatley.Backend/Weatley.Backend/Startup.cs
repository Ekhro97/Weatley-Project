﻿using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weatley.Backend.Core;
using Weatley.DataAccess;
using Weatley.DataAccess.Abstract;
using Weatley.DataAccess.Repositories;
using Weatley.Model.Entities;

namespace Weatley.Backend
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
            services.AddSingleton(Configuration);

            services.AddDbContext<WeatleyContext>(options =>
                options.UseSqlServer(Configuration["Data:WeatleyConnection:ConnectionString"],
                b => b.MigrationsAssembly("Weatley.Backend")));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<WeatleyContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(cfg =>
            {
                cfg.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                            ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

                        return Task.FromResult(0);
                    }
                };
            });

            services.AddScoped<IAccountingRepository, AccountingRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IBookedRoomRepository, BookedRoomRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductOrderedRepository, ProductOrderedRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();

            services.AddCors();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
               builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());

            app.UseExceptionHandler(
              builder =>
              {
                  builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
              });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
