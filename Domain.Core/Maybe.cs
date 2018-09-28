using System;

namespace FreedomFridayServerless.Domain.Core
{
    public struct Maybe<T> where T : class
    {
        public static readonly Maybe<T> Nothing = new Maybe<T>();

        private readonly T _value;
        private readonly bool _hasValue;


        public Maybe(T value)
        {
            if (value == null)
            {
                _hasValue = false;
                _value = default(T);
            }
            else
            {
                _value = value;
                _hasValue = true;
            }
        }

        public T Value
        {
            get
            {
                return _value;
            }
        }

        public bool HasValue
        {
            get { return _hasValue; }
        }
    }

    public static class MaybeEx
    {
        public static Maybe<T> ToMaybe<T>(this T value) where T: class
        {
            return new Maybe<T>(value);
        }

        public static Maybe<U> SelectMany<T, U>(this Maybe<T> m, Func<T, Maybe<U>> k) 
            where T: class
            where U : class
        {
            return !m.HasValue ? Maybe<U>.Nothing : k(m.Value);
        }

        public static Result<T> ToResult<T>(this Maybe<T> maybe, string errorMessage) where T: class
        {
            return maybe.HasValue ? Result.Ok(maybe.Value) : Result.Fail<T>(errorMessage);
        }

        public static Result<T> ToResult<T>(this Maybe<T> maybe, string errorMessageFormat, params object[] args) where T : class
        {
            return maybe.HasValue ? Result.Ok(maybe.Value) : Result.Fail<T>(string.Format(errorMessageFormat, args));
        }

        public static U Match<T, U>(this Maybe<T> maybe, Func<T, U> onSuccess,Func<U> onFailure) where T : class
        {
            return maybe.HasValue ? onSuccess(maybe.Value) : onFailure();
        }

    }
}