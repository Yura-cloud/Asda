using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Order;
using Asda.Integration.Service.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Asda.Integration.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetPicture()
        {
            var b = System.IO.File.ReadAllBytes(@"wwwroot\images\Asda_60.png");
            return File(b, "image/jpeg");
        }

        [HttpPost]
        public OrdersResponse Orders([FromBody] OrdersRequest request)
        {
            return _orderService.GetOrdersAndSendManifest(request);
        }

        [HttpPost]
        public OrderDespatchResponse Dispatch([FromBody] OrderDespatchRequest request)
        {
            return _orderService.SendDispatch(request);
        }

        [HttpPost]
        public OrderCancelResponse CanceledOrders([FromBody] OrderCancelRequest request)
        {
            return _orderService.SendCanceledOrders(request);
        }
    }
}