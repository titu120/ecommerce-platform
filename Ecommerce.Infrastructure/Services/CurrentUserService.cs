using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ecommerce.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                return userIdClaim != null ? int.Parse(userIdClaim) : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        public string? Role => _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}