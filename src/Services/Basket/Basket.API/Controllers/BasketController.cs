using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Basket.API.Entities;
using System.Net;
using Basket.API.GrpcServices;
using EventBus.Messages.Events;
using AutoMapper;
using MassTransit;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _discountGrpcService = discountGrpcService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;

        }
        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            //TODO: Communicate with Discount.Grpc 
            //and Calculate latest prices of product
            //Consume Discount gRPC
            foreach (var item in basket.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult>Checkout([FromBody] BasketCheckout basketCheckout)
        {
            //Get Basket with Total Price
            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if(basket == null)
            {
                return BadRequest();
            }
            //Publish Checkout Event to RabbitMQ
            //To be consumed by Order Api
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);

            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);


            //Delete Basket
            await _repository.DeleteBasket(basket.UserName);
            return Accepted();
        }

    }
}
