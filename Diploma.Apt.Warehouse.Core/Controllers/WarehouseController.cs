using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL;
using Diploma.Apt.Warehouse.Core.Data.Enums;
using Diploma.Apt.Warehouse.Core.Data.Helpers;
using Diploma.Apt.Warehouse.Core.Enums;
using Diploma.Apt.Warehouse.Core.Models;
using Diploma.Apt.Warehouse.Core.Models.RequestModels;
using Diploma.Apt.Warehouse.Core.Models.ResponseModels;
using Diploma.Apt.Warehouse.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;

namespace Diploma.Apt.Warehouse.Core.Controllers
{
    [Route("warehouse"), Authorize(Roles = RoleTypes.WarehouseManager)]
    public class WarehouseController : Controller
    {
        private readonly WarehouseContext _context;
        private readonly UsersRepository _usersRepository;
        private readonly DepartmentsRepository _departmentsRepository;
        private readonly IMapper _mapper;

        public WarehouseController(UserContext userContext, WarehouseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _usersRepository = new UsersRepository(userContext, mapper);
            _departmentsRepository = new DepartmentsRepository(userContext, mapper);
        }

        [HttpGet("stock")]
        public async Task<IActionResult> GetStocksAsync([FromQuery] GetWarehouseStocksRequestModel request)
        {
            var currentUserId = Guid.Parse(User.Identity.Name);
            var user = await _usersRepository.GetOneAsync(currentUserId);
            if (user == null) return BadRequest(new {message = "User does not exist"});
            var userDepartment = await _departmentsRepository.GetOneAsync(user.DepartmentId.Value);
            if (userDepartment == null) return BadRequest(new {message = "Department does not exist"});
            request.DepartmentId = userDepartment.Id;

            var result = await _context.Stocks.AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.DepartmentId == request.DepartmentId && 
                        (
                            string.IsNullOrEmpty(request.Status) || (
                                request.Status == "new" && x.Status == StockStates.New ||
                                request.Status == "ok" && x.Status == StockStates.Ok ||
                                request.Status == "needsOrder" && x.Status == StockStates.NeedsOrder
                                )
                        )
                        &&
                        (
                            string.IsNullOrEmpty(request.Search) ||
                            x.Product.SearchVector.Matches(
                                EF.Functions.ToTsQuery(TsVectorHelper.ToTsQueryString(request.Search))
                            )
                        )
                    
                )
                .Skip(request.Skip)
                .Take(request.Limit)
                .OrderByDescending(o => o.LastUpdatedAt)
                .ThenBy(t => t.TableKey)
                .ToListAsync();
            var response = new List<StockResponseModel>();
            foreach (var entity in result)
            {
                var lastOrder = await _context.Batches.AsNoTracking().Where(x => x.StockId == entity.Id && x.IsRecieved)
                    .OrderByDescending(o => o.TableKey)
                    .FirstOrDefaultAsync();
                response.Add(new StockResponseModel()
                {
                    Id = entity.Id,
                    Count = entity.Count,
                    Name = entity.Product.NameUkr,
                    Details = entity.Product.Description,
                    Status = entity.Status.ToString(),
                    BestBefore = lastOrder?.BestBefore != null
                        ? lastOrder.BestBefore.Value.ToLocalTime().ToString("dd.MM.yyyy")
                        : "",
                    MaxCount = entity.MaxCount,
                    OrderPoint = entity.OrderPoint,
                    OrderRepeat = entity.OrderRepeat.HasValue ? entity.OrderRepeat.Value.ToString("dd.MM.yyyy") : "",
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
            if (string.IsNullOrEmpty(search)) return BadRequest();
            var result = await _context.Products.AsNoTracking()
                .Where(x => x.SearchVector.Matches(EF.Functions.ToTsQuery(TsVectorHelper.ToTsQueryString(search))))
                .OrderBy(o => o.NameUkr)
                .ThenByDescending(t => t.TableKey)
                .Take(20)
                .ToListAsync();
            var response = result.Select(productEntity => new ProductSearchResponseModel()
            {
                ProductId = productEntity.Id,
                Name = $"{productEntity.NameUkr} {TextHelper.GetShortString(productEntity.Description, 80)}"
            }).ToList();

            if (response.Any()) return Ok(response);
            return NoContent();
        }

        [HttpGet("batches")]
        public async Task<IActionResult> GetBatchesAsync([FromQuery] GetWarehouseStocksRequestModel request)
        {
            if (request == null) return BadRequest();
            var currentUserId = Guid.Parse(User.Identity.Name);
            var user = await _usersRepository.GetOneAsync(currentUserId);
            if (user == null) return BadRequest(new {message = "User does not exist"});
            var userDepartment = await _departmentsRepository.GetOneAsync(user.DepartmentId.Value);
            if (userDepartment == null) return BadRequest(new {message = "Department does not exist"});
            request.DepartmentId = userDepartment.Id;

            var query = await _context.Batches.AsNoTracking()
                .Include(x => x.Stock)
                .ThenInclude(x => x.Product)
                .Where(x => x.DepartmentId == request.DepartmentId &&
                            (
                                (string.IsNullOrEmpty(request.Status) ||
                                 request.Status == "new" &&
                                 x.Status == BatchStates.New ||
                                 request.Status == "supplied" &&
                                 x.Status == BatchStates.Supplied
                                )
                                &&
                                (string.IsNullOrEmpty(request.Search) ||
                                 x.Stock.Product.SearchVector.Matches(
                                     EF.Functions.ToTsQuery(TsVectorHelper.ToTsQueryString(request.Search))
                                 ))
                            )
                )
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
                    BestBefore = entity.BestBefore.HasValue ? entity.BestBefore.Value.ToString("dd.MM.yyyy") : "",
                    CreatedAt = entity.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                    RecievedAt = entity.ReceivedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                    TableKey = entity.TableKey
                })
                .ToList();

            if (response.Any()) return Ok(response);
            return NoContent();
        }

