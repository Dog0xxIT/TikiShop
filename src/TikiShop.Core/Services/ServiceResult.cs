using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TikiShop.Core.Services
{
    public class ServiceResult
    {
        public bool Succeeded { get; private set; }
        public IEnumerable<string> Errors { get; private set; }

        private ServiceResult(bool success)
        {
            Succeeded = success;
            Errors = Enumerable.Empty<string>();
        }

        private ServiceResult(IEnumerable<string> errors)
        {
            Succeeded = false;
            Errors = errors;
        }

        public static ServiceResult Success => new(true);

        public static ServiceResult Failed(params string[] errors) => new(errors);

        public static ServiceResult Failed(IEnumerable<string> errors) => new(errors);
    }

    public class ServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public IEnumerable<string> Errors { get; private set; }
        public T? Data { get; private set; }

        private ServiceResult(bool success, T data)
        {
            Succeeded = success;
            Errors = Enumerable.Empty<string>();
            Data = data;
        }

        private ServiceResult(IEnumerable<string> errors)
        {
            Succeeded = false;
            Errors = errors;
        }

        public static ServiceResult<T> Success(T data) => new(true, data);

        public static ServiceResult<T> Failed(params string[] errors) => new(errors);

        public static ServiceResult<T> Failed(IEnumerable<string> errors) => new(errors);
    }
}
