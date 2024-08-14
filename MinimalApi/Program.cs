
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Data;
using MinimalApi.Data.Dto;
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
            builder.Services.AddAutoMapper(typeof(MappingConfig));

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

            app.MapPost("/api/coupon", (IMapper _mapper, [FromBody] CouponCreatedDto couponCreatedDto) => {
                if (string.IsNullOrEmpty(couponCreatedDto.Name) ) {
                    return Results.BadRequest("Invalid Id or coupon name");
                }
                if (CouponStore.CouponList.FirstOrDefault(x=>x.Name.ToLower() == couponCreatedDto.Name.ToLower()) != null) {
                    return Results.BadRequest("Coupon name is already exists");
                }
                Coupon coupon = _mapper.Map<Coupon>(couponCreatedDto);
                coupon.Id = CouponStore.CouponList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                CouponStore.CouponList.Add(coupon);
                CouponDto couponDto = _mapper.Map<CouponDto>(coupon);
                return Results.CreatedAtRoute($"GetCoupon",new { Id = couponDto.Id}, couponDto);
                //return Results.Created($"/api/coupon/{coupon.Id}", coupon);
            }).WithName("PostCoupon").Accepts<CouponCreatedDto>("application/json").Produces<CouponDto>(201).Produces(400);

            app.MapPut("/api/coupon", () => {
                
            }).WithName("UpdateCoupon");

            app.MapDelete("/api/coupon/{id:int}", (int id) => {

            }).WithName("DeleteCoupon");

            app.Run();
        }
    }
}
