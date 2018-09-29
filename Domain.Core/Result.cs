using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FreedomFridayServerless.Domain.Core
{
    [DataContract]
    public class Result
    {   
        protected Result()
        {

        }

        public Result(bool succeded, string message = "")
        {
            IsSuccess = succeded;
            ErrorMessage = message;
        }
        public bool IsFailure
        {
            get { return !IsSuccess; }
        }

        [DataMember]
        public string ErrorMessage { get; private set; }

        [DataMember]
        public Exception Exception { get; protected set; }

        [DataMember]
        public bool IsSuccess { get; protected set; }

        [DataMember]
        public bool ContainsFaultyValue { get; protected set; }

        public virtual object GetValue()
        {
            return new object();
        }

        public static Result Ok()
        {
            return new Result(true);
        }

        public static Result Failure(string message)
        {
            return new Result(false, message);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value);
        }

        public static Result<T> Fail<T>(string errorMessage)
        {
            return new Result<T>(errorMessage);
        }

        public static Result<T> Fail<T>(Exception exception)
        {
            return new Result<T>(exception);
        }

        public static Result<T> Fail<T>(T faultyValue, string errorMessage)
        {
            return new Result<T>(faultyValue, errorMessage);
        }

        public static Result Combine(params Result[] results)
        {
            foreach (Result result in results)
                if (result.IsFailure)
                    return result;
            return Ok();
        }
    }

    [DataContract]
    public class Result<T> : Result
    {
        [DataMember]
        private T _value;

        public T Value
        {
            get
            {
                if (IsFailure && !ContainsFaultyValue)
                    throw new AggregateException(Exception);
                return _value;
            }
            set {
                _value = value;
            }
        }

        private Result() : base ()
        {
            
        }

        public Result(T value)
            : base(true)
        {
            if (value == null)
            {
                IsSuccess = false;
                Exception = new NullReferenceException();
            }
            else
            {
                _value = value;
                Exception = null;
            }
        }

        public Result(T faultyValue, string errorMessage)
            : base(false, errorMessage)
        {
            _value = faultyValue;
            ContainsFaultyValue = true;

        }

        public Result(Exception ex)
            : base(false, ex.Message)
        {
            Exception = ex ?? throw new ArgumentNullException("ex");
            _value = default(T);
        }
        public Result(string errorMessage)
            : base(false, errorMessage)
        {
            _value = default(T);
            Exception = new Exception(errorMessage);
        }

        public override object GetValue()
        {
            return this.Value;
        }

        public static implicit operator T(Result<T> result)
        {
            if (result.IsSuccess)
                return result.Value;
            throw result.Exception ?? new UnwrapResultException<T>(result);
        }

        public static implicit operator Result<T>(Exception exception)
        {
            return new Result<T>(exception);
        }
    }

    public class ResultException : Exception
    {
        public ResultException(string message) : base(message)
        {

        }
        public ResultException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }

    public class UnwrapResultException<T> : Exception
    {
        public UnwrapResultException(Result result) 
            : base(String.Format("Failed to extract value of type {0} from a broken result. Exception message: {1}", typeof(T).Name, result.ErrorMessage)) { }
    }

     public static class ResultEx
    {
        public static Result OnFailure(this Result result, Action<Exception> onFailure)
        {
            if (result.IsFailure)
                onFailure(result.Exception);
            return result;
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Action<Exception> onFailure)
        {
            if (result.IsFailure)
                onFailure(result.Exception);
            return result;
        }


        public static Result OnFailure(this Result result, Action onFailure)
        {
            if (result.IsFailure)
                onFailure();
            return result;
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Func<Result<T>, Result<T>> onFailure)
        {
            if (result.IsFailure)
                onFailure(result);
            return result;
        }

        public static Result OnBoth(this Result result, Action onBoth)
        {
            onBoth();
            return result;
        }

        public static Result<T> OnBoth<T>(this Result result, Func<Result<T>> onBoth)
        {
            return onBoth();
        }

        public static Result<T> OnBoth<T>(this Result<T> result, Func<Result<T>, Result<T>> onBoth)
        {
            return onBoth(result);
        }

        public static Result<U> OnBoth<T, U>(this Result<T> result, Func<Result<T>, Result<U>> onBoth)
        {
            return onBoth(result);
        }

        public static Result OnBoth(this Result result, Action<Result> onBoth)
        {
            onBoth(result);
            return result;
        }

        public static async Task<Result<T>> LogIfFailedResultAsync<T>(this Task<Result<T>> resultTask)
        {
            var result = await resultTask;
            return result;
        }

        public static async Task<HttpResponseMessage> ToEmptyHttpResponseAsync<T>(this Task<Result<T>> resultTask)
        {
            var result = await resultTask;
            return result.ToEmptyHttpResponse();
        }

        public static HttpResponseMessage ToEmptyHttpResponse<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public static async Task<HttpResponseMessage> ToHttpResponseAsync<T>(this Task<Result<T>> resultTask, Func<Result<T>, string> resultToString)
        {
            var result = await resultTask;
            return result.ToHttpResponse(resultToString);
        }

        public static HttpResponseMessage ToHttpResponse<T>(this Result<T> result, Func<Result<T>, string> resultToString)
        {
            if (result.IsSuccess)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                response.Content = new StringContent(resultToString(result));
                return response;
            }
            else
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                response.Content = new StringContent(resultToString(result));
                return response;
            }
        }

        public static Result<T> Match<T>(this Result<bool> result, Func<Result<T>> onTrue, Func<Result<T>> onFalse)
        {
            return result.Value ? onTrue() : onFalse();
        }

        public static Task<Result<T>> MatchAsync<T>(this Result<bool> result, Func<Task<Result<T>>> onTrue, Func<Task<Result<T>>> onFalse)
        {
            return result.Value ? onTrue() : onFalse();
        }

        public static Result<T> Match<T>(this Result<T> result, Func<Result<T>, Result<T>> onSuccess, Func<Exception, Result<T>> onFailure)
        {
            return result.IsSuccess ? onSuccess(result) : onFailure(result.Exception);
        }

        public static Result<U> Match<T, U>(this Result<T> result,
            Func<T, Result<U>> onSuccess,
            Func<Exception, Result<U>> onFailure)
        {
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Exception);
        }

        public static U Match<T, U>(this Result<T> result,
            Func<T, U> onSuccess,
            Func<Exception, U> onFailure)
        {
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Exception);
        }

        public static Result<V> SelectMany<T, U, V>(this Result<T> result, Func<T, Result<U>> select, Func<T, U, V> bind)
        {
            if (select == null) throw new ArgumentNullException("select");
            if (bind == null) throw new ArgumentNullException("bind");

            var resultT = result;
            if (resultT.IsFailure)
                return new Result<V>(resultT.Exception);

            Result<U> resultU;
            resultU = select(resultT.Value);
            if (resultU.IsFailure)
                return new Result<V>(resultU.Exception);
            V resultValueV;
            try
            {
                resultValueV = bind(resultT.Value, resultU.Value);
            }
            catch (Exception ex)
            {
                return new Result<V>(ex);
            }
            return new Result<V>(resultValueV);
        }

        public static async Task<Result<V>> SelectMany<T, U, V>(this Task<Result<T>> result, Func<T, Task<Result<U>>> select, Func<T, U, V> bind)
        {
            if (select == null) throw new ArgumentNullException("select");
            if (bind == null) throw new ArgumentNullException("bind");

            var resultT = await result;
            if (resultT.IsFailure)
                return new Result<V>(resultT.Exception);

            Result<U> resultU;
            try
            {
                resultU = await select(resultT.Value);
            }
            catch (Exception ex)
            {
                return new Result<V>(ex);
            }

            if (resultU.IsFailure)
                return new Result<V>(resultU.Exception);
            V resultValueV;
            try
            {
                resultValueV = bind(resultT.Value, resultU.Value);
            }
            catch (Exception ex)
            {
                return new Result<V>(ex);
            }
            return new Result<V>(resultValueV);
        }

        public static Result<T> ToResult<T>(this Result result, Func<T> onSuccess, bool containsFaultyValue = false)
		{
			return result.IsFailure
		        ? !containsFaultyValue ? Result.Fail<T>(result.ErrorMessage) : Result.Fail<T>(onSuccess(), result.ErrorMessage)
                : Result.Ok(onSuccess());
		}

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsFailure) return ToErrorResult(result);

            return new OkObjectResult(result);
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsFailure) return ToErrorResult(result);

            return new OkObjectResult((T) result);
        }

        public static IActionResult ToActionResult<T>(this Result result, Func<T> success)
        {
            if (result.IsFailure) return ToErrorResult(result);

            var resultContent = success();
            return new OkObjectResult(resultContent);
        }

        public static async Task<IActionResult> ToActionResultAsync<T, U>(this Task<Result<T>> resultTask,
            Func<T, Task<U>> onSuccess) 
        {
            var result = await resultTask.ConfigureAwait(false);
            if (result.IsFailure) return ToErrorResult(result);

            var resultContent = await onSuccess(result.Value).ConfigureAwait(false);
            return new OkObjectResult(resultContent);
        }

        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Result<T>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.IsFailure ? ToErrorResult(result) : new OkObjectResult(result.Value);
        }

        private static IActionResult ToErrorResult(Result result)
        {
            var error = result.ErrorMessage.ToEnum<KnownErrors>();
            if (error == KnownErrors.Unknown)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            return new BadRequestObjectResult(error.ToStringValue());
        }
    }

    public static class OnSuccessEx
    {
        public static Result<T> OnSuccess<T>(this Result<T> result, Action onSuccess)
        {
            if (result.IsFailure)
                return result;
            onSuccess();
            return result;
        }

        public static Result OnSuccess(this Result result, Func<Result> onSuccess)
        {
            if (result.IsFailure)
                return result;
            return onSuccess();
        }

        public static Result OnSuccess(this Result result, Action onSuccess)
        {
            if (result.IsFailure)
                return result;
            onSuccess();
            return result;
        }

        public static Result<T> OnSuccess<T>(this Result<T> result, Func<T, Result<T>> onSuccess)
        {
            return result.IsFailure ? result : onSuccess(result.Value);
        }

        public static Result<U> OnSuccess<T, U>(this Result<T> result, Func<T, Result<U>> onSuccess)
        {
            return result.IsFailure ? ConvertFailedResult<T, U>(result) : onSuccess(result.Value);
        }

        public static Result<U> ConvertFailedResult<T, U>(Result<T> result)
        {
            return result.Exception == null ? new Result<U>(result.ErrorMessage) : new Result<U>(result.Exception);
        }
    }
}