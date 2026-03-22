using System;

namespace ModelLayer.Models;

public class ResponseModel<T>
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } =  " ";

    public T? Data { get; set; } = default(T);
}
