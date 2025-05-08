using Tutorial8.Models;

namespace Tutorial8.Services;

// Provides service on Trip information etc.
public interface ITripService
{
    
    public Task<List<Trip>> GetAllTrips();
    
    public Task<List<Trip>> GetTripsByClientId(int clientId);
    
}