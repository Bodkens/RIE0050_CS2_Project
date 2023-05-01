using DatabaseLayer;
namespace CureConsultation
{
    public class Program
    {
        public static User? CurrentUser { get; set; }

        public static List<Appointment>? UserAppointments { get; set; }

        public static List<Consultation>? Consultations { get; set; }



        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddSingleton<LoginService>();
            builder.Services.AddSingleton<AppointmentService>();

            

            var app = builder.Build();

            

            app.UseSession();

            
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
        }
    }
}