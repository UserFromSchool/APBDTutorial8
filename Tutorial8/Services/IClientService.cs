using Tutorial8.Models.DTO;

namespace Tutorial8.Services;

public interface  IClientService
{

    public enum Response
    {
        ClientNotExists,
        ClientExists,
        TripNotExists,
        InternalError,
        TooManyPeople,
        Success,
        NoRegistration,
        RegistrationExists
    }

    public Task<Response> AddClient(ClientDTO clientDto);
    
    public Task<Response> AddRegisteredTrip(int clientId, int tripId);
    
    public Task<Response> RemoveRegisteredTrip(int clientId, int tripId);

}