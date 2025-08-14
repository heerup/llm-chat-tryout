using LlmChatApp.Data.Interfaces;
using LlmChatApp.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HTTP client for Ollama integration
builder.Services.AddHttpClient();

// Register data services
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IConversationService, ConversationService>();
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddSingleton<IQueueService, QueueService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<ILlmService, LlmService>();

// Configure session for simple authentication
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30); // 30 days instead of 30 minutes
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "LlmChatApp.Session";
    options.Cookie.SameSite = SameSiteMode.Lax;
});

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

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
