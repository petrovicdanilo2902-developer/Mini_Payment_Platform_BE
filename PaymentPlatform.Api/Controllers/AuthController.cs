using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentPlatform.Application.Interfaces;
using PaymentPlatform.Domain.Entities;
using System.Security.Claims;
using static PaymentPlatform.Application.DTOs.AuthDtos;
using System.IdentityModel.Tokens.Jwt;

namespace PaymentPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService) => _userService = userService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            try
            {
                var res = await _userService.RegisterAsync(req);
                return Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            try
            {
                var res = await _userService.LoginAsync(req);
                return Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            // Note: CreateToken used JwtRegisteredClaimNames.Sub = user.Id
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? userId;
            if (sub == null) return Unauthorized();
            var dto = await _userService.GetMeAsync(sub);
            if (dto is null) return NotFound();
            return Ok(dto);
        }
    }
}
