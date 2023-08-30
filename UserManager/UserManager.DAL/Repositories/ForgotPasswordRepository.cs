using UserManager.Common.Helpers;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories;

public class ForgotPasswordRepository : IForgotPasswordRepository
{
    private readonly IDapperRepository _dapperRepository;

    public ForgotPasswordRepository(IDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    public async Task AddForgotTokenAsync(ForgotPassword forgotPasswordEntry)
    {
        var sql = @$"INSERT INTO ""ForgotPassword""
                    VALUES (@Id, @Token, @UserId, @ExpireDate, @IsValid)";

        await _dapperRepository.ExecuteAsync(sql, forgotPasswordEntry);
    }

    public async Task<bool> ValidateResetRequestAsync(Guid id, string token)
    {
        var sql = $@"SELECT *
                    FROM ""ForgotPassword""
                    WHERE ""Id"" = @Id";

        var forgotPasswordEntry = await _dapperRepository.QueryFirstOrDefaultAsync<ForgotPassword?>(sql, new { Id = id });

        return forgotPasswordEntry is not null &&
               CryptoHelper.GetHash(forgotPasswordEntry.Token) == token &&
               forgotPasswordEntry.ExpireDate >= DateTime.UtcNow &&
               forgotPasswordEntry.IsValid;
    }

    public async Task InvalidateTokenAsync(Guid id)
    {
        var sql = @$"UPDATE ""ForgotPassword""
                    SET ""IsValid"" = '0'
                    WHERE ""Id"" = @Id";

        await _dapperRepository.ExecuteAsync(sql, new { Id = id });
    }
}
