using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentPlatform.Application.DTOs.AuthDtos;

namespace PaymentPlatform.Application.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserDto?> GetMeAsync(string userId);
    }
}
