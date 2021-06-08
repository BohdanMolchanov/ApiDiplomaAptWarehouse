using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL;
using Diploma.Apt.Warehouse.Core.Data.Enums;
using Diploma.Apt.Warehouse.Core.Data.Helpers;
using Diploma.Apt.Warehouse.Core.Models;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;

namespace Diploma.Apt.Warehouse.Core.Controllers
{
    [Route("warehouse")]
    public class WarehouseController : Controller
    {
        private readonly WarehouseContext _context;
        private readonly IMapper _mapper;
        public WarehouseController(WarehouseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("stock")]
        public async Task<IActionResult> GetStocksAsync([FromQuery]GetWarehouseStocksRequestModel request)
        {
            var result = await _context.Stocks.AsQueryable()
                .Include(x => x.Product)
                .Where(x => x.DepartmentId == request.DepartmentId)
                .Skip(request.Skip)
                .Take(request.Limit)
                .OrderByDescending(o => o.LastUpdatedAt)
                .ThenBy(t => t.TableKey)
                .ToListAsync();
            var response = new List<StockResponseModel>();
            foreach (var entity in result)
            {
                var sum = await _context.Batches.AsQueryable().Where(x => x.StockId == entity.Id && x.IsRecieved)
                    .Select(x => x.Count).SumAsync();
                
                var lastOrder = await _context.Batches.AsQueryable().Where(x => x.StockId == entity.Id && x.IsRecieved)
                    .OrderByDescending(o => o.TableKey)
                    .Select(x => x.BestBefore)
                    .FirstOrDefaultAsync();
                entity.Status = RecountStockState(entity);
                response.Add(new StockResponseModel()
                {
                    Id = entity.Id,
                    Count = sum,
                    Name = entity.Product.NameUkr,
                    Details = entity.Product.Description,
                    Status = entity.Status,
                    BestBefore = lastOrder.HasValue ? lastOrder.Value.ToString("dd-MM-yyyy") : "",
                    MaxCount = entity.MaxCount,
                    OrderPoint = entity.OrderPoint,
                    OrderRepeat = entity.OrderPoint.HasValue ? entity.OrderPoint.Value.ToString("dd-MM-yyyy") : "",
                    PurchasePrice = entity.PurchasePrice,
                    SellPrice = entity.SellPrice,
                    TableKey = entity.TableKey
                });
            }
            /*var response = _mapper.Map<StockResponseModel>(result);*/
            
            if (response.Any()) return Ok(response);
            return NoContent();
        }
        [HttpGet("products")]
        public async Task<IActionResult> GetProductNamesAsync([FromQuery] string search)
        {
            if (string.IsNullOrEmpty(search) || search.Length < 3) return NoContent();
            var result = await _context.Products.AsQueryable()
                .Where(x => x.SearchVector.Matches(EF.Functions.ToTsQuery(TsVectorHelper.ToTsQueryString(search))))
                .Take(20)
                .OrderBy(o => o.NameUkr)
                .ThenByDescending(t => t.TableKey)
                .ToListAsync();
            var response = result.Select(productEntity => new ProductSearchResponseModel()
            {
                ProductId = productEntity.Id, 
                Name = $"{productEntity.NameUkr} {TextHelper.GetShortString(productEntity.Description, 80)}"
            }).ToList();

            if (response.Any()) return Ok(response);
            return NoContent();
        }
        
        [HttpPost("product/create")]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductRequestModel request)
        {
            if (request == null) return BadRequest();
            var entity = _mapper.Map<ProductEntity>(request);
            entity.Id = Guid.NewGuid();
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
            return Accepted();
        }
        
