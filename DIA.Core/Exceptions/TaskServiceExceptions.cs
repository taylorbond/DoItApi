using System;

namespace DIA.Core.Exceptions
{
    public class NoTasksFoundException : Exception
    {
        public NoTasksFoundException(string message) : base(message)
        {

        }
    }

    public class NoCommentsFoundException : Exception
    {
        public NoCommentsFoundException(string message) : base(message)
        {
            
        }
    }

    public class NoAlertsFoundException : Exception
    {
        public NoAlertsFoundException(string message) : base(message)
        {

        }
    }
}