using System;

namespace TO.Domains.Models
{
    public abstract class AggregateRootBase<TKey> : IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }

        protected AggregateRootBase(TKey id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
        }

        public virtual void ValidateState()
        {
            if (Id.Equals(default(TKey)))
                throw new InvalidOperationException("Aggregate root must have a valid ID");
        }

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        // 其他通用聚合根方法...
    }
}
