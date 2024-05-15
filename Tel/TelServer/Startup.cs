using Tel.Core.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using Tel.Core.Config;
using System.Text;
using Tel.Api.Filters;

#if DEBUG
using Microsoft.OpenApi.Models;
#endif

namespace TelServer;

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
        services.AddAuthorization();

        services.AddControllersWithViews();

#if DEBUG
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v2", new OpenApiInfo { Title = "Tel.Api", Version = "v2" });
        });
#endif
        // -------------------Tel STEP1 OF 3------------------
        services.AddTelServer(Configuration.GetSection("TelConfig"));
        // -------------------Tel STEP1 END-------------------

        services.AddCors(options => {
            options.AddDefaultPolicy(policy =>
            {
                policy.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
        defaultFilesOptions.DefaultFileNames.Clear();
        defaultFilesOptions.DefaultFileNames.Add("index.html");
        app.UseDefaultFiles(defaultFilesOptions);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
#if DEBUG
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "Tel.WebApi v2"));
#endif
        }

        app.UseRouting();

        // -------------------Tel STEP2 OF 3------------------
        app.UseTelServer();
        // -------------------Tel STEP2 END-------------------

        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            // -------------------Tel STEP3 OF 3------------------
            endpoints.MapTelServer();
            // -------------------Tel STEP3 END-------------------
        });
    }
}
