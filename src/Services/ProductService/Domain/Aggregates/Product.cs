using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SharedKernel.Common;

namespace Domain.Aggregates;

/// <summary>
/// Aggregate Root: Đại diện cho một sản phẩm trong hệ thống eCommerce.
/// </summary>
/// <remarks>
/// Constructor của Product.
/// </remarks>
[BsonIgnoreExtraElements]
public class Product(ObjectId id, 
    string name, 
    string description, 
    decimal price, 
    int stockQuantity, 
    string sku, 
    ObjectId categoryId, 
    ObjectId brandId, 
    List<ProductImage> images = null, 
    List<ProductAttribute> attributes = null,
    List<Review> reviews = null, 
    List<ProductVariant> variants = null, 
    Discount discount = null,
    List<Supplier> suppliers = null) : AggregateRoot<ObjectId>(id)
{
    /// <summary>
    /// Tên sản phẩm.
    /// </summary>
    [BsonElement("name")]
    public string Name { get; private set; } = name;

    /// <summary>
    /// Mô tả sản phẩm.
    /// </summary>
    [BsonElement("description")]
    public string Description { get; private set; } = description;

    /// <summary>
    /// Giá của sản phẩm.
    /// </summary>
    [BsonElement("price")]
    public decimal Price { get; private set; } = price;

    /// <summary>
    /// Số lượng tồn kho.
    /// </summary>
    [BsonElement("stockQuantity")]
    public int StockQuantity { get; private set; } = stockQuantity;

    /// <summary>
    /// Mã SKU (Stock Keeping Unit) - Mã sản phẩm duy nhất.
    /// </summary>
    [BsonElement("sku")]
    public string SKU { get; private set; } = sku;

    /// <summary>
    /// Trạng thái sản phẩm (có hoạt động hay không).
    /// </summary>
    [BsonElement("isActive")]
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// ID của danh mục mà sản phẩm thuộc về.
    /// </summary>
    [BsonElement("categoryId")]
    public ObjectId CategoryId { get; private set; } = categoryId;

    /// <summary>
    /// Danh sách hình ảnh của sản phẩm.
    /// </summary>
    [BsonElement("images")]
    public List<ProductImage> Images { get; private set; } = images ?? [];

    /// <summary>
    /// Danh sách thuộc tính sản phẩm (màu sắc, kích thước, v.v.).
    /// </summary>
    [BsonElement("attributes")]
    public List<ProductAttribute> Attributes { get; private set; } = attributes ?? [];

    /// <summary>
    /// Ngày tạo sản phẩm.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Ngày cập nhật cuối cùng.
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; private set; }

    [BsonElement("brandId")]
    public ObjectId BrandId { get; private set; } = brandId;

    [BsonElement("reviews")]
    public List<Review> Reviews { get; private set; } = reviews ?? [];

    [BsonElement("variants")]
    public List<ProductVariant> Variants { get; private set; } = variants ?? [];

    [BsonElement("discount")]
    public Discount? Discount { get; private set; } = discount;

    [BsonElement("suppliers")]
    public List<Supplier> Suppliers { get; private set; } = suppliers ?? [];


    /// <summary>
    /// Cập nhật thông tin sản phẩm.
    /// </summary>
    public void UpdateProduct(string name, string description, decimal price, int stockQuantity, ObjectId categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Vô hiệu hóa sản phẩm.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Kích hoạt lại sản phẩm.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateBrand(ObjectId brandId)
    {
        BrandId = brandId;
        UpdatedAt = DateTime.Now;
    }

    public void AddReview(Review review)
    {
        Reviews.Add(review);
        UpdatedAt = DateTime.Now;
    }

    public void AddVariant(ProductVariant variant)
    {
        Variants.Add(variant);
        UpdatedAt = DateTime.Now;
    }

    public void AddSupplier(Supplier supplier)
    {
        Suppliers.Add(supplier);
        UpdatedAt = DateTime.Now;
    }



}
