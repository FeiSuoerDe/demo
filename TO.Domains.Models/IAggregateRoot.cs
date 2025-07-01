namespace TO.Domains.Models
{
    public interface IAggregateRoot<TKey> : IDisposable
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }
        
        // 统一访问控制方法
        void ValidateState();
        void MarkAsUpdated();
    }
}