        [HttpPost("stock/create")]
        public async Task<IActionResult> CreateStockAsync([FromBody] CreateStockRequestModel request)
        {
            if (request == null) return BadRequest();
            var newStock = await _context.Stocks.AsQueryable().AnyAsync(x => x.ProductId == request.ProductId);
            if (!newStock) return BadRequest("Запас із таким препаратом уже доданий до системи");
            var entity = _mapper.Map<StockEntity>(request);
            entity.Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId);
            if (entity.Product == null)
            {
                return BadRequest("Product with such Id not found");
            }
            entity.Id = Guid.NewGuid();
            entity.OrganizationId = Guid.Parse("37c92016-1b2d-4572-9b07-5d40af3292fc");
            entity.DepartmentId = Guid.Parse("8bcfaed8-e453-4e85-88c5-212b103e5516");
            await _context.Stocks.AddAsync(entity);
            await _context.SaveChangesAsync();
            return Accepted();
        }
        
        [HttpGet("batches")]
        public async Task<IActionResult> GetBatchesAsync([FromQuery] GetWarehouseStocksRequestModel request)
        {
            if (request == null) return BadRequest();
            var query = await _context.Batches.AsQueryable()
                .Include(x => x.Stock)
                .ThenInclude(x => x.Product)
                .Where(x => x.DepartmentId == request.DepartmentId)
                .Skip(request.Skip)
                .Take(request.Limit)
                .OrderByDescending(o => o.LastUpdatedAt)
                .ThenBy(t => t.TableKey)
                .ToListAsync();
            
            var response = query.Select(entity => new BatchResponseModel()
                {
                    Count = entity.Count,
                    Details = TextHelper.GetShortString(entity.Stock.Product.Description, 40),
                    Id = entity.Id,
                    Name = entity.Stock.Product.NameUkr,
                    Provider = entity.ProviderName,
                    Status = entity.Status,
                    BestBefore = entity.BestBefore.HasValue ? entity.BestBefore.Value.ToString("dd-MM-yyyy") : "",
                    CreatedAt = entity.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                    RecievedAt = entity.LastUpdatedAt.HasValue ? entity.LastUpdatedAt.Value.ToString("dd-MM-yyyy HH:mm") : "",
                    TableKey = entity.TableKey
                })
                .ToList();

            if (response.Any()) return Ok(response);
            return NoContent();
        }
        
        [HttpPost("batch/create")]
        public async Task<IActionResult> CreateBatchAsync([FromBody] CreateBatchRequestModel request)
        {
            if (request == null) return BadRequest();
            var entity = _mapper.Map<BatchEntity>(request);
            entity.DepartmentId = Guid.Parse("8bcfaed8-e453-4e85-88c5-212b103e5516");
            entity.Stock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == request.StockId);
            if (entity.Stock == null)
            {
                return BadRequest("Stock with such Id not found");
            }
            entity.Id = Guid.NewGuid();
            await _context.Batches.AddAsync(entity);
            await _context.SaveChangesAsync();
            return Accepted();
        }
        [HttpPatch("batch/confirm")]
        public async Task<IActionResult> ConfirmBatchAsync([FromBody] ConfirmBatchRequestModel request)
        {
            if (request == null) return BadRequest();
            var entity = await _context.Batches.FirstOrDefaultAsync(x => x.Id == request.BatchId);
            if (entity == null)
            {
                return NotFound("Batch such Id not found");
            }

            entity.IsRecieved = true;
            entity.BestBefore = request.BestBefore;
            entity.Status = BatchStates.Supplied;
            _context.Batches.Update(entity);
            //recount stock counts
            var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == entity.StockId);
            stock.Count = await _context.Batches.AsQueryable().Where(x => x.StockId == entity.Id && x.IsRecieved)
                .Select(x => x.Count).SumAsync();
            stock.Status = RecountStockState(stock);
            _context.Stocks.Update(stock);
            await _context.SaveChangesAsync();
            return Accepted();
        }

        [HttpGet("stock/names")]
        public async Task<IActionResult> GetStockNamesAsync([FromQuery] string search)
        {
            var departmentId = Guid.Parse("8bcfaed8-e453-4e85-88c5-212b103e5516");
            if (string.IsNullOrEmpty(search) || search.Length < 3) return NoContent();
            var result = await _context.Stocks.AsQueryable()
                .Include(x => x.Product)
                .Where(x => x.DepartmentId == departmentId &&
                    x.Product.SearchVector.Matches(EF.Functions.ToTsQuery(TsVectorHelper.ToTsQueryString(search))))
                .Take(20)
                .OrderBy(o => o.Product.NameUkr)
                .ThenByDescending(t => t.TableKey)
                .ToListAsync();
            var response = result.Select(stockEntity => new StockNamesResponseModel()
            {
                StockId = stockEntity.Id, 
                Name = $"#{stockEntity.TableKey}, {stockEntity.Product.NameUkr}, {TextHelper.GetShortString(stockEntity.Product.Description, 80)}"
            }).ToList();

            if (response.Any()) return Ok(response);
            return NoContent();
        }


        private static StockStates RecountStockState(StockEntity stock)
        {
            return stock.OrderPoint.HasValue && stock.OrderPoint >= stock.Count || 
                   stock.OrderRepeat.HasValue && stock.OrderRepeat >= DateTime.Today 
                ? StockStates.NeedsOrder 
                : stock.Count == 0 ? StockStates.New : StockStates.Ok;
        }
    }
}