using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models;
using Tutorial8.Services;

namespace Tutorial8;

[ApiController]
[Route("[controller]")]
public class ApiController(ITripService tripService, IClientService clientService) : ControllerBase
{

    private readonly ITripService _tripService = tripService;
    private readonly IClientService _clientService = clientService;
    
    [HttpGet("trips")]
    public async Task<IActionResult> GetAllTasks()
    {
        var trips = await _tripService.GetAllTrips();
        return Ok(trips);
    }

    [HttpGet("clients/{clientId}/trips")]
    public async Task<IActionResult> GetAllClientsTrips(int clientId)
    {
        List<Trip> trips = await _tripService.GetTripsByClientId(clientId);
        if (trips.Count == 0)
        {
            return BadRequest($"Client with id {clientId} doesn't exist or has no trips.");
        }
        return Ok(trips);
    }

    [HttpPost("clients")]
    public async Task<IActionResult> AddClient([FromBody] Client client)
    {
        if (!client.validate()) return BadRequest("Incorrect client information.");
        var response = await _clientService.AddClient(client);
        switch (response)
        {
            case IClientService.Response.ClientExists:
                return BadRequest($"Client with id {client.Id} already exists.");
            case IClientService.Response.InternalError:
                return StatusCode(StatusCodes.Status500InternalServerError);
            case IClientService.Response.Success:
                return Ok("Client successfully added.");
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("clients/{clientId}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClientForTrip(int clientId, int tripId)
    {
        var response = await _clientService.AddRegisteredTrip(clientId, tripId);
        switch (response)
        {
            case IClientService.Response.Success:
                return Ok("Successfully registered a client.");
            case IClientService.Response.InternalError:
                return StatusCode(StatusCodes.Status500InternalServerError);
            case IClientService.Response.ClientNotExists:
                return BadRequest($"Client with id {clientId} doesn't exist.");
            case IClientService.Response.TripNotExists:
                return BadRequest($"Trip with id {tripId} doesn't exist.");
            case IClientService.Response.RegistrationExists:
                return BadRequest($"Trip registration with id {tripId} for client with id {clientId} already exists.");
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("clients/{clientId}/trips/{tripId}")]
    public async Task<IActionResult> RemoveClientForTrip(int clientId, int tripId)
    {
        var response = await _clientService.RemoveRegisteredTrip(clientId, tripId);
        switch (response)
        {
            case IClientService.Response.Success:
                return Ok($"Successfully deleted a trip {tripId} registered for client {clientId}.");
            case IClientService.Response.InternalError:
                return StatusCode(StatusCodes.Status500InternalServerError);
            case IClientService.Response.ClientNotExists:
                return BadRequest($"Client with id {clientId} doesn't exist.");
            case IClientService.Response.TripNotExists:
                return BadRequest($"Trip with id {tripId} doesn't exist.");
            case IClientService.Response.TooManyPeople:
                return BadRequest("Trip has already too much people signed for it.");
            case IClientService.Response.NoRegistration:
                return BadRequest($"There is no registration of client {clientId} for the trip {tripId}.");
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
}