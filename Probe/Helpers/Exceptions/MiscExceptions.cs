using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Probe.Helpers.Exceptions
{
    public class JavaScriptException : Exception
    {
        public JavaScriptException(string message)
            : base(message)
        {
        }
    }

    //this is for logging on Elmah (work around); so Elmah specifies the type
    //as "Action"
    public class ActionException : Exception
    {
        public ActionException(string message)
            : base(message)
        {
        }
    }

    public class ApiArgException : Exception
    {
        public ApiArgException(string message)
            : base(message)
        {
        }
    }
   
}