using eVOL.Application.UseCases.UCInterfaces.IChatGroupCases;
using Microsoft.AspNetCore.SignalR;

namespace eVOL.API.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IAddUserToChatGroupUseCase _addUserToChatGroupUseCase;

        public ChatHub(IAddUserToChatGroupUseCase addUserToChatGroupUseCase)
        {
            _addUserToChatGroupUseCase = addUserToChatGroupUseCase;
        }

        public async Task JoinGroup(string groupName, int userId)
        {

            await _addUserToChatGroupUseCase.ExecuteAsync(userId, groupName);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", userId);
        }

        public async Task LeaveGroup(string groupName, string user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user);
        }

        public async Task SendGroupMessage(string groupName, string user, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        }
    }
}
