using MinimalApi.Data.Dto;

namespace MinimalApi.Repository.IRepository {
    public interface IAuthRepository {
        bool IsUserUnique(string username);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequest);
        Task<UserDto> Register(RegistrationRequestDto requestDto);
    }
}
