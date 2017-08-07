using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaitingRoomBigScreen.WebService
{
    /// <summary>
    ///     封装操作返回结果和错误信息
    /// </summary>
    public class Result
    {
        public Result()
        {
        }

        public Result(bool success, string error)
        {
            IsSuccess = success;
            Message = error;
        }

        public Result(bool success, string error, Exception exception) : this(success, error)
        {
            Exception = exception;
        }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(IsSuccess ? "Success" : "Fail");
            if (!string.IsNullOrEmpty(Message))
                sb.Append(" " + Message);
            return sb.ToString();
        }

        public static Result Success()
        {
            return new Result(true, string.Empty);
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result Fail(string message, Exception exception)
        {
            return new Result(false, message, exception);
        }
    }

    /// <summary>
    ///     封装操作结果 返回数据和错误信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        public Result()
        {
        }

        public Result(bool success, string error) : base(success, error)
        {
        }

        public Result(bool success, string error, Exception exception) : base(success, error, exception)
        {
        }

        public Result(bool success, string error, T value) : base(success, error)
        {
            Value = value;
        }

        public Result(bool success, string error, Exception exception, T value)
            : base(success, error, exception)
        {
            Value = value;
        }

        public T Value { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(IsSuccess ? "Success" : "Fail");
            if (!string.IsNullOrEmpty(Message))
                sb.Append(" " + Message);
            sb.Append($"Value={Value}");
            return sb.ToString();
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, string.Empty, value);
        }

        public new static Result<T> Fail(string message)
        {
            return new Result<T>(false, message);
        }

        public new static Result<T> Fail(string message, Exception exception)
        {
            return new Result<T>(false, message, exception);
        }

        public static Result<T> Convert(Result res)
        {
            return new Result<T>(res.IsSuccess, res.Message, res.Exception);
        }

        public static Result<T> Convert<T2>(Result<T2> res)
        {
            if (typeof(T2) == typeof(Result))
                return new Result<T>(res.IsSuccess, res.Message, res.Exception, default(T));
            return new Result<T>(res.IsSuccess, res.Message, res.Exception, res.Value == null ? default(T) : (T)(object)res.Value);
        }
    }
}
