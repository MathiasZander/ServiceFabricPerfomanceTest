using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web4.Pages
{
    using MassTransit.RabbitMqTransport;


    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            RabbitMqHostAddress address = new RabbitMqHostAddress("localhost", 80, string.Empty);

            _logger.LogInformation(address.Port.Value.ToString());
        }
    }
}
