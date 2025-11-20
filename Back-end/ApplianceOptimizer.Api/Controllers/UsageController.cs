using ApplianceOptimizer.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ApplianceOptimizer.Api.Controllers
{
    [ApiController]
    [Route("api/usage")]
    [Authorize]
    public class UsageController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsageController(AppDbContext db)
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

        [HttpGet("today")]
        public async Task<IActionResult> Today()
        {
            var userId = GetUserId();
            var total = await _db.Appliances
                .Where(a => a.UserId == userId)
                .SumAsync(a => (a.Wattage * a.Hours) / 1000.0);

            return Ok(new { totalKwh = Math.Round(total, 2) });
        }

        [HttpGet("predicted")]
        public async Task<IActionResult> Predicted()
        {
            var userId = GetUserId();
            var total = await _db.Appliances
                .Where(a => a.UserId == userId)
                .SumAsync(a => (a.Wattage * a.Hours) / 1000.0);

            return Ok(new { predicted = Math.Round(total * 1.12, 2) });
        }

        [HttpGet("weekly")]
        public IActionResult Weekly()
        {
            return Ok(new 
            { 
                labels = new[] { "Mon","Tue","Wed","Thu","Fri","Sat","Sun" },
                values = new[] { 1.2, 1.5, 1.4, 1.6, 1.7, 1.55, 1.6 }
            });
        }

        [HttpGet("category")]
        public async Task<IActionResult> Category()
        {
            var userId = GetUserId();
            var apps = await _db.Appliances.Where(a => a.UserId == userId).ToListAsync();

            double essential = apps
                .Where(a => a.Category.ToLower() == "essential")
                .Sum(a => (a.Wattage * a.Hours) / 1000.0);

            double optional = apps
                .Where(a => a.Category.ToLower() != "essential")
                .Sum(a => (a.Wattage * a.Hours) / 1000.0);

            return Ok(new 
            {
                essential = Math.Round(essential, 2),
                optional = Math.Round(optional, 2)
            });
        }
    }
}
