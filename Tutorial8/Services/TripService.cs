using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTO;

namespace Tutorial8.Services;

public class TripService(string connectionString) : ITripService
{
    
    private readonly string _connectionString = connectionString;
    
    public async Task<List<TripDTO>> GetAllTrips()
    {
        List<TripDTO> trips = new List<TripDTO>();
        
        var query = "SELECT Trip.IdTrip AS IdTrip, Trip.Name as Name, Trip.Description as Description, " +
                    "Trip.DateFrom AS DateFrom, Trip.DateTo AS DateTo, Trip.MaxPeople AS MaxPeople, Country.Name AS CountryName " +
                    "FROM Trip INNER JOIN Country_Trip " +
                    "ON Trip.IdTrip = Country_Trip.IdTrip INNER JOIN Country " +
                    "ON Country.IdCountry = Country_Trip.IdCountry";
        
        using (var connection = new SqlConnection(_connectionString))
        await using (var command = new SqlCommand(query, connection))
        {
            await connection.OpenAsync();

            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    trips.Add(new TripDTO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        StartDate = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        EndDate = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                        DestinationCountryName = reader.GetString(reader.GetOrdinal("CountryName"))
                    });
                }
            }
        }
        
        return trips;
    }

    public async Task<List<TripDTO>> GetTripsByClientId(int clientId)
    {
        List<TripDTO> trips = new List<TripDTO>();
        
        var query = @"
        SELECT Trip.IdTrip AS IdTrip, 
        Trip.Name AS Name, 
        Trip.Description AS Description,
        Trip.DateFrom AS DateFrom, 
        Trip.DateTo AS DateTo, 
        Trip.MaxPeople AS MaxPeople,
        Client_Trip.RegisteredAt AS RegisteredAt, 
        Client_Trip.PaymentDate AS PaymentDate
        FROM Trip 
        INNER JOIN Client_Trip ON Client_Trip.IdTrip = Trip.IdTrip 
        WHERE Client_Trip.IdClient = @ClientId";
        
        using (var connection = new SqlConnection(_connectionString))
        await using (var command = new SqlCommand(query, connection))
        {
            await connection.OpenAsync();
            
            command.Parameters.AddWithValue("@ClientId", clientId);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    trips.Add(new TripDTO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        StartDate = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        EndDate = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                        PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate")) ? null : reader.GetInt32(reader.GetOrdinal("PaymentDate")),
                        RegisteredAt = reader.GetInt32(reader.GetOrdinal("RegisteredAt"))
                    });
                }
            }
        }

        return trips;
    }
    
}