namespace TikiShop.Core.Services;

public class ServiceResult
{
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

    public bool Succeeded { get; private set; }
    public IEnumerable<string> Errors { get; private set; }

    public static ServiceResult Success => new(true);

    public static ServiceResult Failed(params string[] errors)
    {
        return new ServiceResult(errors);
    }

    public static ServiceResult Failed(IEnumerable<string> errors)
    {
        return new ServiceResult(errors);
    }
}

public class ServiceResult<T>
{
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

    public bool Succeeded { get; private set; }
    public IEnumerable<string> Errors { get; private set; }
    public T? Data { get; private set; }

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>(true, data);
    }

    public static ServiceResult<T> Failed(params string[] errors)
    {
        return new ServiceResult<T>(errors);
    }

    public static ServiceResult<T> Failed(IEnumerable<string> errors)
    {
        return new ServiceResult<T>(errors);
    }
}