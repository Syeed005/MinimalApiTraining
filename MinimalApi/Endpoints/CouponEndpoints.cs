using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MinimalApi.Data.Dto;
using MinimalApi.Data;
using MinimalApi.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace MinimalApi.Endpoints {
    public static class CouponEndpoints {
        public static void ConfigureCouponEndpoints(this WebApplication app) {

            app.MapGet("/api/coupon", GetAllCoupon).WithName("GetCoupons").Produces<APIResponse>(200).RequireAuthorization("AdminOnly");

            app.MapGet("/api/coupon/{id:int}", GetCoupon).WithName("GetCoupon").Produces<APIResponse>(200).AddEndpointFilter(async(context, next) => {
                var id = context.GetArgument<int>(1);
                if (id == 0) {
                    return Results.BadRequest("Id cant not be 0");
                }
                //logic before enpoint
                var result =  await next(context);
                //logic after endpont
                return result;
            });

            app.MapPost("/api/coupon", CreateCoupon).WithName("PostCoupon").Accepts<CouponCreatedDto>("application/json").Produces<APIResponse>(201).Produces(400);

            app.MapPut("/api/coupon", UpdateCoupon).WithName("UpdateCoupon").Accepts<CouponUpdatedDto>("application/json").Produces<APIResponse>(200).Produces(400);

            app.MapDelete("/api/coupon/{id:int}", DeleteCoupon).WithName("DeleteCoupon").Produces(400).Produces<APIResponse>(204);
        }
        public async static Task<IResult> GetAllCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger) {
            APIResponse response = new APIResponse();
            response.IsSuccess = true;
            response.Result = await _couponRepo.GetAllAsync();
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        public async static Task<IResult> GetCoupon(ICouponRepository _couponRepo, int id) {
            APIResponse response = new APIResponse();
            response.IsSuccess = true;
            response.Result = await _couponRepo.GetAsync(id, false);
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        [Authorize]
        public async static Task<IResult> CreateCoupon(ICouponRepository _couponRepo, IValidator<CouponCreatedDto> _validator, IMapper _mapper, [FromBody] CouponCreatedDto couponCreatedDto) {
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
        }
        public async static Task<IResult> UpdateCoupon(ICouponRepository _couponRepo, IValidator<CouponUpdatedDto> _validator, IMapper _mapper, [FromBody] CouponUpdatedDto couponUpdatedDto) {
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
        }
        public async static Task<IResult> DeleteCoupon(ICouponRepository _couponRepo, int id) {
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
        }
    }
}
