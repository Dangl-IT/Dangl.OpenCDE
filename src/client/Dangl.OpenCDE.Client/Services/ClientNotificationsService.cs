using Dangl.OpenCDE.Client.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Services
{
    public class ClientNotificationsService
    {
        private readonly IHubContext<CdeClientHub> _hubContext;
        
        public ClientNotificationsService(IHubContext<CdeClientHub> hubContext)
        {
            _hubContext = hubContext;

        }

        public Task SendInformationToClientAsync(string message)
        {
            return SendHubMessage(false, message);
        }

        public Task SendErrorToClientAsync(string message)
        {
            return SendHubMessage(false, message);
        }

        private Task SendHubMessage(bool isError, string message)
        {
            return _hubContext
                    .Clients
                    .All
                    .SendAsync("NotificationMessage", new
                    {
                        isError,
                        message
                    });
        }
    }
}
