using CUnity.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CUnity.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public AdminController(ApplicationDBContext context)
        {
            this._context = context;
        }
        // POST: api/Admin/GetClients
        [HttpGet("GetClients")]
        public async Task<IActionResult> GetClients()
        {
            //string text = s.ClientName + "," + s.StartDate.ToString() + "," + s.EndDate.ToString() + "," + s.ProductName;
            var res = await _context.Clients.ToListAsync();
            return Ok(res);
        }
    }
}
