
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Data;
using System.Collections.Generic;

namespace MinimalApi {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapGet("/api/coupon", (ILogger<Program> _logger) => {
                _logger.LogInformation("Getting all coupon");
                return Results.Ok(CouponStore.CouponList);
            }).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);

            app.MapGet("/api/coupon/{id:int}", (int id) => {
                return Results.Ok(CouponStore.CouponList.FirstOrDefault(x=>x.Id==id));
            }).WithName("GetCoupon").Produces<Coupon>(200);

            app.MapPost("/api/coupon", ([FromBody] Coupon coupon) => {
                if (coupon.Id != 0 || string.IsNullOrEmpty(coupon.Name) ) {
                    return Results.BadRequest("Invalid Id or coupon name");
                }
                if (CouponStore.CouponList.FirstOrDefault(x=>x.Name.ToLower() == coupon.Name.ToLower()) != null) {
                    return Results.BadRequest("Coupon name is already exists");
                }
                coupon.Id = CouponStore.CouponList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                CouponStore.CouponList.Add(coupon);
                return Results.CreatedAtRoute($"GetCoupon",new { Id = coupon.Id}, coupon);
                //return Results.Created($"/api/coupon/{coupon.Id}", coupon);
            }).WithName("PostCoupon").Accepts<Coupon>("application/json").Produces<Coupon>(201).Produces(400);

            app.MapPut("/api/coupon", () => {
                
            }).WithName("UpdateCoupon");

            app.MapDelete("/api/coupon/{id:int}", (int id) => {

            }).WithName("DeleteCoupon");

            app.Run();
        }
    }
}
