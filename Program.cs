var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//view不用了(這Razor用的)
//builder.Services.AddControllersWithViews(); 
builder.Services.AddControllers();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 自動過期時間
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //view不用了(這Razor用的)
    //app.UseExceptionHandler("/Home/Error"); (這Razor用的)
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();

app.UseHttpsRedirection();
//導向首頁
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "login/index.html" } // 不用加 `/`
});
app.UseStaticFiles();
app.UseStaticFiles();  // 支援靜態 HTML、JS、CSS
app.UseRouting();
app.UseAuthorization();
//不用Razor
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers(); // 使用 Web API 控制器
app.Run();
