using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CUnity.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CUnity.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListConfigurationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ListConfigurationController(ApplicationDBContext context)
        {
            this._context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var devs = await _context.ListConfiguration.ToListAsync();
            return Ok(devs);
        }
    }
}
