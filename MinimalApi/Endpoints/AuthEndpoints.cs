
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Data;
using MinimalApi.Data.Dto;
using MinimalApi.Repository.IRepository;
using System.Net;

namespace MinimalApi.Endpoints {
    public static class AuthEndpoints {
        public static void ConfigureAuthEndpoints(this WebApplication app) {
            app.MapPost("api/login", Login).WithName("Login").Accepts<LoginRequestDto>("application/json").Produces<APIResponse>(200).Produces(400);

            app.MapPost("api/register", Register).WithName("Register").Accepts<RegistrationRequestDto>("application/json").Produces<APIResponse>(200).Produces(400);
        }

        private static async Task<IResult> Register(IAuthRepository _authRepo, [FromBody] RegistrationRequestDto registerRequest) {
            APIResponse response = new APIResponse();
            if (_authRepo.IsUserUnique(registerRequest.UserName) == false) {
                response.ErrorMessages.Add("Username already exist");
                response.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(response);
            }

            var registerResponse = await _authRepo.Register(registerRequest);
            if (registerRequest == null || string.IsNullOrEmpty(registerResponse.UserName)) {
                return Results.BadRequest(response);
            }            
            
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        }

        private static async Task<IResult> Login(IAuthRepository _authRepo ,[FromBody]LoginRequestDto loginRequest) {
            APIResponse response = new APIResponse();

            LoginResponseDto loginResponse = await _authRepo.Login(loginRequest);
            if (loginResponse == null) {
                response.ErrorMessages.Add("Username or password is incorrect");
                response.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(response);
            }            

            response.IsSuccess = true;
            response.Result = loginResponse;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);

        }
    }
}
