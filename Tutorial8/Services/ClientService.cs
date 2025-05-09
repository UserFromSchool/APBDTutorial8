using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTO;

namespace Tutorial8.Services;


/**
 * In a bigger project, I would separate the SQL requests into separate classes or perhaps use repositories for CRUD,
 * so the code is more re-usable, but
 * here since the project is pretty small, I've just use sql queries directly inside the function, since separating would require
 * separate classes and seemed like too much effort on such a small scale.
 */

public class ClientService(string connectionString) : IClientService
{
    private readonly string _connectionString = connectionString;

    public async Task<IClientService.Response> AddClient(ClientDTO clientDto)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            // Check if a client already exists.
            await using (var command = new SqlCommand("SELECT * FROM Client WHERE IdClient = @IdClient", connection))
            {
                var foundClient = false;
                command.Parameters.AddWithValue("@IdClient", clientDto.Id);
                await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundClient = true;
                        break;
                    }
                }

                if (foundClient) return IClientService.Response.ClientExists;
            }
            
            // Create a new client
            var query = @"
            INSERT INTO Client (IdClient, FirstName, LastName, Email, Telephone, Pesel) VALUES (@IdClient, @FirstName, @LastName, @Email, @Telephone, @Pesel)
            ";
            await using (var command = new SqlCommand(query, connection))
            {
                var transaction = await connection.BeginTransactionAsync();
                command.Transaction = transaction as SqlTransaction;

                try
                {
                    command.Parameters.AddWithValue("@IdClient", clientDto.Id);
                    command.Parameters.AddWithValue("@FirstName", clientDto.FirstName);
                    command.Parameters.AddWithValue("@LastName", clientDto.LastName);
                    command.Parameters.AddWithValue("@Email", clientDto.Email);
                    command.Parameters.AddWithValue("@Telephone", clientDto.PhoneNumber);
                    command.Parameters.AddWithValue("@Pesel", clientDto.Pesel);
                    await command.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                    return IClientService.Response.InternalError;
                }
            }
        }

        return IClientService.Response.Success;
    }

    public async Task<IClientService.Response> AddRegisteredTrip(int clientId, int tripId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var maxPeople = 0;
            
            // Check if a client exists
            await using (var command = new SqlCommand("SELECT * FROM Client WHERE IdClient = @IdClient", connection))
            {
                var foundClient = false;
                command.Parameters.AddWithValue("@IdClient", clientId);
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundClient = true;
                        break;
                    }
                }

                if (!foundClient) return IClientService.Response.ClientNotExists;
            }

            await using (var command = new SqlCommand("SELECT * FROM Trip WHERE IdTrip = @IdTrip", connection))
            {
                // Check if a trip exists
                var foundTrip = false;
                command.Parameters.AddWithValue("@IdTrip", tripId);
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundTrip = true;
                        maxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople"));
                        break;
                    }
                }

                if (!foundTrip) return IClientService.Response.TripNotExists;
            }
            
            
            // Check for clients with such a trip and validate max people amount
            await using (var command = new SqlCommand("SELECT * FROM Client INNER JOIN Client_Trip " +
                                                      "ON Client_Trip.IdClient = Client.IdClient " +
                                                      "WHERE Client_Trip.IdTrip = @IdTrip", connection))
            {
                command.Parameters.AddWithValue("@IdTrip", tripId);
                await using (var reader = await command.ExecuteReaderAsync())
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
            }

            // Check if there is a registration for such a trip already
            await using (var command = new SqlCommand("SELECT * FROM Client_Trip WHERE IdTrip = @IdTrip AND IdClient = @IdClient", connection))
            {
                var foundRegistration = false;
                command.Parameters.AddWithValue("@IdTrip", tripId);
                command.Parameters.AddWithValue("@IdClient", clientId);
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundRegistration = true;
                        break;
                    }
                }

                if (foundRegistration) return IClientService.Response.RegistrationExists;
            }
            
            // Register a given client for the given trip.
            await using (var command = new SqlCommand(@"
            INSERT INTO Client_Trip (IdTrip, IdClient, RegisteredAt) VALUES (@IdTrip, @IdClient, @RegisteredAt)
            ", connection))
            {
                var transaction = await connection.BeginTransactionAsync();
                command.Transaction = transaction as SqlTransaction;
                try
                {
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
        }

        return IClientService.Response.Success;
    }

    public async Task<IClientService.Response> RemoveRegisteredTrip(int clientId, int tripId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Check if a client exists
            await using (var command = new SqlCommand("SELECT * FROM Client WHERE IdClient = @IdClient", connection))
            {
                var foundClient = false;
                command.Parameters.AddWithValue("@IdClient", clientId);
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundClient = true;
                        break;
                    }
                }

                if (!foundClient) return IClientService.Response.ClientNotExists;
            }

            // Check if a trip exists
            await using (var command = new SqlCommand("SELECT * FROM Trip WHERE IdTrip = @IdTrip", connection))
            {
                var foundTrip = false;
                command.Parameters.AddWithValue("@IdTrip", tripId);
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundTrip = true;
                        break;
                    }
                }

                if (!foundTrip) return IClientService.Response.TripNotExists;
            }
            
            // Check if there even is a registration
            await using (var command =
                   new SqlCommand("SELECT * FROM Client_Trip WHERE IdTrip = @IdTrip AND IdClient = @IdClient",
                       connection))
            {
                var foundRegistration = false;   
                command.Parameters.AddWithValue("@IdTrip", tripId);
                command.Parameters.AddWithValue("@IdClient", clientId);
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundRegistration = true;
                        break;
                    }
                }

                if (!foundRegistration) return IClientService.Response.NoRegistration;
            }

            // Delete the registration
            await using (var command = new SqlCommand("DELETE Client_Trip WHERE IdTrip = @IdTrip AND IdClient = @IdClient", connection))
            {
                var transaction = await connection.BeginTransactionAsync();
                command.Transaction = transaction as SqlTransaction;

                try
                {
                    command.Parameters.AddWithValue("@IdTrip", tripId);
                    command.Parameters.AddWithValue("@IdClient", clientId);
                    await command.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                    return IClientService.Response.InternalError;
                }
            }
        }

        return IClientService.Response.Success;
    }
}