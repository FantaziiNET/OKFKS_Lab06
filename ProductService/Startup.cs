using Microsoft.EntityFrameworkCore;

namespace ProductService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProductServiceDbContext>(options =>
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
                endpoints.MapGet("/api/products/isAlive", () => "Welcome to Product Service!");

                // Получение всех продуктов
                endpoints.MapGet("/api/products/list", async (ProductServiceDbContext db) =>
                {
                    try
                    {
                        var products = await db.Products.ToListAsync();
                        return Results.Ok(products);
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
                endpoints.MapGet("/api/products/{id:int}", async (int id, ProductServiceDbContext db) =>
                {
                    try
                    {
                        var product = await db.Products.FindAsync(id);
                        if (product == null)
                            return Results.NotFound(new { message = $"Продукт с ID {id} не найден." });

                        return Results.Ok(product);
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
                endpoints.MapPost("/api/products/add", async (Product product, ProductServiceDbContext db) =>
                {
                    try
                    {
                        db.Products.Add(product);
                        await db.SaveChangesAsync();
                        return Results.Created($"/api/products/{product.Id}", product);
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
                endpoints.MapPut("/api/products/{id:int}", async (int id, Product updatedProduct, ProductServiceDbContext db) =>
                {
                    try
                    {
                        var product = await db.Products.FindAsync(id);
                        if (product == null)
                            return Results.NotFound(new { message = $"Продукт с ID {id} не найден." });

                        product.Name = updatedProduct.Name;
                        product.Price = updatedProduct.Price;

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
                endpoints.MapDelete("/api/products/{id:int}", async (int id, ProductServiceDbContext db) =>
                {
                    try
                    {
                        var product = await db.Products.FindAsync(id);
                        if (product == null)
                            return Results.NotFound(new { message = $"Продукт с ID {id} не найден." });

                        db.Products.Remove(product);
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