using System;

namespace PAP.Generators
{
    public sealed class GenerationValidationException : Exception
    {
        public GenerationValidationException(string message) : base(message)
        {
        }
    }
}
