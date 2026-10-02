using AutoMapper;
using FluentValidation;
using MongoDB.Driver;
using OrdersService.BusinessLogicLayer.DTO;
using OrdersService.BusinessLogicLayer.HttpClients;
using OrdersService.BusinessLogicLayer.ServiceContracts;
using OrdersService.DataAccessLayer.Entities;
using OrdersService.DataAccessLayer.RepositoryContracts;

namespace OrdersService.BusinessLogicLayer.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrdersRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<OrderAddRequest> _orderAddRequestValidator;
        private readonly IValidator<OrderItemAddRequest> _orderItemAddRequestValidator;
        private readonly IValidator<OrderUpdateRequest> _orderUpdateRequestValidator;
        private readonly IValidator<OrderItemUpdateRequest> _orderItemUpdateRequestValidator;
        private readonly UsersMicroserviceClient _usersMicroserviceClient;
        private readonly ProductsMicroserviceClient _productsMicroserviceClient;

        public OrdersService(IOrdersRepository repository,
            IMapper mapper,
            IValidator<OrderAddRequest> orderAddRequestValidator,
            IValidator<OrderItemAddRequest> orderItemAddRequestValidator,
            IValidator<OrderUpdateRequest> orderUpdateRequestValidator,
            IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator,
            UsersMicroserviceClient usersMicroserviceClient,
            ProductsMicroserviceClient productsMicroserviceClient)
        {
            _repository = repository;
            _mapper = mapper;
            _orderAddRequestValidator = orderAddRequestValidator;
            _orderItemAddRequestValidator = orderItemAddRequestValidator;
            _orderUpdateRequestValidator = orderUpdateRequestValidator;
            _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;
            _usersMicroserviceClient = usersMicroserviceClient;
            _productsMicroserviceClient = productsMicroserviceClient;
        }

        public async Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest)
        {
            if (orderAddRequest is null) throw new ArgumentNullException(nameof(orderAddRequest));

            var validationResult = await _orderAddRequestValidator.ValidateAsync(orderAddRequest);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }
            if (orderAddRequest.OrderItems != null)
            {
                foreach (var orderItem in orderAddRequest.OrderItems)
                {
                    validationResult = await _orderItemAddRequestValidator.ValidateAsync(orderItem);
                    if (!validationResult.IsValid)
                    {
                        throw new ArgumentException(string.Join(',',
                            validationResult.Errors.Select(e => e.ErrorMessage)));
                    }
                    var product = await _productsMicroserviceClient.GetProduct(orderItem.ProductId);
                    if (product == null) throw new ArgumentException("Invalid Product");
                }
            }

            var user = await _usersMicroserviceClient.GetUser(orderAddRequest.UserId);
            if (user == null) throw new ArgumentException("Invalid User");

            var order = _mapper.Map<Order>(orderAddRequest);
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
            }
            order.TotalAmount = order.OrderItems.Sum(p => p.TotalPrice);

            var fromRepo = await _repository.AddOrder(order);
            if (fromRepo is null) return null;
            return _mapper.Map<OrderResponse>(fromRepo);
        }

        public async Task<bool> DeleteOrder(Guid orderId)
        {
            var filter = Builders<Order>.Filter.Eq(o =>  o.OrderId, orderId);
            var fromRepo = await _repository.GetOrder(filter);
            if (fromRepo is null) return false;
            return await _repository.DeleteOrder(orderId);
        }

        public async Task<OrderResponse?> GetOrder(FilterDefinition<Order> filter)
        {
            var fromRepo = await _repository.GetOrder(filter);
            if (fromRepo is null) return null;
            return _mapper.Map<OrderResponse>(fromRepo);
        }

        public async Task<List<OrderResponse?>> GetOrders()
        {
            var fromRepo = await _repository.GetOrders();
            var orders = _mapper.Map<IEnumerable<OrderResponse?>>(fromRepo);

            foreach (var order in orders)
            {
                if (order is null) continue;
                if (order.OrderItems != null)
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        // loading ProductName and Category for each OrderItem
                        // TO-DO: same for the remaining endpoints
                        if (orderItem is null) continue;
                        var product = await _productsMicroserviceClient.GetProduct(orderItem.ProductId);
                        if (product is null) continue;
                        // maps existing object (does not create new object)
                        _mapper.Map<ProductDto, OrderItemResponse>(product, orderItem);
                    }
                }
                // loading UserName and Email for each Order
                // TO-DO: same for the remaining endpoints
                var user = await _usersMicroserviceClient.GetUser(order.UserId);
                if (user != null)
                {
                    // maps existing object (does not create new object)
                    _mapper.Map<UserDto, OrderResponse>(user, order);
                }
            }
            return orders.ToList();
        }

        public async Task<List<OrderResponse?>> GetOrders(FilterDefinition<Order> filter)
        {
            var fromRepo = await _repository.GetOrders(filter);
            return _mapper.Map<IEnumerable<OrderResponse?>>(fromRepo).ToList();
        }

        public async Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest is null) throw new ArgumentNullException(nameof(orderUpdateRequest));
            
            var validationResult = await _orderUpdateRequestValidator.ValidateAsync(orderUpdateRequest);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            if (orderUpdateRequest.OrderItems != null)
            {
                foreach (var orderItem in orderUpdateRequest.OrderItems)
                {
                    validationResult = await _orderItemUpdateRequestValidator.ValidateAsync(orderItem);
                    if (!validationResult.IsValid)
                    {
                        throw new ArgumentException(string.Join(',',
                            validationResult.Errors.Select(e => e.ErrorMessage)));
                    }
                    var product = await _productsMicroserviceClient.GetProduct(orderItem.ProductId);
                    if (product == null) throw new ArgumentException("Invalid Product");
                }
            }

            var user = await _usersMicroserviceClient.GetUser(orderUpdateRequest.UserId);
            if (user == null) throw new ArgumentException("Invalid User");

            var order = _mapper.Map<Order>(orderUpdateRequest);
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
            }
            order.TotalAmount = order.OrderItems.Sum(p => p.TotalPrice);

            var fromRepo = await _repository.UpdateOrder(order);
            if (fromRepo is null) return null;
            return _mapper.Map<OrderResponse>(fromRepo);

        }
    }
}
