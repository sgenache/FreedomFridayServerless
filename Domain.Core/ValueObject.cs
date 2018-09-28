using System;

namespace FreedomFridayServerless.Domain.Core
{
    public abstract class ValueObject<T> : IEquatable<T>
        where T: ValueObject<T>
    {
        protected abstract bool EqualsCore(T other);
        protected abstract int GetHashCodeCore();

	    public bool Equals(T other)
	    {
			if (ReferenceEquals(null, other)) return false;
		    if (ReferenceEquals(this, other)) return true;
		    return this.EqualsCore(other);
	    }

	    public override bool Equals(object obj)
        {
            var valueObj = obj as T;

            if (ReferenceEquals(valueObj, null))
                return false;

            return EqualsCore(valueObj);
        }

        public override int GetHashCode()
        {
            return GetHashCodeCore();
        }

        public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject<T> a, ValueObject<T> b)
        {
            return !(a == b);
        }
    }
}