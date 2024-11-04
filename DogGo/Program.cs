//using DogGo.Repositories;

//namespace DogGo
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.
//            builder.Services.AddControllersWithViews();

//            builder.Services.AddTransient<IWalkerRepository, WalkerRepository>(); //added this --let ASP.NET Core know about our new repository. 
//            builder.Services.AddTransient<IOwnerRepository, OwnerRepository>();
//            builder.Services.AddTransient<IDogRepository, DogRepository>();
//            builder.Services.AddTransient<INeighborhoodRepository, NeighborhoodRepository>();

//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//                app.UseHsts();
//            }

//            app.UseHttpsRedirection();
//            app.UseStaticFiles();

//            app.UseRouting();

//            app.UseAuthorization();

//            //In ASP.NET framework, each of the public methods in the controllers is considered an Action.
//            //App receives HTTP requests, ASP.NET know which controller Action to invoke.
//            //ASP.NET inspect url/request: localhost:5001/Walkers/Index, the framework will look for an Index action on the Walker controller and invoke it. If a request comes in at localhost:5001/Walkers/Details/5, The framework will look for a Details action in the Walkers controller and invoke it while passing in the parameter 5.

//            //note about default: If url dont have an action, ASP.NET invoke Index action by default--meaning localhost:5001/Walkers would still trigger the Index action in the Walkers controller. Likewise, if the url doesn't contain a controller, i.e. localhost:5001/, the framework will assume the Home controller and the Index action. You can change these defaults.
//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}");

//            app.Run();
//        }
//    }
//}




using DogGo.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DogGo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register repositories with dependency injection
            builder.Services.AddTransient<IWalkerRepository, WalkerRepository>(); //added this --let ASP.NET Core know about our new repository. 
            builder.Services.AddTransient<IOwnerRepository, OwnerRepository>();
            builder.Services.AddTransient<IDogRepository, DogRepository>();
            builder.Services.AddTransient<INeighborhoodRepository, NeighborhoodRepository>();

            //added this for login:
            // Configure authentication services
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    // Set the login path for the application
                    options.LoginPath = "/Owners/LogIn"; //--------------------REDIRECT PATH IF NOT AUTHORIZED (LOGGED IN) USER like if try to access dogs when not logged in
                });

            var app = builder.Build();

            //NOTE:Dont need Startup.cs file. The configuration is done within the Program.cs file using the WebApplication builder and the app instance.
            //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            //.NET 6 and later, the Program.cs file handles both service configuration (previously in ConfigureServices in Startup.cs) and middleware setup (previously in Configure in Startup.cs). You configure services and middleware directly within Program.cs.

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                //added this for login:
                app.UseDeveloperExceptionPage();

            }
            else
            {

            //moved previous content in else:
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            // Added this for login: 
            //// Add authentication middleware
            app.UseAuthorization();

            //In ASP.NET framework, each of the public methods in the controllers is considered an Action.
            //App receives HTTP requests, ASP.NET know which controller Action to invoke.
            //ASP.NET inspect url/request: localhost:5001/Walkers/Index, the framework will look for an Index action on the Walker controller and invoke it. If a request comes in at localhost:5001/Walkers/Details/5, The framework will look for a Details action in the Walkers controller and invoke it while passing in the parameter 5.

            //note about default: If url dont have an action, ASP.NET invoke Index action by default--meaning localhost:5001/Walkers would still trigger the Index action in the Walkers controller. Likewise, if the url doesn't contain a controller, i.e. localhost:5001/, the framework will assume the Home controller and the Index action. You can change these defaults.
            // Configure endpoints:
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}