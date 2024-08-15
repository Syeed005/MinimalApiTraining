
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Data;
using MinimalApi.Data.Dto;
using MinimalApi.Database;
using MinimalApi.Repository;
using MinimalApi.Repository.IRepository;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.Xml;

namespace MinimalApi {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            //add dbcontext
            builder.Services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<ICouponRepository, CouponRepository>();

            
            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingConfig));
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapGet("/api/coupon", async (ICouponRepository _couponRepo, ILogger<Program> _logger) => {
                _logger.Log(LogLevel.Information,"Getting all coupon");

                APIResponse response = new APIResponse();
                response.IsSuccess = true;
                response.Result = await _couponRepo.GetAllAsync();
                response.StatusCode = HttpStatusCode.OK;

                return Results.Ok(response);
            }).WithName("GetCoupons").Produces<APIResponse>(200);

            app.MapGet("/api/coupon/{id:int}", async (ICouponRepository _couponRepo, int id) => {
                APIResponse response = new APIResponse();
                response.IsSuccess = true;
                response.Result = await _couponRepo.GetAsync(id,false);
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("GetCoupon").Produces<APIResponse>(200);

            app.MapPost("/api/coupon", async (ICouponRepository _couponRepo, IValidator<CouponCreatedDto> _validator, IMapper _mapper, [FromBody] CouponCreatedDto couponCreatedDto) => {
                APIResponse response = new APIResponse();
                var validationResult = await _validator.ValidateAsync(couponCreatedDto);
                if (!validationResult.IsValid) {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                    return Results.BadRequest(response);
                }
                
                if (await _couponRepo.GetAsync(couponCreatedDto.Name, false) != null) {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Coupon name is already exists");
                    return Results.BadRequest(response);
                }

                Coupon coupon = _mapper.Map<Coupon>(couponCreatedDto);
                //coupon.Id = CouponStore.CouponList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                //CouponStore.CouponList.Add(coupon);
                await _couponRepo.CreateAsync(coupon);
                await _couponRepo.SaveAsync();

                CouponDto couponDto = _mapper.Map<CouponDto>(coupon);

                response.IsSuccess = true;
                response.Result = couponDto;
                response.StatusCode = HttpStatusCode.Created;

                return Results.Ok(response);

                //return Results.CreatedAtRoute($"GetCoupon",new { Id = couponDto.Id}, couponDto);
                //return Results.Created($"/api/coupon/{coupon.Id}", coupon);
            }).WithName("PostCoupon").Accepts<CouponCreatedDto>("application/json").Produces<APIResponse>(201).Produces(400);

            app.MapPut("/api/coupon", async (ICouponRepository _couponRepo, IValidator<CouponUpdatedDto> _validator, IMapper _mapper, [FromBody] CouponUpdatedDto couponUpdatedDto) => {
                APIResponse response = new APIResponse();
                var responseResult = await _validator.ValidateAsync(couponUpdatedDto);
                
                if (!responseResult.IsValid) {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add(responseResult.Errors.FirstOrDefault().ToString());
                    return Results.BadRequest(response);
                }

                //Coupon coupon = CouponStore.CouponList.Where(x => x.Id == couponUpdatedDto.Id).FirstOrDefault();
                Coupon coupon = await _couponRepo.GetAsync(couponUpdatedDto.Id, false);
                if (coupon == null) {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Coupon is not found");
                    return Results.BadRequest(response);
                }

                //coupon.Name = couponUpdatedDto.Name;
                //coupon.Percent = couponUpdatedDto.Percent;
                //coupon.IsActive = couponUpdatedDto.IsActive;
                //coupon.LastUpdated = DateTime.Now;

                await _couponRepo.UpdateAsync(_mapper.Map<Coupon>(couponUpdatedDto));
                await _couponRepo.SaveAsync();

                //CouponDto couponDto = _mapper.Map<CouponDto>(coupon);

                response.IsSuccess = true;
                response.Result = _mapper.Map<CouponDto>(await _couponRepo.GetAsync(couponUpdatedDto.Id, true));
                response.StatusCode = HttpStatusCode.OK;

                return Results.Ok(response);
                //Coupon coupon = _mapper.Map<Coupon>(couponUpdatedDto);

            }).WithName("UpdateCoupon").Accepts<CouponUpdatedDto>("application/json").Produces<APIResponse>(200).Produces(400);

            app.MapDelete("/api/coupon/{id:int}", async (ICouponRepository _couponRepo, int id) => {
                APIResponse response = new APIResponse();
                
                if (id == 0) {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Id is invalid");
                    return Results.BadRequest(response);
                }

                Coupon coupon = await _couponRepo.GetAsync(id, true);

                if (coupon == null) {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Coupon is not found");
                    return Results.BadRequest(response);
                }

                await _couponRepo.RemoveAsync(coupon);
                await _couponRepo.SaveAsync();
                //CouponStore.CouponList.Remove(coupon);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.NoContent;
                return Results.Ok(response);

            }).WithName("DeleteCoupon").Produces(400).Produces<APIResponse>(204);

            app.Run();
        }
    }
}
