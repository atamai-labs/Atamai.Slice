using Atamai.Slice;
using Atamai.Slice.Authentication;
using Atamai.Slice.Sample;
using Atamai.Slice.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.AddSlice());
builder.Services.AddSingleton<DataBase>();
builder.Services.AddScoped<Authenticator>();
builder.Services.AddScoped<IAuthenticator>(s => s.GetRequiredService<Authenticator>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSlice();
app.Run();