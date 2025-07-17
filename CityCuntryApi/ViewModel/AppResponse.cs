using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;

public class AppResponse
{
    public int Result { get; set; }
    public IEnumerable List { get; set; }
    public Object Object { get; set; }
    public string Msg { get; set; }
    public bool Error { get; set; }         // Error flag
}
