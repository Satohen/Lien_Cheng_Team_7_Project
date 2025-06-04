var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//view���ΤF(�oRazor�Ϊ�)
//builder.Services.AddControllersWithViews(); 
builder.Services.AddControllers();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // �۰ʹL���ɶ�
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //view���ΤF(�oRazor�Ϊ�)
    //app.UseExceptionHandler("/Home/Error"); (�oRazor�Ϊ�)
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();

app.UseHttpsRedirection();
//�ɦV����
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "login/index.html" } // ���Υ[ `/`
});
app.UseStaticFiles();
app.UseStaticFiles();  // �䴩�R�A HTML�BJS�BCSS
app.UseRouting();
app.UseAuthorization();
//����Razor
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers(); // �ϥ� Web API ���
app.Run();
