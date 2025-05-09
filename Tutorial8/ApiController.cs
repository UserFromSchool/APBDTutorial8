using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTO;
using Tutorial8.Services;

namespace Tutorial8;

/**
 * As the API is pretty small using a single controller for the endpoints.
 */

[ApiController]
[Route("[controller]")]
public class ApiController(ITripService tripService, IClientService clientService) : ControllerBase
{

    private readonly ITripService _tripService = tripService;
    private readonly IClientService _clientService = clientService;
    
    [HttpGet("trips")]
    
    // Returns all the trips in JSON format. Each trip contains all the standard information needed for the trip.
    public async Task<IActionResult> GetAllTrips()
    {
        var trips = await _tripService.GetAllTrips();
        return Ok(trips.Select(entity => new
        {
            id = entity.Id,
            name = entity.Name,
            description = entity.Description,
            startDate = entity.StartDate,
            endDate = entity.EndDate,
            maxPeople = entity.MaxPeople,
            destinationCountryName = entity.DestinationCountryName
        }));
    }

    // Returns all the trips associated with the particular client id in JSON format. In case the client doesn't have trips or does
    // not exist, the information 'Client with id {clientId} doesn't exist or has no trips' is returned as string.
    [HttpGet("clients/{clientId}/trips")]
    public async Task<IActionResult> GetAllClientsTrips(int clientId)
    {
        List<TripDTO> trips = await _tripService.GetTripsByClientId(clientId);
        if (trips.Count == 0)
        {
            return BadRequest($"Client with id {clientId} doesn't exist or has no trips.");
        }
        return Ok(trips.Select(entity => new
        {
            id = entity.Id,
            name = entity.Name,
            description = entity.Description,
            startDate = entity.StartDate,
            endDate = entity.EndDate,
            maxPeople = entity.MaxPeople,
            paymentDate = entity.PaymentDate,
            registeredAt = entity.RegisteredAt
        }));
    }

    // Allows creating a client. Requires a body to specify all the required fields if the clientDTO. Also,
    // clientDTO has some basic validation. In case a client exists already, or the information is incorrect, the correct
    // message will be sent with the correct status code.
    [HttpPost("clients")]
    public async Task<IActionResult> AddClient([FromBody] ClientDTO clientDto)
    {
        if (!clientDto.Validate()) return BadRequest("Incorrect client information.");
        var response = await _clientService.AddClient(clientDto);
        switch (response)
        {
            case IClientService.Response.ClientExists:
                return BadRequest($"Client with id {clientDto.Id} already exists.");
            case IClientService.Response.InternalError:
                return StatusCode(StatusCodes.Status500InternalServerError);
            case IClientService.Response.Success:
                return Ok("Client successfully added.");
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // Allows registering the user for the given trip if both user and trip exist, and user have not been already signed for
    // this trip. Also, if the trip has all places taken, the request will be refused. Returns information as string with proper status
    // codes.
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
                return BadRequest($"Trip with id {tripId} for client with id {clientId} already registered.");
            case IClientService.Response.TooManyPeople:
                return BadRequest($"Trip with id {tripId} has already all the places taken.");
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // Deletes given registration of the client for the given trip given that it exists. In case of incorrect
    // information (client id, trip id etc.) returns the appropriate string message and status code.
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