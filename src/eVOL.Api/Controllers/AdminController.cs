using eVOL.Application.DTOs;
using eVOL.Application.UseCases.UCInterfaces.IAdminCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace eVOL.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {

        private readonly IAdminGetUserUseCase _getUserUseCase;
        private readonly IAdminDeleteUserUseCase _deleteUserUseCase;

        public AdminController(IAdminGetUserUseCase getUserUseCase, IAdminDeleteUserUseCase deleteUserUseCase)
        {
            _getUserUseCase = getUserUseCase;
            _deleteUserUseCase = deleteUserUseCase;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserInfo(int id)
        {
            var user = await _getUserUseCase.ExecuteAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id) 
        {
            var user = await _deleteUserUseCase.ExecuteAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


    }
}
