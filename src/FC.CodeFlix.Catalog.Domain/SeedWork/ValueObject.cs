namespace FC.CodeFlix.Catalog.Domain.SeedWork
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        public abstract bool Equals(ValueObject? other);
        

        public override bool Equals(object? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.GetType() == this.GetType() && Equals((ValueObject)other);
        }

        protected abstract int GetCustomHashCode();

        public override int GetHashCode()
            => GetCustomHashCode();

        public static bool operator ==(ValueObject? left, ValueObject? right)
            => left?.Equals(right) ?? false;

        public static bool operator !=(ValueObject? left, ValueObject? right)
            => !(left == right);
    }
}