        [HttpGet("stock/names")]
        public async Task<IActionResult> GetStockNamesAsync([FromQuery] string search)
        {
            if (string.IsNullOrEmpty(search)) return BadRequest();
            var currentUserId = Guid.Parse(User.Identity.Name);
            var user = await _usersRepository.GetOneAsync(currentUserId);
            if (user == null) return BadRequest(new {message = "User does not exist"});
            var userDepartment = await _departmentsRepository.GetOneAsync(user.DepartmentId.Value);
            if (userDepartment == null) return BadRequest(new {message = "Department does not exist"});
            var departmentId = userDepartment.Id;
            if (string.IsNullOrEmpty(search) || search.Length < 3) return NoContent();
            var result = await _context.Stocks.AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.DepartmentId == departmentId &&
                            x.Product.SearchVector.Matches(
                                EF.Functions.ToTsQuery(TsVectorHelper.ToTsQueryString(search))))
                .Take(20)
                .OrderBy(o => o.Product.NameUkr)
                .ThenByDescending(t => t.TableKey)
                .ToListAsync();
            var response = result.Select(stockEntity => new StockNamesResponseModel()
            {
                StockId = stockEntity.Id,
                Name =
                    $"#{stockEntity.TableKey}, {stockEntity.Product.NameUkr}, {TextHelper.GetShortString(stockEntity.Product.Description, 80)}"
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
            var currentUserId = Guid.Parse(User.Identity.Name);
            var user = await _usersRepository.GetOneAsync(currentUserId);
            if (user == null) return BadRequest(new {message = "User does not exist"});
            var userDepartment = await _departmentsRepository.GetOneAsync(user.DepartmentId.Value);
            if (userDepartment == null) return BadRequest(new {message = "Department does not exist"});

            var newStock = await _context.Stocks.AsNoTracking()
                .AnyAsync(x => x.ProductId == request.ProductId && x.DepartmentId == userDepartment.Id);
            if (newStock) return BadRequest("Запас із таким препаратом уже доданий до системи");
            var entity = _mapper.Map<StockEntity>(request);
            entity.Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId);
            if (entity.Product == null)
            {
                return BadRequest("Product with such Id not found");
            }

            entity.Id = Guid.NewGuid();
            entity.OrganizationId = userDepartment.OrganizationId;
            entity.DepartmentId = userDepartment.Id;
            await _context.Stocks.AddAsync(entity);
            await _context.SaveChangesAsync();
            return Accepted();
        }


        [HttpPost("batch/create")]
        public async Task<IActionResult> CreateBatchAsync([FromBody] CreateBatchRequestModel request)
        {
            if (request == null) return BadRequest();

            var currentUserId = Guid.Parse(User.Identity.Name);
            var user = await _usersRepository.GetOneAsync(currentUserId);
            if (user == null) return BadRequest(new {message = "User does not exist"});
            var userDepartment = await _departmentsRepository.GetOneAsync(user.DepartmentId.Value);
            if (userDepartment == null) return BadRequest(new {message = "Department does not exist"});
            var entity = _mapper.Map<BatchEntity>(request);
            entity.DepartmentId = userDepartment.Id;
            entity.Stock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == request.StockId);
            if (entity.Stock == null)
            {
                return BadRequest("Stock with such Id not found");
            }

            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.Now.ToUniversalTime();
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
            entity.ReceivedAt = DateTime.Now.ToUniversalTime();
            _context.Batches.Update(entity);
            await _context.SaveChangesAsync();
            //recount stock counts
            var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == entity.StockId);
            stock.Count = await _context.Batches.Where(x => x.StockId == stock.Id && x.IsRecieved)
                .Select(x => x.Count).SumAsync();
            stock.OrderRepeat = entity.ReceivedAt.AddDays(stock.OrderPeriod.Value);
            stock.Status = RecountStockState(stock);
            _context.Stocks.Update(stock);
            await _context.SaveChangesAsync();
            return Accepted();
        }


        private static StockStates RecountStockState(StockEntity stock)
        {
            return stock.OrderPoint.HasValue && stock.OrderPoint >= stock.Count ||
                   stock.OrderRepeat.HasValue && stock.OrderRepeat.Value.Date <= DateTime.Now.Date
                ? StockStates.NeedsOrder
                : stock.Count == 0
                    ? StockStates.New
                    : StockStates.Ok;
        }
    }
}