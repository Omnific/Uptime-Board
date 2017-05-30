using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UptimeBoard.RequestServer.Models;

namespace UptimeBoard.RequestServer.Controllers
{
    [Route("api/[controller]")]
    public class DevicesController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var deviceObjects = new List<DeviceViewModel>
            {
                new DeviceViewModel
                {
                    Name = "raxxy_1",
                    Address = "8.8.8.8",
                    RequestTimeout = 10000,
                    Type = RequestType.Ping,
                    Total = 4
                }
            };

            return Ok(deviceObjects);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            return NotFound();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]string value)
        {
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NotFound();
        }
    }
}
