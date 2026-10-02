using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using OrdersService.BusinessLogicLayer.DTO;
using OrdersService.BusinessLogicLayer.HttpClients;
using OrdersService.DataAccessLayer.Entities;
using OrdersService.DataAccessLayer.RepositoryContracts;
using System.Text;
using System.Text.Json;

using OrdersServiceClass = OrdersService.BusinessLogicLayer.Services.OrdersService;

namespace OrdersService.Test;

public class OrdersServiceTest
{
    private readonly Mock<IValidator<OrderAddRequest>> _orderAddRequestValidatorMock;
    private readonly Mock<IValidator<OrderItemAddRequest>> _orderItemAddRequestValidatorMock;
    private readonly Mock<IValidator<OrderUpdateRequest>> _orderUpdateRequestValidatorMock;
    private readonly Mock<IValidator<OrderItemUpdateRequest>> _orderItemUpdateRequestValidatorMock;

    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IOrdersRepository> _repositoryMock;

    private readonly OrdersServiceClass _service;

    public OrdersServiceTest()
    {
        // Dependencies for the microservice clients
        _cacheMock = new Mock<IDistributedCache>();
        var httpClient = new HttpClient();

        // Dependencies for the service
        _orderAddRequestValidatorMock = new Mock<IValidator<OrderAddRequest>>();
        _orderItemAddRequestValidatorMock = new Mock<IValidator<OrderItemAddRequest>>();
        _orderUpdateRequestValidatorMock = new Mock<IValidator<OrderUpdateRequest>>();
        _orderItemUpdateRequestValidatorMock = new Mock<IValidator<OrderItemUpdateRequest>>();

        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IOrdersRepository>();

        var usersClient = new UsersMicroserviceClient(httpClient, _cacheMock.Object);

        var productsClient = new ProductsMicroserviceClient(httpClient, _cacheMock.Object);

        // Service
        _service = new OrdersServiceClass(_repositoryMock.Object, _mapperMock.Object,
            _orderAddRequestValidatorMock.Object, _orderItemAddRequestValidatorMock.Object,
            _orderUpdateRequestValidatorMock.Object, _orderItemUpdateRequestValidatorMock.Object,
            usersClient, productsClient);
    }

    #region AddOrder

    [Fact]
    public async Task AddOrder_ShouldReturnOrderResponse_WhenOrderIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var orders = new List<OrderItemAddRequest>() { new(productId, Quantity: 2, UnitPrice: 100 ) };
        var orderAddRequest = new OrderAddRequest(userId, DateTime.Now, orders);

        _orderAddRequestValidatorMock.Setup(validator => validator.ValidateAsync(
            It.IsAny<OrderAddRequest>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(new ValidationResult());
        _orderItemAddRequestValidatorMock.Setup(validator => validator.ValidateAsync(
            It.IsAny<OrderItemAddRequest>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(new ValidationResult(Array.Empty<ValidationFailure>()));

        var order = new Order { 
            OrderId = Guid.NewGuid(),
            UserId = userId,
            OrderItems = [ new(productId, 2, 100) ]
        };
        _mapperMock.Setup(mapper => mapper.Map<Order>(orderAddRequest)).Returns(order);

        var user = new UserDto(userId, "john@example.com", "John", "Male");
        var userJson = JsonSerializer.Serialize(user);
        _cacheMock.Setup(cache => cache.GetAsync($"user:{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(userJson));

        var product = new ProductDto(productId, "Name", "Category", 100, 100);
        var productJson = JsonSerializer.Serialize(product);
        _cacheMock.Setup(cache => cache.GetAsync($"product:{productId}", It.IsAny<CancellationToken>()))
             .ReturnsAsync(Encoding.UTF8.GetBytes(productJson));

        _repositoryMock.Setup(repository => repository.AddOrder(order)).ReturnsAsync(order);

        var orderResponse = new OrderResponse
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            OrderItems = null,
            UserDisplayName = "John",
            Email = "john@example.com"
        };
        _mapperMock.Setup(mapper => mapper.Map<OrderResponse>(order)).Returns(orderResponse);

        // Act
        var result = await _service.AddOrder(orderAddRequest);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(orderResponse);
    }

    #endregion

    }
