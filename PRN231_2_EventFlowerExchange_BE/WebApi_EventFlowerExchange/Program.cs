using BusinessObject;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;



var builder = WebApplication.CreateBuilder(args);
// Config OData

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<User>("User");
modelBuilder.EntitySet<Flower>("Flower");
modelBuilder.EntitySet<Batch>("Batch");
modelBuilder.EntitySet<Company>("Company");
modelBuilder.EntitySet<Review>("Review");
modelBuilder.EntitySet<Delivery>("Delivery");
modelBuilder.EntitySet<Payment>("Payment");
modelBuilder.EntitySet<Order>("Order");
modelBuilder.EntitySet<OrderDetail>("OrderDetail");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddOData(
    options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
        "odata",
        modelBuilder.GetEdmModel()));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseODataBatching();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
