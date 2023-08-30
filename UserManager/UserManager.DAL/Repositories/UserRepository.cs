using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDapperRepository _dapperRepository;

    public UserRepository(IDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    public async Task AddUserAsync(User user)
    {
        var sql = @"INSERT INTO ""Users"" 
                   VALUES (@Id, @FirstName, @LastName, @Email, @PhoneNumber, @Password, @Organization, @Department, @Salt, @Role)";

        await _dapperRepository.ExecuteAsync(sql, user);
    }

    public async Task<bool> CheckEmailIsExistAsync(string email)
    {
        var sql = @"SELECT *
                   FROM ""Users""
                   WHERE ""Email"" = @Email";

        return (await _dapperRepository.QueryFirstOrDefaultAsync<User?>(sql, new { Email = email.ToLower() })) is not null;
    }

    public async ValueTask<User?> GetUserByIdAsync(Guid id)
    {
        var sql = $@"SELECT *
                   FROM ""Users""
                   WHERE ""Id"" = @Id";

        return await _dapperRepository.QueryFirstOrDefaultAsync<User?>(sql, new { Id = id });
    }
    public async Task<User?> GetUserByForgetPasswordIdAsync(Guid id)
    {
        var sql = $@"SELECT u.*
                    FROM ""Users"" u
                    INNER JOIN ""ForgotPassword"" fp ON
                    u.""Id"" = fp.""UserId""
                    WHERE fp.""Id"" = @ForgotPasswordId";

        return await _dapperRepository.QueryFirstOrDefaultAsync<User?>(sql, new { ForgotPasswordId = id });
    }

    public async Task UpdateUserAsync(User user)
    {
        var sql = @"UPDATE ""Users"" 
                    SET ""FirstName"" = @FirstName,
                        ""LastName"" = @LastName,
                        ""Email"" = @Email,
                        ""PhoneNumber"" = @PhoneNumber,
                        ""Password"" = @Password,
                        ""Organization"" = @Organization,
                        ""Department"" = @Department,
                        ""Salt"" = @Salt,
                        ""Role"" = @Role
                    WHERE ""Id"" = @Id";

        await _dapperRepository.ExecuteAsync(sql, user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var sql = $@"SELECT *
                    FROM ""Users""
                    WHERE ""Email"" = @Email";

        return await _dapperRepository.QueryFirstOrDefaultAsync<User?>(sql, new { Email = email.ToLower() });
    }
}
