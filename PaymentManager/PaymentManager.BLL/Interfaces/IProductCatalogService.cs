using PaymentManager.BLL.Models;

namespace PaymentManager.BLL.Interfaces;

/// <summary>
///     Interface defining methods for accessing product catalog information.
/// </summary>
public interface IProductCatalogService
{
    /// <summary>
    ///     Retrieves product information asynchronously based on the product ID.
    /// </summary>
    /// <param name="productId">The unique identifier of the product</param>
    /// <returns>Task representing the asynchronous operation, returning the product model</returns>
    Task<ProductModel> GetProductAsync(string productId);

    /// <summary>
    ///     Retrieves product information asynchronously based on the product name.
    /// </summary>
    /// <param name="productName">The name of the product</param>
    /// <returns>Task representing the asynchronous operation, returning the product model</returns>
    Task<ProductModel> GetProductByNameAsync(string productName);

    /// <summary>
    ///     Retrieves the subscription catalog asynchronously.
    /// </summary>
    /// <returns>An asynchronous enumerable collection of product models representing the subscription catalog</returns>
    Task<IEnumerable<ProductModel>> GetSubscriptionCatalogAsync();
}