using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.API.Extensions;
using Ordering.Infrastructure.Persistence;
using MassTransit;
using EventBus.Messages.Common;
using Ordering.API.EventBusConsumer;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.m    s/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Service Collection Extensions
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddMassTransit(config =>
            {
                config.AddConsumer<BasketCheckoutConsumer>();
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
                    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
                    {
                        c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                    });
                });
            });

            //builder.Services.AddOptions<MassTransitHostOptions>()
            //.Configure(options =>
            //{
            //    // if specified, waits until the bus is started before
            //    // returning from IHostedService.StartAsync
            //    // default is false
            //    options.WaitUntilStarted = true;

            //    // if specified, limits the wait time when starting the bus
            //    options.StartTimeout = TimeSpan.FromSeconds(10);

            //    // if specified, limits the wait time when stopping the bus
            //    options.StopTimeout = TimeSpan.FromSeconds(30);
            //});

            builder.Services.AddAutoMapper(typeof(Program));

            var app = builder.Build();

            app.MigrateDatabase<OrderContext>((context, services) => {

                var logger = services.GetService<ILogger<OrderContextSeed>>();
                OrderContextSeed.SeedAsync(context, logger)
                                .Wait();
            });

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