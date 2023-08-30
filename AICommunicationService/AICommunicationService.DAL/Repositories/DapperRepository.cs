using AICommunicationService.DAL.Interfaces;
using Dapper;
using Npgsql;
using System.Data;

namespace AICommunicationService.DAL.Repositories
{
    public class DapperRepository : IDapperRepository
    {
        private readonly string _connectionString;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public DapperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task<IDbConnection> OpenConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
        {
            await _semaphore.WaitAsync();
            try
            {
                using var connection = await OpenConnectionAsync();
                return await connection.QueryAsync<T>(sql, parameters);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
        {
            await _semaphore.WaitAsync();
            try
            {
                using var connection = await OpenConnectionAsync();
                return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            await _semaphore.WaitAsync();
            try
            {
                using var connection = await OpenConnectionAsync();
                return await connection.ExecuteAsync(sql, parameters);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
