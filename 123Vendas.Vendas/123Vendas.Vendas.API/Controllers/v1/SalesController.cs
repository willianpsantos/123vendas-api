using _123Vendas.Vendas.Domain.Interfaces.Services;
using _123Vendas.Vendas.Domain.Models;
using _123Vendas.Vendas.Domain.Queries;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace _123Vendas.Vendas.API.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly ISaleService _salesService;
        private readonly IValidator<InsertOrUpdateSaleModel> _saleModelValidator;

        public SalesController(
            ILogger<SalesController> logger,
            ISaleService salesService,
            IValidator<InsertOrUpdateSaleModel> saleModelValidator
        )
        {
            _logger = logger;
            _salesService = salesService;
            _saleModelValidator = saleModelValidator;
        }


        /// <summary>
        /// Filter sales based on given query criterias.
        /// </summary>
        /// <param name="query"> The criterias used to filter sales (Optional). </param>
        /// <returns> A list of sales and their products. </returns>
        /// <response code="200"> Success - return a list of sales </response>
        /// <response code="500"> Error - return the error that occurs during the process. </response>
        [HttpGet()]        
        public async Task<IActionResult> GetAsync([FromQuery] SaleQuery? query = null)
        {
            try
            {
                var sales = await _salesService.GetAsync(query);

                _logger.LogInformation("Sales got!");

                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to get sales");

                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex);
            }
        }

        /// <summary>
        /// Filter sales based on given query criterias and paginate those results.
        /// </summary>
        /// <param name="page"> The number of page. </param>
        /// <param name="pageSize"> The quantity of items per page </param>
        /// <returns> The total count of records, the page number and size, and the list of sales and their products. </returns>
        /// <response code="200"> Success - return a list of sales </response>
        /// <response code="500"> Error - return the error that occurs during the process. </response>
        [HttpGet("/{page}/{pageSize}/paginated")]
        public async Task<IActionResult> GetPaginatedASync(int page, int pageSize, [FromQuery] SaleQuery? query = null)
        {
            try
            {
                var sales = await _salesService.GetPaginatedAsync(page, pageSize, query);

                _logger.LogInformation("Paginated sales got!");

                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to get sales paginated");

                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex);
            }
        }

        /// <summary>
        /// Gets a sale by the ID.
        /// </summary>
        /// <param name="id"> The ID of sale </param>
        /// <returns> The sale and its products. </returns>
        /// <response code="200"> Success - return the sale </response>
        /// <response code="400"> BadRequest - The ID is empty. </response>
        /// <response code="500"> Error - return the error that occurs during the process. </response>
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid uuid))
                return BadRequest();

            try
            {
                var sale = await _salesService.GetByIdAsync(uuid);

                return Ok(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to get sale ID {0}", id);

                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex);
            }
        }

        /// <summary>
        /// Inserts a new sale.
        /// </summary>
        /// <param name="model"> The information about the sale that will be inserted. </param>
        /// <returns> The inserted sale. </returns>
        /// <response code="200"> Success - The sale was inserted succeded, </response>
        /// <response code="400"> BadRequest - There's no information given or some validation error happened. </response>
        /// <response code="500"> Error - return the error that occurs during the process. </response>
        [HttpPost()]
        public async Task<IActionResult> PostAsync([FromBody] InsertOrUpdateSaleModel? model)
        {
            if (model is null)
                return BadRequest();

            var validationResult = _saleModelValidator.Validate(model);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                var saleId = await _salesService.InsertAsync(model, Guid.NewGuid());

                await _salesService.SaveChangesAsync();

                _logger.LogInformation("Sale ID {0} inserted", saleId);

                return Ok(saleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to insert new sale");

                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex);
            }
        }

        /// <summary>
        /// Updates a existing sale.
        /// </summary>
        /// <param name="id"> The to be updated sale ID. </param>
        /// <param name="model"> The information about the sale that will be updated. </param>
        /// <returns> The updated sale. </returns>
        /// <response code="200"> Success - The sale was updated succeded. </response>
        /// <response code="400"> BadRequest - There's no information given or some validation error happened. </response>
        /// <response code="500"> Error - return the error that occurs during the process. </response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(string? id, [FromBody] InsertOrUpdateSaleModel? model)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid uuid))
                return BadRequest();

            if (model is null)
                return BadRequest();

            var validationResult = _saleModelValidator.Validate(model);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                var sale = _salesService.Update(uuid, model, Guid.NewGuid());

                await _salesService.SaveChangesAsync();

                _logger.LogInformation("Sale ID {0} updated", sale.Id);

                return Ok(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to update sale ID {0}", id);

                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex);
            }
        }

        /// <summary>
        /// Updates a existing sale.
        /// </summary>
        /// <param name="id"> The to be updated sale ID. </param>
        /// <param name="model"> The information about the sale that will be updated. </param>
        /// <returns> The updated sale. </returns>
        /// <response code="200"> Success - The sale was updated succeded. </response>
        /// <response code="400"> BadRequest - There's no information given or some validation error happened. </response>
        /// <response code="500"> Error - return the error that occurs during the process. </response>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelAsync(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid uuid))
                return BadRequest();
            
            try
            {
                var canceled = await _salesService.CancelAsync(uuid, Guid.NewGuid());

                await _salesService.SaveChangesAsync();

                _logger.LogInformation("Sale ID {0} canceled", id);

                return Ok(canceled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to cancel sale ID {0}", id);

                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex);
            }
        }

        /// <summary>
        /// Deletes a sale.
        /// </summary>
        /// <param name="id"> The to be deleted sale ID. </param>
        /// <returns> The updated sale. </returns>
        /// <response code="200"> Success - The sale was updated succeded. </response>
        /// <response code="400"> BadRequest - The ID is empty. </response>
        /// <response code="500"> Error - return the error that occurs during the process. </response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid uuid))
                return BadRequest();

            try
            {
                var sale = await _salesService.DeleteAsync(uuid, Guid.NewGuid());

                await _salesService.SaveChangesAsync();

                _logger.LogInformation("Sale ID {0} deleted", id);

                return Ok(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to delete sale ID {0}", id);

                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex);
            }
        }
    }
}
