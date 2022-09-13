using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Services
{
    public class OpenCdeFileUploadBackgroundService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public OpenCdeFileUploadBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_timer == null)
                {
                    _timer = new Timer(CheckQueueAndPerformUploadAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            _timer = null;
            return Task.CompletedTask;
        }

        private async void CheckQueueAndPerformUploadAsync(object state)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var cacheService = scope.ServiceProvider.GetRequiredService<OpenCdeUploadOperationsCache>();

                if (cacheService.UploadTasks.TryDequeue(out var queuedTask))
                {
                    var uploadService = scope.ServiceProvider.GetRequiredService<OpenCdeUploadService>();
                    await uploadService.PerformUploadAsync(queuedTask);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }
    }
}
