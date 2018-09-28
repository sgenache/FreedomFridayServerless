namespace FreedomFridayServerless.Domain.Core
{
    public abstract class Entity
    {
        public virtual string Id { get; protected set; }

        public override bool Equals(object obj)
        {
            var other = obj as Entity;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(other.Id))
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a==b);
        }

        //check http://bit.ly/1FSzTg1 for explanation
        public override int GetHashCode()
        {
            return (GetType() + Id).GetHashCode();
        }
    }
}