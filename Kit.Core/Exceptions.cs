using System;

namespace Kit
{
    public class ArgumentNullOrEmptyException : ArgumentNullException
    {
        public ArgumentNullOrEmptyException(string paramName) : base(paramName) { }
    }

    public class ArgumentNullOrWhiteSpaceException : ArgumentNullException
    {
        public ArgumentNullOrWhiteSpaceException(string paramName) : base(paramName) { }
    }
}
