using Tutorial8.Models.DTO;

namespace Tutorial8.Services;

// Provides service on Trip information, etc.
public interface ITripService
{
    
    public Task<List<TripDTO>> GetAllTrips();
    
    public Task<List<TripDTO>> GetTripsByClientId(int clientId);
    
}