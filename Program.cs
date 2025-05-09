var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//view���ΤF(�oRazor�Ϊ�)
//builder.Services.AddControllersWithViews(); 
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //view���ΤF(�oRazor�Ϊ�)
    //app.UseExceptionHandler("/Home/Error"); (�oRazor�Ϊ�)
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//�ɦV����
app.UseDefaultFiles();  // �� / �۰ʶ} wwwroot/index.html
app.UseStaticFiles();  // �䴩�R�A HTML�BJS�BCSS

app.UseRouting();

app.UseAuthorization();

//����Razor
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // �ϥ� Web API ���

app.Run();
