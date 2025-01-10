using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentTaskManagement.Models;
using Microsoft.CodeAnalysis.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using StudentTaskManagement.Security;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Authorize
builder.Services.AddMvc(options => { 
/*    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    // the user need to sign in
    options.Filters.Add(new AuthorizeFilter(policy));*/
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
    //options.AddPolicy("EditRolePolicy", policy => policy.RequireRole("Edit Role", "true"));

    // Assertion
    /*    options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => 
            context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                context.User.IsInRole("Super Admin")));*/
    options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
    options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));

    //If you do not want the rest pf the handler to be called, when a failure is returned, 
    //set InvokeHandlersAfterFailure property to false. The default is true
    options.InvokeHandlersAfterFailure = false;
});

// Permission
builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
//Data Protection
builder.Services.AddSingleton<DataProtectionPurposeStrings>();


//Add google authentication
builder.Services.AddAuthentication().AddGoogle
    (options =>
    {
        options.ClientId = "819122494798-9g3e9398ekhv805iu1n18mkollcaua41.apps.googleusercontent.com";
        options.ClientSecret = "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";
    });

builder.Services.AddDbContext<StudentTaskManagementContext>(options =>
{
    options.UseSqlServer("Server=LAPTOP-G1DCCDQT; Database=StudentTaskManagementDB; Trusted_Connection=True; TrustServerCertificate=True;");
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
});

builder.Services.AddIdentity<L1Students, IdentityRole>(option => {
    option.Password.RequiredLength = 6;
    option.Password.RequiredUniqueChars = 0;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireUppercase = false;
    option.Password.RequireDigit = false;

    // confirm email
    option.SignIn.RequireConfirmedEmail = true;

    //
    option.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;//"CustomEmailConfirmaion";

    // Lockout
    option.Lockout.MaxFailedAccessAttempts = 5;
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
}).AddEntityFrameworkStores<StudentTaskManagementContext>()
.AddDefaultTokenProviders()
.AddTokenProvider<EmailTokenProvider<L1Students>>(TokenOptions.DefaultEmailProvider); // Add email token provider
//.AddTokenProvider<CustomEmailConfirmationTokenProvider<L1Students>>("CustomEmailConfirmation");

// changes token lifespan of all token types
builder.Services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromMinutes(30));
// changes token lifespan of just the email confirmation token type
builder.Services.Configure<CustomEmailConfirmationTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromDays(3));

// Add this to your service configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
/*builder.Services.Configure<IdentityOptions>(option => {
    option.Password.RequiredLength = 12;
    option.Password.RequiredUniqueChars = 1;
    option.Password.RequireNonAlphanumeric = true;
    option.Password.RequireLowercase = true;
    option.Password.RequireUppercase = true;
    option.Password.RequireDigit = true;
});*/

var app = builder.Build(); 

// Add the DbInitializer here
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StudentTaskManagementContext>();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseAuthentication();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

