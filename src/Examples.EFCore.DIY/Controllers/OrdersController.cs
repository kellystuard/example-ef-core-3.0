using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Examples.EFCore.DIY.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly Context _context;

        public OrdersController(ILogger<OrdersController> logger, Context context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var orders = _context.Orders.ToArray();

            return Ok(orders);
        }
    }
}
