using Tutorial8.Services;

namespace Tutorial8;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        builder.Services.AddSingleton(connectionString);
        builder.Services.AddControllers();
        builder.Services.AddScoped<ITripService, TripService>();
        builder.Services.AddScoped<IClientService, ClientService>();

        builder.Services.AddOpenApi();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}