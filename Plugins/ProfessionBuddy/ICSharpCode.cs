using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighVoltz
{
    public enum CsharpCodeType { BoolExpression, Statements }

    public interface ICSharpCode
    {
        int CodeLineNumber { get; set; }
        string CompileError { get; set; }
        CsharpCodeType CodeType { get;  }
        string Code { get;}
        Delegate CompiledMethod { get; set; }
    }
}
