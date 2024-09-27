using _123Vendas.Vendas.Domain.Events;
using _123Vendas.Vendas.Domain.Interfaces.Services;
using _123Vendas.Vendas.Domain.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace _123Vendas.Vendas.Events
{
    public class SaleUpdatedEventConsumer : IConsumer<SaleUpdatedEvent>
    {
        private readonly ILogger<SaleUpdatedEventConsumer> _logger;
        private readonly ISaleService _saleService;

        public SaleUpdatedEventConsumer(
            ILogger<SaleUpdatedEventConsumer> logger,
            ISaleService saleService
        )
        {
            _logger = logger;
            _saleService = saleService;
        }

        private InsertOrUpdateSaleModel _ToModel(SaleUpdatedEvent @event) =>
            new InsertOrUpdateSaleModel
            {
                BranchId = @event.BranchId,
                CompanyId = @event.CompanyId,
                CustomerId = @event.CustomerId,
                SaleCode = @event.SaleCode,
                SaleDate = @event.SaleDate,
                SalerId = @event.SalerId,

                Products = @event.Products?.Select(_ => new InsertOrUpdateSaleProductModel
                {
                    Id = _.Id ?? Guid.Empty,
                    SaleId = @event.Id,
                    Amount = _.Amount,
                    Discount = _.Discount,
                    ProductId = _.ProductId,
                    Quantity = _.Quantity
                })
            };

        public async Task Consume(ConsumeContext<SaleUpdatedEvent> context)
        {
            _logger.LogInformation("Sale created event received.");

            var message = context.Message;

            try
            {
                var model = _ToModel(message);
                var result = _saleService.Update(message.Id, model, message.UpdatedBy);
                var affected = await _saleService.SaveChangesAsync();

                if (affected > 0)
                    _logger.LogInformation("Sale ID {0} updated", message.Id);
                else
                    _logger.LogWarning("Sale not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to update sale ID {0}", message.Id);
            }
        }
    }
}
