using eVOL.Application.UseCases.UCInterfaces.IChatGroupCases;
using eVOL.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace eVOL.API.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IAddUserToChatGroupUseCase _addUserToChatGroupUseCase;
        private readonly IRemoveUserFromChatGroupUseCase _removeUserFromChatGroupUseCase;
        private readonly ISendChatGroupMessageUseCase _sendChatGroupMessageUseCase;
        private readonly ILeaveChatGroupUseCase _leaveChatGroupUseCase;

        public ChatHub(IAddUserToChatGroupUseCase addUserToChatGroupUseCase, 
            IRemoveUserFromChatGroupUseCase removeUserFromChatGroupUseCase, 
            ISendChatGroupMessageUseCase sendChatGroupMessageUseCase, 
            ILeaveChatGroupUseCase leaveChatGroupUseCase)
        {
            _addUserToChatGroupUseCase = addUserToChatGroupUseCase;
            _removeUserFromChatGroupUseCase = removeUserFromChatGroupUseCase;
            _sendChatGroupMessageUseCase = sendChatGroupMessageUseCase;
            _leaveChatGroupUseCase = leaveChatGroupUseCase;
        }

        public async Task AddUserToGroup(string groupName, int userId)
        {

            var user = await _addUserToChatGroupUseCase.ExecuteAsync(userId, groupName);

            if (user == null)
            {
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", user.Name, new ChatMessage
            {
                Text = $"{user.Name} has joined the group!",
                CreatedAt = DateTime.Now,
            });
        }

        public async Task LeaveGroup(string groupName, int userId)
        {
            var user = await _leaveChatGroupUseCase.ExecuteAsync(userId, groupName);

            if (user == null)
            {
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", user.Name, new ChatMessage
            {
                Text = $"{user.Name} has left the group!",
                CreatedAt = DateTime.Now,
            });
        }

        public async Task RemoveUserFromGroup(string groupName, int userId)
        {
            var user = await _removeUserFromChatGroupUseCase.ExecuteAsync(userId, groupName);

            if (user == null)
            {
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", user.Name, new ChatMessage
            {
                Text = $"{user.Name} has left the group!",
                CreatedAt = DateTime.Now,
            });
        }

        public async Task SendGroupMessage(string groupName, int userId, string message)
        {
            (ChatMessage? newMessage, User? user) = await _sendChatGroupMessageUseCase.ExecuteAsync(message, groupName, userId);

            if (newMessage == null || user == null)
            {
                return;
            }

            await Clients.Group(groupName).SendAsync("ReceiveGroupCustomMessage", user.Name, newMessage);
        }
    }
}
