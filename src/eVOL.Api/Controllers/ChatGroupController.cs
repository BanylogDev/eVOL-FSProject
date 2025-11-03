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
        private readonly IAddUserToChatGroupUseCase _addUserToChatGroupUseCase;
        private readonly IRemoveUserFromChatGroupUseCase _removeUserFromChatGroupUseCase;

        public ChatGroupController(ICreateChatGroupUseCase createChatGroupUseCase, IDeleteChatGroupUseCase deleteChatGroupUseCase, IGetChatGroupByIdUseCase getChatGroupByIdUseCase, IAddUserToChatGroupUseCase addUserToChatGroupUseCase, IRemoveUserFromChatGroupUseCase removeUserFromChatGroupUseCase)
        {
            _createChatGroupUseCase = createChatGroupUseCase;
            _deleteChatGroupUseCase = deleteChatGroupUseCase;
            _getChatGroupByIdUseCase = getChatGroupByIdUseCase;
            _addUserToChatGroupUseCase = addUserToChatGroupUseCase;
            _removeUserFromChatGroupUseCase = removeUserFromChatGroupUseCase;
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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteChatGroup(int id)
        {
            var chatGroup = await _deleteChatGroupUseCase.ExecuteAsync(id);

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

    }
}
