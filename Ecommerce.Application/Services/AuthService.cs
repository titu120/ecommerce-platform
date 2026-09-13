using Ecommerce.Application.DTOs.Auth;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Settings;
using Ecommerce.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IUnitOfWork unitOfWork,
            IValidator<RegisterDto> registerValidator,
            IValidator<LoginDto> loginValidator,
            IOptions<JwtSettings> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<UserResponseDto> RegisterAsync(RegisterDto dto)
        {
            var validationResult = await _registerValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(validationResult.Errors);
            }

            var emailExists = await _unitOfWork.Users.EmailExistsAsync(dto.Email);
            if (emailExists)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(
                    new List<FluentValidation.Results.ValidationFailure>
                    {
                        new("Email", "This email is already registered.")
                    });
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Customer"
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var validationResult = await _loginValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(validationResult.Errors);
            }

            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Invalid email or password.");
            }

            var (token, expiresAt) = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }

        private (string token, DateTime expiresAt) GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}