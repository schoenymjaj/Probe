﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Probe.Helpers.Exceptions;
using Elmah;

namespace Probe.Controllers
{
    public class ErrorController : Controller
    {
        public void LogJavaScriptError(string message)
        {
            ErrorSignal
                .FromCurrentContext()
                .Raise(new JavaScriptException(message));
        }
    }

}