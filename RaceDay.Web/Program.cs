var builder = WebApplication.CreateBuilder(args);

//Register and HTTP client for communicating with the RaceDay API
builder.Services.AddHttpClient("RaceDayApi",client =>
{
    //Read the API address from appsettings.json
    client.BaseAddress = new Uri(builder.Configuration["ApiSetting:BaseUri1"]!);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
