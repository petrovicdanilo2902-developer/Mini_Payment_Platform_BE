using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.DTOs
{
    public class AuthDtos
    {
        public record RegisterRequest(string Email, string Password, string FullName);
        public record LoginRequest(string Email, string Password);
        public record AuthResponse(string Token, string UserId, string Email, string FullName);
        public record UserDto(string Id, string Email, string FullName);
    }
}
