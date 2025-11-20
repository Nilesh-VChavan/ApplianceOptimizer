using ApplianceOptimizer.Api.Data;
using ApplianceOptimizer.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ApplianceOptimizer.Api.Controllers
{
    [ApiController]
    [Route("api/appliances")]
    [Authorize]
    public class AppliancesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AppliancesController(AppDbContext db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (id == null)
                throw new UnauthorizedAccessException("User ID claim missing.");

            return int.Parse(id);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();

            var list = await _db.Appliances
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Appliance model)
        {
            model.UserId = GetUserId();

            _db.Appliances.Add(model);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Added", model });
        }
    }
}
