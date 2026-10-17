using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OrderServiceDbContext>(options =>
                options.UseMySQL(Environment.GetEnvironmentVariable("CONNECTION_STRING")));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseSwagger();
                //app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Проверка доступности сервиса
                endpoints.MapGet("/api/orders/isAlive", () => "Welcome to Order Service!");

                // Получение всех продуктов
                endpoints.MapGet("/api/orders/list", async (OrderServiceDbContext db) =>
                {
                    try
                    {
                        var oders = await db.Orders.ToListAsync();
                        return Results.Ok(oders);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(
                            detail: ex.Message,
                            title: "Ошибка при получении списка продуктов",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }
                });

                // Получение продукта по ID
                endpoints.MapGet("/api/oders/{id:int}", async (int id, OrderServiceDbContext db) =>
                {
                    try
                    {
                        var order = await db.Orders.FindAsync(id);
                        if (order == null)
                            return Results.NotFound(new { message = $"Продукт с ID {id} не найден." });

                        return Results.Ok(order);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(
                            detail: ex.Message,
                            title: "Ошибка при получении продукта по ID",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }
                });

                // Создание нового продукта
                endpoints.MapPost("/api/orders/add", async (Order order, OrderServiceDbContext db) =>
                {
                    try
                    {
                        db.Orders.Add(order);
                        await db.SaveChangesAsync();
                        return Results.Created($"/api/orders/{order.Id}", order);
                    }
                    catch (DbUpdateException ex)
                    {
                        return Results.Problem(
                            detail: ex.InnerException?.Message ?? ex.Message,
                            title: "Ошибка при добавлении продукта (проверьте корректность данных)",
                            statusCode: StatusCodes.Status400BadRequest);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(
                            detail: ex.Message,
                            title: "Внутренняя ошибка при создании продукта",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }
                });

                // Обновление продукта
                endpoints.MapPut("/api/orders/{id:int}", async (int id, Order updatedOrder, OrderServiceDbContext db) =>
                {
                    try
                    {
                        var order = await db.Orders.FindAsync(id);
                        if (order == null)
                            return Results.NotFound(new { message = $"Продукт с ID {id} не найден." });

                        order.ProductId = updatedOrder.ProductId;
                        order.Quantity = updatedOrder.Quantity;

                        await db.SaveChangesAsync();
                        return Results.NoContent();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Results.Problem(
                            title: "Конфликт при обновлении данных (возможно, продукт был изменён другим пользователем)",
                            statusCode: StatusCodes.Status409Conflict);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(
                            detail: ex.Message,
                            title: "Ошибка при обновлении продукта",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }
                });

                // Удаление продукта
                endpoints.MapDelete("/api/orders/{id:int}", async (int id, OrderServiceDbContext db) =>
                {
                    try
                    {
                        var order = await db.Orders.FindAsync(id);
                        if (order == null)
                            return Results.NotFound(new { message = $"Продукт с ID {id} не найден." });

                        db.Orders.Remove(order);
                        await db.SaveChangesAsync();
                        return Results.NoContent();
                    }
                    catch (DbUpdateException ex)
                    {
                        return Results.Problem(
                            detail: ex.InnerException?.Message ?? ex.Message,
                            title: "Ошибка при удалении продукта (возможно, есть связанные данные)",
                            statusCode: StatusCodes.Status400BadRequest);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(
                            detail: ex.Message,
                            title: "Внутренняя ошибка при удалении продукта",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }
                });
            });

        }
    }
}
