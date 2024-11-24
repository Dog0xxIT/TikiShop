using TikiShop.Model.Enums;

namespace TikiShop.Model.DTO;

public class ResultObject<T>
{
    public ResultCode ResultCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ResultObject<T> Success()
    {
        return new ResultObject<T>
        {
            Data = default,
            ResultCode = ResultCode.Success,
            Message = null
        };
    }

    public static ResultObject<T> Success(T data)
    {
        return new ResultObject<T>
        {
            Data = data,
            ResultCode = ResultCode.Success,
            Message = null
        };
    }

    public static ResultObject<T> Success(T data, string message)
    {
        return new ResultObject<T>
        {
            Data = data,
            ResultCode = ResultCode.Success,
            Message = message
        };
    }

    public static ResultObject<T> Failed(string message)
    {
        return new ResultObject<T>
        {
            Data = default,
            ResultCode = ResultCode.Failure,
            Message = message
        };
    }

    public static ResultObject<T> Failed(IEnumerable<string> errors)
    {
        return new ResultObject<T>
        {
            Data = default,
            ResultCode = ResultCode.Failure,
            Message = errors?.FirstOrDefault()
        };
    }
}