using eVOL.Application.DTOs;
using eVOL.Application.DTOs.Requests;
using eVOL.Application.ServicesInterfaces;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BankApi_Clean_Architecture.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginUserUseCase _loginUserUseCase;
        private readonly IRegisterUserUseCase _registerUserUseCase;
        private readonly IRefreshTokenUseCase _refreshTokenUseCase;

        public AuthController(ILoginUserUseCase loginUserUseCase, IRegisterUserUseCase registerUserUseCase, IRefreshTokenUseCase refreshTokenUseCase)
        {
            _loginUserUseCase = loginUserUseCase;
            _registerUserUseCase = registerUserUseCase;
            _refreshTokenUseCase = refreshTokenUseCase;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _registerUserUseCase.ExecuteAsync(dto);

            if (user == null)
            {
                return BadRequest();
            }

            return Ok(new { message = "Registration successful", user.Name, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _loginUserUseCase.ExecuteAsync(dto);
            if (user == null) return Unauthorized("Invalid username or password");

            return Ok(new { message = "Login successful", user.UserId, user.Name, user.Email, user.AccessToken, user.RefreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenDto)
        {
            if (tokenDto is null)
                return BadRequest("Invalid client request");

            var tokens = await _refreshTokenUseCase.ExecuteAsync(tokenDto);

            return Ok(new
            {
                token = tokens?.AccessToken,
                refreshToken = tokens?.RefreshToken
            });
        }


    }
}
