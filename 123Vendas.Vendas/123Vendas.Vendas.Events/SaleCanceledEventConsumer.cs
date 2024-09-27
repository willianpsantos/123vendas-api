using _123Vendas.Vendas.Domain.Events;
using _123Vendas.Vendas.Domain.Interfaces.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace _123Vendas.Vendas.Events
{
    public class SaleCanceledEventConsumer : IConsumer<SaleCanceledEvent>
    {
        private readonly ILogger<SaleCanceledEventConsumer> _logger;
        private readonly ISaleService _saleService;

        public SaleCanceledEventConsumer(
            ILogger<SaleCanceledEventConsumer> logger,
            ISaleService saleService
        )
        {
            _logger = logger;
            _saleService = saleService;
        }

        public async Task Consume(ConsumeContext<SaleCanceledEvent> context)
        {
            _logger.LogInformation("Sale canceled event received.");

            var message = context.Message;

            try
            {                
                var result = await _saleService.CancelAsync(message.SaleId, message.CanceledBy);

                await _saleService.SaveChangesAsync();

                if (result)
                    _logger.LogInformation("Sale ID {0} canceled", message.SaleId);
                else
                    _logger.LogWarning("Sale ID {0} not found", message.SaleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to cancel sale ID {0}", message.SaleId);
            }
        }
    }
}
