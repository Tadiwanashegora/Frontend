using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EdgeFrontend.Data;
using EdgeFrontend.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using EdgeFrontend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services with Entity Framework stores
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Register all services with their implementations
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

// Add session support for cart functionality
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add memory cache for caching service
builder.Services.AddMemoryCache();

// Add external authentication providers (optional - uncomment if needed)
/*
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    });
*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session must come before authentication
app.UseSession();

// Authentication and Authorization must be in correct order
app.UseAuthentication();
app.UseAuthorization();

// Custom 404 handling
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/404";
        await next();
    }
});






app.MapGet("/Account/about.cshtml", () => Results.Redirect("/EdgeRentals/about"));

app.MapGet("/Account/wishlist.cshtml", () => Results.Redirect("/EdgeRentals/Wishlist"));
app.MapGet("/Account/404.cshtml", () => Results.Redirect("/EdgeRentals/404"));
app.MapGet("/Account/cart.cshtml", () => Results.Redirect("/EdgeRentals/cart"));
app.MapGet("/Account/Index.cshtml", () => Results.Redirect("/EdgeRentals/Index"));
app.MapGet("/Account/login.cshtml", () => Results.Redirect("/EdgeRentals/login"));
app.MapGet("/Account/product.cshtml", () => Results.Redirect("/EdgeRentals/product"));
// Route mappings for static .cshtml files
app.MapGet("/404.cshtml", () => Results.Redirect("/Home/Error"));
app.MapGet("/about.cshtml", () => Results.Redirect("/EdgeRentals/About"));
app.MapGet("/cart.cshtml", () => Results.Redirect("/EdgeRentals/Cart"));
app.MapGet("/checkout.cshtml", () => Results.Redirect("/EdgeRentals/Checkout"));
app.MapGet("/contact.cshtml", () => Results.Redirect("/EdgeRentals/Contact"));
app.MapGet("/dashboard.cshtml", () => Results.Redirect("/EdgeRentals/Dashboard"));
app.MapGet("/elements-cta.cshtml", () => Results.Redirect("/Home/ElementsCta"));
app.MapGet("/elements-portfolio.cshtml", () => Results.Redirect("/Home/ElementsPortfolio"));
app.MapGet("/elements-product-category.cshtml", () => Results.Redirect("/Home/ElementsProductCategory"));
app.MapGet("/elements-products.cshtml", () => Results.Redirect("/Home/ElementsProducts"));
app.MapGet("/faq.cshtml", () => Results.Redirect("/Home/Faq"));
app.MapGet("/Account/checkout.cshtml", () => Results.Redirect("/EdgeRentals/Checkout"));
app.MapGet("/login.cshtml", () => Results.Redirect("/Account/Login"));
app.MapGet("/Privacy.cshtml", () => Results.Redirect("/EdgeRentals/Privacy"));
app.MapGet("/product.cshtml", () => Results.Redirect("/EdgeRentals/Products"));
app.MapGet("/wishlist.cshtml", () => Results.Redirect("/EdgeRentals/Wishlist"));

// Handle EdgeRentals subdirectory paths
app.MapGet("/EdgeRentals/checkout.cshtml", () => Results.Redirect("/EdgeRentals/Checkout"));
app.MapGet("/EdgeRentals/cart.cshtml", () => Results.Redirect("/EdgeRentals/Cart"));
app.MapGet("/EdgeRentals/wishlist.cshtml", () => Results.Redirect("/EdgeRentals/Wishlist"));
app.MapGet("/EdgeRentals/contact.cshtml", () => Results.Redirect("/EdgeRentals/Contact"));
app.MapGet("/EdgeRentals/about.cshtml", () => Results.Redirect("/EdgeRentals/About"));
app.MapGet("/EdgeRentals/dashboard.cshtml", () => Results.Redirect("/EdgeRentals/Dashboard"));
app.MapGet("/EdgeRentals/product.cshtml", () => Results.Redirect("/EdgeRentals/Products"));
app.MapGet("/EdgeRentals/index.cshtml", () => Results.Redirect("/EdgeRentals/Index"));

app.MapGet("/Categories.cshtml", () => Results.Redirect("/EdgeRentals/Categories"));
app.MapGet("/EdgeRentals/category.cshtml", () => Results.Redirect("/EdgeRentals/Categories"));

// Route mappings for static .cshtml files - ROOT LEVEL
app.MapGet("/index.cshtml", () => Results.Redirect("/EdgeRentals/Index"));
app.MapGet("/blog.cshtml", () => Results.Redirect("/EdgeRentals/Blog"));
app.MapGet("/products.cshtml", () => Results.Redirect("/EdgeRentals/Products"));
app.MapGet("/category.cshtml", () => Results.Redirect("/EdgeRentals/Categories"));
app.MapGet("/register.cshtml", () => Results.Redirect("/Account/Register"));
app.MapGet("/terms.cshtml", () => Results.Redirect("/EdgeRentals/Terms"));

// Route mappings for EdgeRentals subdirectory paths
app.MapGet("/EdgeRentals/blog.cshtml", () => Results.Redirect("/EdgeRentals/Blog"));
app.MapGet("/EdgeRentals/products.cshtml", () => Results.Redirect("/EdgeRentals/Products"));
app.MapGet("/EdgeRentals/categories.cshtml", () => Results.Redirect("/EdgeRentals/Categories"));
app.MapGet("/EdgeRentals/privacy.cshtml", () => Results.Redirect("/EdgeRentals/Privacy"));
app.MapGet("/EdgeRentals/terms.cshtml", () => Results.Redirect("/EdgeRentals/Terms"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EdgeRentals}/{action=Index}/{id?}");

app.Run();