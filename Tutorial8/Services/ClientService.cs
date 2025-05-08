using Microsoft.Data.SqlClient;
using Tutorial8.Models;

namespace Tutorial8.Services;

public class ClientService(string connectionString) : IClientService
{
    private readonly string _connectionString = connectionString;

    public async Task<IClientService.Response> AddClient(Client client)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("SELECT * FROM Client WHERE IdClient = @IdClient", connection))
        {
            await connection.OpenAsync();

            var foundClient = false;
            command.Parameters.AddWithValue("@IdClient", client.Id);
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    foundClient = true;
                    break;
                }
            }

            if (foundClient) return IClientService.Response.ClientExists;
            command.Parameters.Clear();

            var transaction = await connection.BeginTransactionAsync();
            var query = @"
            INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel) VALUES (@IdClient, @FirstName, @LastName, @Email, @Telephone, @Pesel)
            ";
            command.Transaction = transaction as SqlTransaction;

            try
            {
                command.CommandText = query;
                command.Parameters.AddWithValue("@IdClient", client.Id);
                command.Parameters.AddWithValue("@FirstName", client.FirstName);
                command.Parameters.AddWithValue("@LastName", client.LastName);
                command.Parameters.AddWithValue("@Email", client.Email);
                command.Parameters.AddWithValue("@Telephone", client.PhoneNumber);
                command.Parameters.AddWithValue("@Pesel", client.Pesel);
                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (SqlException e)
            {
                await transaction.RollbackAsync();
                return IClientService.Response.InternalError;
            }
        }

        return IClientService.Response.Success;
    }

    public async Task<IClientService.Response> AddRegisteredTrip(int clientId, int tripId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("SELECT * FROM Client WHERE IdClient = @IdClient", connection))
        {
            await connection.OpenAsync();

            bool foundClient = false;
            bool foundTrip = false;
            bool foundRegistration = false;

            // Check if a client exists
            command.Parameters.AddWithValue("@IdClient", clientId);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    foundClient = true;
                    break;
                }
            }

            if (!foundClient) return IClientService.Response.ClientNotExists;
            command.Parameters.Clear();

            // Check if a trip exists
            command.CommandText = "SELECT * FROM Trip WHERE IdTrip = @IdTrip";
            command.Parameters.AddWithValue("@IdTrip", tripId);
            var maxPeople = 0;
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    foundTrip = true;
                    maxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople"));
                    break;
                }
            }
            if (!foundTrip) return IClientService.Response.TripNotExists;
            command.Parameters.Clear();
            
            // Check for clients with such trip
            command.CommandText = "SELECT * FROM Client INNER JOIN Client_Trip ON Client_Trip.IdClient = Client.IdClient " +
                                  "WHERE Client_Trip.IdTrip = @IdTrip";
            command.Parameters.AddWithValue("@IdTrip", tripId);
            using (var reader = await command.ExecuteReaderAsync())
            {
                var count = 0;
                while (await reader.ReadAsync())
                {
                    count++;
                }

                if (count >= maxPeople)
                {
                    return IClientService.Response.TooManyPeople;
                }
            }
            command.Parameters.Clear();
            
            // Check if there is a registration for such trip already
            command.CommandText = "SELECT * FROM Client_Trip WHERE IdTrip = @IdTrip AND IdClient = @IdClient";
            command.Parameters.AddWithValue("@IdTrip", tripId);
            command.Parameters.AddWithValue("@IdClient", clientId);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    foundRegistration = true;
                    break;
                }
            }

            if (foundRegistration) return IClientService.Response.RegistrationExists;
            command.Parameters.Clear();
            
            // Register a new trip
            var transaction = await connection.BeginTransactionAsync();
            command.Transaction = transaction as SqlTransaction;

            try
            {
                command.CommandText = @"
                INSERT INTO Client_Trip (IdTrip, IdClient, RegisteredAt) VALUES (@IdTrip, @IdClient, @RegisteredAt)
                ";
                command.Parameters.AddWithValue("@IdTrip", tripId);
                command.Parameters.AddWithValue("@IdClient", clientId);
                command.Parameters.AddWithValue("@RegisteredAt", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (SqlException e)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(e.Message);
                return IClientService.Response.InternalError;
            }
        }

        return IClientService.Response.Success;
    }

    public async Task<IClientService.Response> RemoveRegisteredTrip(int clientId, int tripId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("SELECT * FROM Client WHERE IdClient = @IdClient", connection))
        {
            await connection.OpenAsync();

            bool foundClient = false;
            bool foundTrip = false;
            bool foundRegistration = false;

            // Check if a client exists
            command.Parameters.AddWithValue("@IdClient", clientId);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    foundClient = true;
                    break;
                }
            }

            if (!foundClient) return IClientService.Response.ClientNotExists;
            command.Parameters.Clear();

            // Check if a trip exists
            command.CommandText = "SELECT * FROM Trip WHERE IdTrip = @IdTrip";
            command.Parameters.AddWithValue("@IdTrip", tripId);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    foundTrip = true;
                    break;
                }
            }

            if (!foundTrip) return IClientService.Response.TripNotExists;
            command.Parameters.Clear();
            
            // Check if there even is a registration
            command.CommandText = "SELECT * FROM Client_Trip WHERE IdTrip = @IdTrip AND IdClient = @IdClient";
            command.Parameters.AddWithValue("@IdTrip", tripId);
            command.Parameters.AddWithValue("@IdClient", clientId);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    foundRegistration = true;
                    break;
                }
            }

            if (!foundRegistration) return IClientService.Response.NoRegistration;
            command.Parameters.Clear();

            // Delete the registration.
            var transaction = await connection.BeginTransactionAsync();
            command.Transaction = transaction as SqlTransaction;

            try
            {
                command.Parameters.AddWithValue("@IdTrip", tripId);
                command.Parameters.AddWithValue("@IdClient", clientId);
                command.CommandText = @"
                DELETE Client_Trip WHERE IdTrip = @IdTrip AND IdClient = @IdClient
                ";
                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (SqlException e)
            {
                await transaction.RollbackAsync();
                return IClientService.Response.InternalError;
            }
        }

        return IClientService.Response.Success;
    }
}