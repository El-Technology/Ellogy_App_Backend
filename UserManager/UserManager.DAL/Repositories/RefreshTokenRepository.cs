using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDapperRepository _dapperRepository;
        public RefreshTokenRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task UpdateTokenExpireDateAsync(RefreshToken refreshToken)
        {
            var sql = $@"UPDATE ""RefreshTokens""
                        SET ""ExpireDate"" = @ExpireDate
                        WHERE ""Id"" = @Id";

            await _dapperRepository.ExecuteAsync(sql,
                new
                {
                    refreshToken.ExpireDate,
                    refreshToken.Id
                });
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            var sql = $@"INSERT INTO ""RefreshTokens"" 
                        VALUES (@Id, @ExpireDate, @Value, @IsValid, @UserId)";

            await _dapperRepository.ExecuteAsync(sql, refreshToken);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(Guid userId)
        {
            var sql = $@"SELECT *
                        FROM ""RefreshTokens""
                        WHERE ""UserId"" = @UserId";

            return await _dapperRepository.QueryFirstOrDefaultAsync<RefreshToken?>(sql, new { UserId = userId });
        }
    }
}
