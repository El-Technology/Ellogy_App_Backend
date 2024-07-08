using UserManager.DAL.Models;

namespace UserManager.DAL.Interfaces;

public interface IUserRepository
{
    /// <summary>
    ///     Add new user to database
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task AddUserAsync(User user);

    /// <summary>
    ///     Update user in database
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task UpdateUserAsync(User user);

    /// <summary>
    ///     Get user by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ValueTask<User?> GetUserByIdAsync(Guid id);


    /// <summary>
    ///     Get user by email
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<User?> GetUserByForgetPasswordIdAsync(Guid id);

    /// <summary>
    ///     Check if email is exist
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public Task<bool> CheckEmailIsExistAsync(string email);

    /// <summary>
    ///     Get user by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    ///     Delete user from database
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task DeleteUserAsync(User user);

    /// <summary>
    ///    Find user by email
    /// </summary>
    /// <param name="emailPrefix"></param>
    /// <returns></returns>
    Task<List<User>> FindUserByEmailAsync(string emailPrefix);
}