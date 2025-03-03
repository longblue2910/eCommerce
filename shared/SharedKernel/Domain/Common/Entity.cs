namespace SharedKernel.Common;


/*
 * IDomainEvent là một interface đại diện cho sự kiện miền (Domain Event) trong mô hình DDD.
 * OccurredOn: Thuộc tính này lưu thời gian sự kiện xảy ra.
 * Mục đích: Dùng để đánh dấu một sự kiện quan trọng trong domain, giúp các service khác có thể phản ứng lại.
 */
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}


/*
 * Entity<TId> là một lớp cơ sở cho mọi entity trong hệ thống.
 * Id: Mỗi entity đều có một ID duy nhất.
 */
public abstract class Entity<TId>(TId id)
{
    public TId Id { get; protected set; } = id;

    /// <summary>
    /// _domainEvents: Danh sách chứa các sự kiện miền mà entity phát ra.
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    ///DomainEvents: Cung cấp danh sách sự kiện nhưng chỉ đọc để tránh bị sửa trực tiếp.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// AddDomainEvent: Dùng để thêm một sự kiện miền vào danh sách _domainEvents.
    /// </summary>
    /// <param name="domainEvent"></param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Khi một entity được lưu vào database, hệ thống có thể phát các domain events. Sau đó, chúng cần được xóa để tránh xử lý trùng lặp.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// So sánh entity dựa vào Id. Equals(): Hai entity được coi là giống nhau nếu có cùng Id.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> entity) return false;
        return EqualityComparer<TId>.Default.Equals(Id, entity.Id);
    }

    /// <summary>
    /// GetHashCode(): Dùng Id để tạo mã băm
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return EqualityComparer<TId>.Default.GetHashCode(Id);
    }
}


/*
 * AggregateRoot<TId> kế thừa từ Entity<TId>, đại diện cho một Aggregate Root trong mô hình DDD.
   Aggregate Root là một thực thể chính của một tập hợp các thực thể liên quan.
    Ví dụ: Order là Aggregate Root của OrderItem, Customer là Aggregate Root của Address.
 */
public abstract class AggregateRoot<TId>(TId id) : Entity<TId>(id)
{

}

/*
 * 🔥 Tóm tắt
    IDomainEvent: Interface đánh dấu sự kiện miền.
    Entity<TId>: Lớp cơ sở cho mọi thực thể, có ID, quản lý domain events.
    AggregateRoot<TId>: Định nghĩa Aggregate Root, đóng vai trò là điểm truy cập chính của một tập hợp các entity.
*
*/