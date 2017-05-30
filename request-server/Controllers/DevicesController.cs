using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UptimeBoard.RequestServer.Models;

namespace UptimeBoard.RequestServer.Controllers
{
    [Route("api/[controller]")]
    public class DevicesController : Controller
    {
        private string _content = null;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (_content == null)
            {
                var info = new System.IO.FileInfo("servers.json");
                var reader = info.OpenText();
                _content = await reader.ReadToEndAsync();
            }
            
            return Ok(JsonConvert.DeserializeObject<List<DeviceViewModel>>(_content));
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
