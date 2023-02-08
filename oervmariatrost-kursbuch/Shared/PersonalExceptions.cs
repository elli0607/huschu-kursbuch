namespace oervmariatrost_kursbuch.Shared
{
    public class ContactNotFoundException : Exception
    {
        public ContactNotFoundException(string message) : base(message)
        {
        }
    }

    public class NoCoursesForUserFoundException : Exception
    {
        public NoCoursesForUserFoundException(string message) : base (message)
        {
        }
    }

    public class StrizziException : Exception
    {
        public StrizziException(string message) : base (message)
        {

        }
    }
}


