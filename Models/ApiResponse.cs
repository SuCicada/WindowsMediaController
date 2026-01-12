namespace WindowsMediaController.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
    public string? Code { get; set; }

    public static ApiResponse<T> Ok(T? data, string? message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> Fail(string error, string? code = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error,
            Code = code
        };
    }
}

