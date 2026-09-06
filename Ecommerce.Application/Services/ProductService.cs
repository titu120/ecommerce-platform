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

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            // Step 1: Validation
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(validationResult.Errors);
            }

            // Step 2: Product খুঁজে বের করা
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Product", id);
            }

            // Step 3: Field গুলো আপডেট করা
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;

            // Step 4: Repository তে Update mark করা
            _unitOfWork.Products.Update(product);

            // Step 5: Database তে Save
            await _unitOfWork.SaveChangesAsync();

            // Step 6: Category সহ আবার লোড করা
            var updatedProduct = await _unitOfWork.Products.GetProductWithCategoryAsync(product.Id);

            // Step 7: Entity -> DTO, রিটার্ন
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            var products = await _unitOfWork.Products.GetPagedProductsAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
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