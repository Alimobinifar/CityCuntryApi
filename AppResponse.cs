using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;

public class AppResponse<T>
{
    public List<T> Data { get; set; }               // Generic list of results
    public IList NonGenericList { get; set; }       // Non-generic IList for other data
    public string Message { get; set; }             // Response message
    public int Status { get; set; }                  // Numeric status code
    public string StatusCode { get; set; }           // Status code string
    public object AdditionalData { get; set; }       // Optional additional object data
    public bool Error { get; set; } = false;          // Error flag
}
