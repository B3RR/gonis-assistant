using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gonis.Assistant.Core.Exceptions
{
    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCodeException(string message, int statusCode)
            : base(message) => this.StatusCode = statusCode;

        public virtual int StatusCode { get; set; }

        public string ContentType { get; set; } = @"text/plain; charset=utf-8";
    }
}