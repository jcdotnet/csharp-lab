using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IMapper _mapper;
        private readonly IValidator<ProductAddRequest> _addValidator;
        private readonly IValidator<ProductUpdateRequest> _updateValidator;

        private readonly IProductsRepository _repository;
        private readonly IRabbitMQPublisher _rabbitmqPublisher;

        public ProductsService(IValidator<ProductAddRequest> addValidator,
            IValidator<ProductUpdateRequest> updateValidator,
            IMapper mapper,
            IProductsRepository repository,
            IRabbitMQPublisher rabbitmqPublisher)
        {
            _addValidator = addValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
            _repository = repository;
            _rabbitmqPublisher = rabbitmqPublisher;
        }

        public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
        {
            if (productAddRequest == null)
            {
                throw new ArgumentNullException(nameof(productAddRequest));
            }
            var validationResult = await _addValidator.ValidateAsync(productAddRequest);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(error => error.ErrorMessage));
                throw new ArgumentException(errors);
            }
            var product = _mapper.Map<Product>(productAddRequest);
            var fromRepo = await _repository.AddProduct(product);

            if (fromRepo is null) return null;
            return _mapper.Map<ProductResponse>(fromRepo);
        }

        public async Task<bool> DeleteProduct(Guid productId)
        {
            var fromRepo = await _repository.GetProduct(product => product.ProductId == productId);

            if (fromRepo == null) return false;

            var isDeleted = await _repository.DeleteProduct(productId);

            if (isDeleted)
            {
                string routingKey = "product.delete";
                ProductDeletionMessage message = new(productId, fromRepo.ProductName);
                await _rabbitmqPublisher.Publish(routingKey, message);
            }

            return isDeleted;
        }

        public async Task<ProductResponse?> GetProduct(Expression<Func<Product, bool>> condition)
        {
            var fromRepo = await _repository.GetProduct(condition);

            if (fromRepo is null) return null;
            return _mapper.Map<ProductResponse>(fromRepo);
        }

        public async Task<List<ProductResponse>> GetProducts()
        {
            var fromRepo = await _repository.GetProducts();

            return _mapper.Map<IEnumerable<ProductResponse>>(fromRepo).ToList();

        }

        public async Task<List<ProductResponse?>> GetProducts(Expression<Func<Product, bool>> condition)
        {
            var fromRepo = await _repository.GetProducts(condition);

            return _mapper.Map<IEnumerable<ProductResponse?>>(fromRepo).ToList();
        }

        public async Task<ProductResponse> UpdateProduct(ProductUpdateRequest productUpdateRequest)
        {
            var fromRepo = await _repository.GetProduct(
                product => product.ProductId == productUpdateRequest.ProductId
            );

            if (fromRepo == null) throw new ArgumentException("Invalid Product ID");

            var validationResult = await _updateValidator.ValidateAsync(productUpdateRequest);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(error => error.ErrorMessage));
                throw new ArgumentException(errors);
            }

            bool isNameChanged = productUpdateRequest.ProductName != fromRepo.ProductName;

            var product = _mapper.Map<Product>(productUpdateRequest);

            var updatedProduct = await _repository.UpdateProduct(product);

            if (isNameChanged)
            {
                string routingKey = "product.update.name";

                //var message = new
                //{
                //    ProductId = product.ProductId,
                //    UpdatedProductName = product.ProductName,
                //};
                // using record instead of anonymous object
                var message = new ProductNameUpdateMessage(product.ProductId, product.ProductName);

                await _rabbitmqPublisher.Publish(routingKey, message);
            }

            return _mapper.Map<ProductResponse>(updatedProduct);
        }
    }
}
