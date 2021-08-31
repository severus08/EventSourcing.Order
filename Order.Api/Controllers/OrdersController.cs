using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Requests;
using Order.Application.Commands;
using Order.Application.Queries;
using Order.Domain.DomainSpecifications;
using Order.Domain.ReadModels;
using Order.Infrastructure.Command;
using Order.Infrastructure.Query;
using Order.Infrastructure.ReadModel;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;
        private readonly IQueryBus _queryBus;

        public OrdersController(ICommandBus commandBus, IMapper mapper,
            IQueryBus queryBus)
        {
            _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _queryBus = queryBus;
        }

        #region Orders

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, [FromQuery] bool? CourierAssignedFilter, bool? QuietDeliveryFilter)
        {
            var getOrderQuery = new GetOrderQuery
            {
                OrderId = id,
                SpecificationList = new List<Specification<OrderReadModel>>()
            };
            if (CourierAssignedFilter.GetValueOrDefault())
            {
                getOrderQuery.SpecificationList.Add(new IsCourierAssigned());
            }
            if (QuietDeliveryFilter.GetValueOrDefault())
            {
                getOrderQuery.SpecificationList.Add(new IsQuietDelivery());
            }

            var order = await _queryBus.Send<GetOrderQuery,OrderReadModel>(getOrderQuery);
            if (order == default)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(bool? CourierAssignedFilter, bool? QuietDeliveryFilter)
        {
            var getOrderListQuery = new GetOrderListQuery()
            {
                SpecificationList = new List<Specification<OrderReadModel>>()
            };
            if (CourierAssignedFilter.GetValueOrDefault())
            {
                getOrderListQuery.SpecificationList.Add(new IsCourierAssigned());
            }
            if (QuietDeliveryFilter.GetValueOrDefault())
            {
                getOrderListQuery.SpecificationList.Add(new IsQuietDelivery());
            }
            var orders = await _queryBus.Send<GetOrderListQuery, List<OrderReadModel>>(getOrderListQuery);
            
            return Ok(orders);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrderRequest request)
        {
            request.FillMetaData(Request.Headers);
            var command = _mapper.Map<CreateOrderCommand>(request);
            await _commandBus.Send(command);

            return Ok();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancelOrderRequest request)
        {
            request.FillMetaData(Request.Headers);
            var command = _mapper.Map<CancelOrderCommand>(request);
            command.OrderId = id;
            await _commandBus.Send(command);

            return Ok();
        }

        #endregion

        #region Payments

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{id}/payments")]
        public async Task<IActionResult> Post(Guid id, [FromBody] PaymentApproveRequest request)
        {
            request.FillMetaData(Request.Headers);
            var command = _mapper.Map<PaymentApproveCommand>(request);
            command.OrderId = id;
            await _commandBus.Send(command);

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("{id}/payments")]
        public async Task<IActionResult> PaymentDelete(Guid id, [FromBody] PaymentRejectRequest request)
        {
            request.FillMetaData(Request.Headers);
            var command = _mapper.Map<PaymentRejectCommand>(request);
            command.OrderId = id;
            await _commandBus.Send(command);

            return Ok();
        }

        #endregion

        #region Shipment

        [HttpPost("{id}/shipments")]
        public async Task<IActionResult> Post(Guid id, [FromBody] ShipmentStartRequest request)
        {
            request.FillMetaData(Request.Headers);
            var command = _mapper.Map<ShipmentStartCommand>(request);
            command.OrderId = id;
            await _commandBus.Send(command);

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}/shipments")]
        public async Task<IActionResult> Delete(Guid id, [FromBody] ShipmentFinishRequest request)
        {
            request.FillMetaData(Request.Headers);
            var command = _mapper.Map<ShipmentFinishCommand>(request);
            command.OrderId = id;
            await _commandBus.Send(command);

            return Ok();
        }

        #endregion
    }
}