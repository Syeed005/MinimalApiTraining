using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Data.Dto;
using MinimalApi.Database;
using MinimalApi.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalApi.Repository {
    public class AuthRepository : IAuthRepository {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private string secret;

        public AuthRepository(ApplicationDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
            secret = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public bool IsUserUnique(string username) {
            if (_db.LocalUsers.FirstOrDefault(x=>x.UserName == username) == null) {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequest) {
            var user = _db.LocalUsers.SingleOrDefault(x => x.UserName == loginRequest.UserName && x.Password == loginRequest.Password);
            if (user == null) {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, "admin")
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDto loginResponse = new LoginResponseDto() {
                User = _mapper.Map<UserDto>(user),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return loginResponse;
        }

        public async Task<UserDto> Register(RegistrationRequestDto requestDto) {
            LocalUser user = _mapper.Map<LocalUser>(requestDto);
            user.Role = "admin";
            _db.LocalUsers.Add(user);
            _db.SaveChanges();
            return _mapper.Map<UserDto>(user);
        }
    }
}
