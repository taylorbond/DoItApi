using System;

namespace DIA.Core.Exceptions
{
    public class DatabaseUpdateException : Exception
    {
        public DatabaseUpdateException(Exception e) : base(e.ToString())
        {
            
        }
    }

    public class NoDatabaseObjectFoundException : Exception
    {
        public NoDatabaseObjectFoundException(string message) : base(message)
        {
            
        }
    }
}