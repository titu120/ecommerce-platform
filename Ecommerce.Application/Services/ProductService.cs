using AutoMapper;
using Ecommerce.Application.DTOs.Product;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using FluentValidation;

namespace Ecommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateProductDto> createValidator,
            IValidator<UpdateProductDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            // Step 1: Validation
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(validationResult.Errors);
            }

            // Step 2: DTO -> Entity
            var product = _mapper.Map<Product>(dto);

            // Step 3: Repository তে Add
            await _unitOfWork.Products.AddAsync(product);

            // Step 4: Database তে Save
            await _unitOfWork.SaveChangesAsync();

            // Step 5: Category সহ আবার লোড করা (যাতে CategoryName ঠিকমতো ম্যাপ হয়)
            var createdProduct = await _unitOfWork.Products.GetProductWithCategoryAsync(product.Id);

            // Step 6: Entity -> DTO, রিটার্ন
            return _mapper.Map<ProductDto>(createdProduct);
        }

        public Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold)
        {
            throw new NotImplementedException();
        }
    }
}