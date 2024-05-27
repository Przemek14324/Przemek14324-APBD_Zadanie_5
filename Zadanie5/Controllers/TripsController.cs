using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripApi.Models;
using TripApi.DTOs;

namespace Zadanie5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly TripContext _context;

        public TripsController(TripContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetTrips()
        {
            var trips = await _context.Trips
                .OrderByDescending(t => t.DateFrom)
                .Select(t => new TripDto
                {
                    IdTrip = t.IdTrip,
                    Name = t.Name,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo
                }).ToListAsync();

            return Ok(trips);
        }

        [HttpPost("{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, ClientTripDto dto)
        {
            var trip = await _context.Trips.FindAsync(idTrip);
            if (trip == null)
            {
                return NotFound("Trip not found.");
            }

            var client = await _context.Clients.SingleOrDefaultAsync(c => c.Pesel == dto.Pesel);
            if (client == null)
            {
                client = new Client { Pesel = dto.Pesel, FirstName = "Unknown", LastName = "Unknown", Email = "unknown@example.com", Telephone = "000000000" };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }

            var clientTripExists = await _context.ClientTrips
                .AnyAsync(ct => ct.IdClient == client.IdClient && ct.IdTrip == idTrip);
            if (clientTripExists)
            {
                return BadRequest("Client is already assigned to this trip.");
            }

            var clientTrip = new ClientTrip
            {
                IdClient = client.IdClient,
                IdTrip = idTrip,
                PaymentDate = dto.PaymentDate,
                RegisteredAt = DateTime.Now
            };

            _context.ClientTrips.Add(clientTrip);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrips), new { idTrip }, clientTrip);
        }
    }
}
