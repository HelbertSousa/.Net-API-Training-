using Microsoft.OpenApi.Models;
using PizzaStore.Models;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;

// Criando o construtor
// Criando instancia do Aplicativo
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<PizzaDbContext>(options => options.UseInMemoryDatabase("items"));

builder.Services.AddSwaggerGen(c =>
   {
      c.SwaggerDoc("v1", new OpenApiInfo
      {
         Title = "PizzaStore API",
         Description = "Making the Pizzas you love",
         Version = "v1"
      });
   });

var app = builder.Build();

// builder.Services.AddCors(options => {});

app.UseSwagger();
app.UseSwaggerUI(c =>
   {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
   });

app.MapGet("/", () => "Hello World!");
app.MapGet("/pizzas", async (PizzaDbContext db) =>
{
   if (db is null)
   {
      throw new ArgumentNullException(nameof(db));
   }

   return await db.Pizzas.ToListAsync();
});

app.MapPost("/pizzas", async (PizzaDbContext pizzaDb, Pizza pizza) =>
{
   await pizzaDb.Pizzas.AddAsync(pizza);
   await pizzaDb.SaveChangesAsync();
   return Results.Created($"/pizza/{pizza.Id}", pizza);
});

app.MapGet("/pizzas/{id}", async (PizzaDbContext pizzaDb, int id) => await pizzaDb.Pizzas.FindAsync(id));

app.MapPut("/pizzas", async (PizzaDbContext pizzaDb, Pizza updatepizza, int id) =>
{
   var pizza = await pizzaDb.Pizzas.FindAsync(id);
   if (pizza is null) return Results.NotFound();
   pizza.Name = updatepizza.Name;
   pizza.Description = updatepizza.Description;
   await pizzaDb.SaveChangesAsync();
   return Results.NoContent();
});

app.MapDelete("/pizzas/{id}", async (PizzaDbContext pizzaDb, int id) =>
{
   var pizza = await pizzaDb.Pizzas.FindAsync(id);
   if (pizza is null) return Results.NotFound();
   pizzaDb.Pizzas.Remove(pizza);
   await pizzaDb.SaveChangesAsync();
   return Results.Ok();
});

app.Run();
