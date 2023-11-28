namespace AICommunicationService.DAL.Interfaces
{
    /// <summary>
    /// Interface defining methods for managing user wallet transactions and permissions.
    /// </summary>
    public interface IWalletRepository
    {
        /// <summary>
        /// Checks if a user is allowed to create a request based on their ID and a minimum requirement.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userMinimum">Minimum requirement for user permission</param>
        /// <returns>Task representing the asynchronous operation, returning a boolean indicating permission</returns>
        Task<bool> CheckIfUserAllowedToCreateRequest(Guid userId, int userMinimum);

        /// <summary>
        /// Takes a service fee from a user's wallet based on their ID and the fee amount.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="feeAmount">Amount of the service fee</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task TakeServiceFeeAsync(Guid userId, int feeAmount);
    }

}
