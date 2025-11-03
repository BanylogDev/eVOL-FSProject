using eVOL.Application.DTOs;
using eVOL.Application.UseCases.UCInterfaces.ISupportTicketCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVOL.API.Controllers
{
    [ApiController]
    [Route("api/support-ticket")]
    [Authorize(Roles = "User,Admin")]
    public class SupportTicketController : ControllerBase
    {
        private readonly ICreateSupportTicketUseCase _createSupportTicketUseCase;
        private readonly IDeleteSupportTicketUseCase _deleteSupportTicketUseCase;
        private readonly IGetSupportTicketByIdUseCase _getSupportTicketByIdUseCase;
        private readonly IClaimSupportTicketUseCase _claimSupportTicketUseCase;
        private readonly IUnClaimSupportTicketUseCase _unClaimSupportTicketUseCase;

        public SupportTicketController(ICreateSupportTicketUseCase createSupportTicketUseCase, IDeleteSupportTicketUseCase deleteSupportTicketUseCase, IGetSupportTicketByIdUseCase getSupportTicketByIdUseCase, IClaimSupportTicketUseCase claimSupportTicketUseCase, IUnClaimSupportTicketUseCase unClaimSupportTicketUseCase)
        {
            _createSupportTicketUseCase = createSupportTicketUseCase;
            _deleteSupportTicketUseCase = deleteSupportTicketUseCase;
            _getSupportTicketByIdUseCase = getSupportTicketByIdUseCase;
            _claimSupportTicketUseCase = claimSupportTicketUseCase;
            _unClaimSupportTicketUseCase = unClaimSupportTicketUseCase;
        }


        [HttpPost]
        public async Task<IActionResult> CreateSupportTicket(SupportTicketDTO dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supportTicket = await _createSupportTicketUseCase.ExecuteAsync(dto);

            if (supportTicket == null)
            {
                return BadRequest();
            }

            return Ok(supportTicket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupportTicket(int id)
        {
            var supportTicket = await _deleteSupportTicketUseCase.ExecuteAsync(id);

            if (supportTicket == null)
            {
                return NotFound();
            }

            return Ok(supportTicket);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupportTicketById(int id)
        {
            var supportTicket = await _getSupportTicketByIdUseCase.ExecuteAsync(id);

            if (supportTicket == null)
            {
                return NotFound();
            }

            return Ok(supportTicket);
        }

        [HttpPost("claim")]
        public async Task<IActionResult> ClaimSupportTicket([FromBody] ClaimSupportTicketDTO dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _claimSupportTicketUseCase.ExecuteAsync(dto);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("unclaim")]
        public async Task<IActionResult> UnClaimSupportTicket([FromBody] ClaimSupportTicketDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _unClaimSupportTicketUseCase.ExecuteAsync(dto);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
