using Microsoft.AspNetCore.Identity;
using PaymentPlatform.Application.Interfaces;
using PaymentPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentPlatform.Application.DTOs.AuthDtos;

namespace PaymentPlatform.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepo, ITokenService tokenService, IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var existing = await _userRepo.GetByEmailAsync(normalizedEmail);
            if (existing is not null)
                throw new InvalidOperationException("Email already registered.");

            var user = new User
            {
                Email = normalizedEmail,
                FullName = request.FullName,
                Role = "User"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepo.AddAsync(user);

            var token = _tokenService.CreateToken(user);
            return new AuthResponse(token, user.Id.ToString(), user.Email, user.FullName);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepo.GetByEmailAsync(normalizedEmail)
                       ?? throw new InvalidOperationException("Invalid credentials.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Invalid credentials.");

            var token = _tokenService.CreateToken(user);
            return new AuthResponse(token, user.Id.ToString(), user.Email, user.FullName);
        }

        public async Task<UserDto?> GetMeAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var id)) return null;
            var user = await _userRepo.GetByIdAsync(id);
            return user is null ? null : new UserDto(user.Id.ToString(), user.Email, user.FullName);
        }
    }
}
