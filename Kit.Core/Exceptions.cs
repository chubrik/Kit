using System;

namespace Kit
{
    public class ArgumentNullOrEmptyException : ArgumentException
    {
        public ArgumentNullOrEmptyException(string message) : base(message) { }
    }

    public class ArgumentNullOrWhiteSpaceException : ArgumentException
    {
        public ArgumentNullOrWhiteSpaceException(string message) : base(message) { }
    }
}
