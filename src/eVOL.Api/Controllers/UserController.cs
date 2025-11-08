using eVOL.Application.DTOs.Requests;
using eVOL.Application.UseCases.UCInterfaces.IUserCases;
using eVOL.Application.UseCases.UserCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace eVOL.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class UserController : ControllerBase
    {

        private readonly IUpdateUserUseCase _updateUserUseCase;
        private readonly IGetUserUseCase _getUserUseCase;
        private readonly IDeleteUserUseCase _deleteUserUseCase;

        public UserController(IUpdateUserUseCase updateUserUseCase, IGetUserUseCase getUserUseCase, IDeleteUserUseCase deleteUserUseCase)
        {
            _updateUserUseCase = updateUserUseCase;
            _getUserUseCase = getUserUseCase;
            _deleteUserUseCase = deleteUserUseCase;
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _updateUserUseCase.ExecuteAsync(dto);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromBody] DeleteAccountDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _deleteUserUseCase.ExecuteAsync(dto);

            if (user == null)
                return NotFound(new { message = $"User with id {id} not found" });

            return Ok(new { message = "Account has been deleted successfully", user.Name });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _getUserUseCase.ExecuteAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
