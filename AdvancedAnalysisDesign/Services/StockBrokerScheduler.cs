using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdvancedAnalysisDesign.Services
{
    internal class BloodworkScheduler : BackgroundService
    {
        public IServiceProvider Services { get; }
    
        public BloodworkScheduler(IServiceProvider services)
        {
            Services = services;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }
        
        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService = 
                    scope.ServiceProvider
                        .GetRequiredService<IScopedProcessingService>();

                await scopedProcessingService.CheckIfBloodworkIsRequired(stoppingToken);
            }
        }
    }
}