using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LB.Core.Containers
{
    internal class ContainerException : ApplicationException
    {
        public ContainerException(string message) : base(message)
        {
        }

        public ContainerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ContainerException() : base("An error occurred in the container.")
        {
        }

        public ContainerException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        public ContainerException(Exception innerException) : base("An error occurred in the container.", innerException)
        {
        }
    }
}