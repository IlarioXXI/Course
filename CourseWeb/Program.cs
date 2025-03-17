using Course.DataAccess.Data;
using Course.DataAccess.Repository;
using Course.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Course.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

//inietta automaticamente le chiavi di stripe presenti in appsettings
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LoginPath = $"/Identity/Account/Logout";
    options.LoginPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = builder.Configuration["Facebook:AppId"];
    options.AppSecret = builder.Configuration["Facebook:AppSecret"];
    //con questa opzione se l'autente annulla il login di facebook viene reindirizzato alla pagina di login
    //poiché l'evento di annulla nel login di facebook è gestito da facebook come se fosse negato l'accesso
    //così possiamo settare un eventuale reiderizzamento della pagina 
    options.Events.OnRemoteFailure = ContextBoundObject =>
    {
        ContextBoundObject.Response.Redirect("identity/account/login");

        //Chiamando HandleResponse dopo Redirect, ci si assicura che la risposta 
        //venga immediatamente inviata al client e che non si verifichi alcuna ulteriore 
        //elaborazione per la richiesta corrente. Ciò è importante negli scenari in cui si desidera terminare anticipatamente 
        //l'elaborazione della richiesta, ad esempio per la gestione di errori di autenticazione.

        ContextBoundObject.HandleResponse();
        //ritorna un task completato poiché onRemoteFaiure è un metodo async
        //quindi usiamo handleResponse per terminare la richiesta e fermare altri processi
        return Task.CompletedTask;
    };
});

//aggiunge la memoryCache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    //se l'utente non fa nulla per 100 minuti la sessione scade
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    //HttpOnly è un flag che indica che il cookie non può essere letto da javascript quindi è più sicuro
    options.Cookie.HttpOnly = true;
    //anche se l'utente non accetta i cookie non essenziali la sessione funziona
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//stripe configurato
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<String>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
 