using Shopping.Aggregrator.Services;
using System.Net.Http;
namespace Shopping.Aggregrator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Add services to the container.
            builder.Services.AddAuthorization();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddHttpClient<ICatalogService, CatalogService>
                (c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]));
            builder.Services.AddHttpClient<IBasketService, BasketService>
              (c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]));
            builder.Services.AddHttpClient<IOrderService, OrderService>
              (c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();
            app.Run();
        }
    }
}