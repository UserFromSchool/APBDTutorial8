using Tutorial8.Models;

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

    public Task<Response> AddClient(Client client);
    
    public Task<Response> AddRegisteredTrip(int clientId, int tripId);
    
    public Task<Response> RemoveRegisteredTrip(int clientId, int tripId);

}