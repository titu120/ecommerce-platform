using Ecommerce.Application.Interfaces;

public async Task DeleteProductAsync(int id)
{
    // Step 1: Product খুঁজে বের করা
    var product = await _unitOfWork.Products.GetByIdAsync(id);
    if (product == null)
    {
        throw new Ecommerce.Application.Exceptions.NotFoundException("Product", id);
    }

    // Step 2: Repository তে Delete mark করা
    _unitOfWork.Products.Delete(product);

    // Step 3: Database তে Save
    await _unitOfWork.SaveChangesAsync();
}