using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using KonaAnalyzer.Blazor.Areas.Identity;
using KonaAnalyzer.Blazor.Data;
using KonaAnalyzer.Blazor.Pages;
using KonaAnalyzer.Dapper;
using KonaAnalyzer.Data.Interface; 
using Syncfusion.Blazor;
using KonaAnalyzer.ViewModels;

namespace KonaAnalyzer.Blazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddScoped<OverviewViewModel>();
            services.AddScoped<ChangeChartViewModel>();
            services.AddScoped<DapperLocationSource>();
            services.AddScoped<ILocationSource>(x=>x.GetRequiredService<DapperLocationSource>());
            services.AddScoped<DapperCovidSource>();
            services.AddScoped<ICovidSource>(x => x.GetRequiredService<DapperCovidSource>());
            services.AddScoped<ICovidSourceAsync>(x => x.GetRequiredService<DapperCovidSource>());
            services.AddScoped<MaskUseSource>();
            services.AddScoped<IMaskSource>(x => x.GetRequiredService<MaskUseSource>());
            services.AddSyncfusionBlazor();
            services.AddScoped(x=> new KonaContextService(Configuration.GetConnectionString("MacMiniKona"))); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
