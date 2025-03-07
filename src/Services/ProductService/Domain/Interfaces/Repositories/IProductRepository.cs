using Domain.Aggregates;
using MongoDB.Bson;

namespace Domain.Interfaces.Repositories;

public interface IProductRepository
{
    /// <summary>
    /// Thêm sản phẩm mới.
    /// </summary>
    Task AddAsync(Product product);

    /// <summary>
    /// Cập nhật sản phẩm.
    /// </summary>
    Task UpdateAsync(Product product);

    /// <summary>
    /// Xóa sản phẩm theo ID.
    /// </summary>
    Task DeleteAsync(ObjectId id);

    /// <summary>
    /// Tìm sản phẩm theo ID.
    /// </summary>
    Task<Product?> GetByIdAsync(ObjectId id);

    /// <summary>
    /// Lấy danh sách sản phẩm theo danh mục.
    /// </summary>
    Task<List<Product>> GetByCategoryIdAsync(ObjectId categoryId);

    /// <summary>
    /// Lấy danh sách sản phẩm theo thương hiệu.
    /// </summary>
    Task<List<Product>> GetByBrandIdAsync(ObjectId brandId);

    /// <summary>
    /// Tìm kiếm sản phẩm theo tên (hỗ trợ tìm kiếm gần đúng).
    /// </summary>
    Task<List<Product>> SearchByNameAsync(string keyword);

    /// <summary>
    /// Lấy danh sách sản phẩm đang còn hàng.
    /// </summary>
    Task<List<Product>> GetAvailableProductsAsync();

    /// <summary>
    /// Kiểm tra SKU có tồn tại hay chưa.
    /// </summary>
    Task<bool> IsSkuExistsAsync(string sku);
}