using eVOL.Application.DTOs;
using eVOL.Application.UseCases.UCInterfaces.IChatGroupCases;
using eVOL.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVOL.API.Controllers
{
    [ApiController]
    [Route("api/chat-group")]
    [Authorize(Roles = "User,Admin")]
    public class ChatGroupController : ControllerBase
    {
        private readonly ICreateChatGroupUseCase _createChatGroupUseCase;
        private readonly IDeleteChatGroupUseCase _deleteChatGroupUseCase;
        private readonly IGetChatGroupByIdUseCase _getChatGroupByIdUseCase;
        private readonly ITransferOwnershipOfChatGroupUseCase _transferOwnershipOfChatGroupUseCase;

        public ChatGroupController(ICreateChatGroupUseCase createChatGroupUseCase, 
            IDeleteChatGroupUseCase deleteChatGroupUseCase, 
            IGetChatGroupByIdUseCase getChatGroupByIdUseCase, 
            ITransferOwnershipOfChatGroupUseCase transferOwnershipOfChatGroupUseCase)
        {
            _createChatGroupUseCase = createChatGroupUseCase;
            _deleteChatGroupUseCase = deleteChatGroupUseCase;
            _getChatGroupByIdUseCase = getChatGroupByIdUseCase;
            _transferOwnershipOfChatGroupUseCase = transferOwnershipOfChatGroupUseCase;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChatGroup(ChatGroupDTO dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var chatGroup = await _createChatGroupUseCase.ExecuteAsync(dto);

            if (chatGroup == null)
            {
                return BadRequest();
            }

            return Ok(chatGroup);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteChatGroup([FromBody] DeleteChatGroupDTO dto)
        {
            var chatGroup = await _deleteChatGroupUseCase.ExecuteAsync(dto.ChatGroupId, dto.ChatGroupOwnerId);

            if (chatGroup == null)
            {
                return NotFound();
            }

            return Ok(chatGroup);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChatGroupById(int id)
        {
            var chatGroup = await _getChatGroupByIdUseCase.ExecuteAsync(id);

            if (chatGroup == null)
            {
                return NotFound();
            }

            return Ok(chatGroup);
        }

        [HttpPut("transfer")]
        public async Task<IActionResult> TransferOwnershipOfChatGroup(TransferOwnershipOfCGDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var chatGroup = await _transferOwnershipOfChatGroupUseCase.ExecuteAsync(dto.CurrentOwnerId, dto.NewOwnerId, dto.ChatGroupId);

            if (chatGroup == null)
            {
                return NotFound();
            }

            return Ok(chatGroup);


        }

    }
}
