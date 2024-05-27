using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripApi.Models;

namespace TripApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly TripContext _context;

        public ClientsController(TripContext context)
        {
            _context = context;
        }

        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var client = await _context.Clients.FindAsync(idClient);
            if (client == null)
            {
                return NotFound();
            }

            var clientTrips = await _context.ClientTrips
                .Where(ct => ct.IdClient == idClient)
                .ToListAsync();

            if (clientTrips.Any())
            {
                return BadRequest("Client has assigned trips.");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
