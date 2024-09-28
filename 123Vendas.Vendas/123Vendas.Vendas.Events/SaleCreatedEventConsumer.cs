using _123Vendas.Vendas.Domain.Events;
using _123Vendas.Vendas.Domain.Interfaces.Services;
using _123Vendas.Vendas.Domain.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace _123Vendas.Vendas.Events
{
    public class SaleCreatedEventConsumer : IConsumer<SaleCreatedEvent>
    {
        private readonly ILogger<SaleCreatedEventConsumer> _logger;
        private readonly ISaleService _saleService;

        public SaleCreatedEventConsumer(
            ILogger<SaleCreatedEventConsumer> logger,
            ISaleService saleService
        )
        {
            _logger = logger;
            _saleService = saleService;
        }


        private InsertOrUpdateSaleModel _ToModel(SaleCreatedEvent @event) =>
            new InsertOrUpdateSaleModel
            {
                BranchId = @event.BranchId,
                CompanyId = @event.CompanyId,
                CustomerId = @event.CustomerId,
                SaleCode = @event.SaleCode,
                SaleDate = @event.SaleDate,
                SalerId = @event.SalerId,

                Products = @event.Products?.Select(_ => new InsertUpdateOrDeleteSaleProductModel
                {
                    Amount = _.Amount,
                    Discount = _.Discount,
                    ProductId = _.ProductId,
                    Quantity = _.Quantity
                })?
                .ToHashSet()
            };

        public async Task Consume(ConsumeContext<SaleCreatedEvent> context)
        {
            _logger.LogInformation("Sale created event received.");

            var message = context.Message;

            try
            {
                var model = _ToModel(message);
                var result = await _saleService.InsertAsync(model, message.CreatedBy);

                await _saleService.SaveChangesAsync();

                if (result != Guid.Empty)
                    _logger.LogInformation("Sale ID {0} created", result);
                else
                    _logger.LogWarning("Sale not created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to create sale");
            }
        }
    }
}
