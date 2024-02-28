using AutoFixture;
using AutoMapper;
using Moq;
using Moq.Protected;
using PaymentManager.BLL.Mapping;
using PaymentManager.BLL.Models;
using PaymentManager.BLL.Services;
using Stripe;

namespace PaymentManager.Tests.ServiceTests;
public class ProductCatalogServiceTest : StripeBaseServiceForTests
{
    private IMapper _mapper;
    private Mock<ProductCatalogService> _productCatalogService;

    [SetUp]
    public void Setup()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ProductProfile>());
        _mapper = new Mapper(configuration);

        _productCatalogService = new Mock<ProductCatalogService>(_mapper);
        _productService = new Mock<ProductService>();
    }

    [Test]
    public async Task GetSubscriptionCatalogAsync_ShouldReturnListOfProducts()
    {
        // Arrange
        _productCatalogService.Protected().Setup<ProductService>(GET_PRODUCT_SERVICE)
            .Returns(_productService.Object);

        _productService.Setup(x => x.ListAsync(It.IsAny<ProductListOptions>(), null, default))
            .ReturnsAsync(new StripeList<Product>() { Data = new List<Product>() { new Product() } });

        // Act
        var result = await _productCatalogService.Object.GetSubscriptionCatalogAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<ProductModel>>());
    }

    [Test]
    public async Task GetProductAsync_ShouldReturnProduct()
    {
        // Arrange
        _productCatalogService.Protected().Setup<ProductService>(GET_PRODUCT_SERVICE)
            .Returns(_productService.Object);

        _productService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<ProductGetOptions>(), null, default))
            .ReturnsAsync(new Product());

        // Act
        var result = await _productCatalogService.Object.GetProductAsync(_fixture.Create<string>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ProductModel>());
    }

    [Test]
    public async Task GetProductByNameAsync_ShouldReturnProduct()
    {
        // Arrange
        _productCatalogService.Protected().Setup<ProductService>(GET_PRODUCT_SERVICE)
            .Returns(_productService.Object);

        _productService.Setup(x => x.SearchAsync(It.IsAny<ProductSearchOptions>(), null, default))
            .ReturnsAsync(new StripeSearchResult<Product>() { Data = new List<Product>() { new Product() } });

        // Act
        var result = await _productCatalogService.Object.GetProductByNameAsync(_fixture.Create<string>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ProductModel>());
    }
}
