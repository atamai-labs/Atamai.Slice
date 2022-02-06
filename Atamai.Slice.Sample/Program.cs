using Atamai.Slice;
using Atamai.Slice.Authorization;
using Atamai.Slice.Sample;
using Atamai.Slice.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.AddSlice());
builder.Services.AddSingleton<DataBase>();
builder.Services.AddScoped<IAuthorizer, Authorizer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSlice();
app.Run();