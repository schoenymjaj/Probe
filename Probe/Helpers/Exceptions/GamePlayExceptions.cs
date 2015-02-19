using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Probe.Helpers.Exceptions
{
    public class GamePlayDoesNotExistException : Exception
    {
        private Exception _ex;

        public GamePlayDoesNotExistException()
        {
        }

        public GamePlayDoesNotExistException(Exception ex)
        {
            _ex = ex;
        }

        public Exception GetEx
        {
            get
            {
                return _ex;
            }
        }

    }//public class GamePlayDoesNotExistException 

    public class GamePlayNotActiveException : Exception
    {
        private Exception _ex;

        public GamePlayNotActiveException()
        {

        }

        public GamePlayNotActiveException(Exception ex)
        {
            _ex = ex;
        }

        public Exception GetEx
        {
            get
            {
                return _ex;
            }
        }
    }//public class GamePlayNotActiveException

    public class GamePlayDuplicatePlayerNameException : Exception
    {
        private Exception _ex;

        public GamePlayDuplicatePlayerNameException()
        {

        }

        public GamePlayDuplicatePlayerNameException(Exception ex)
        {
            _ex = ex;
        }

        public Exception GetEx
        {
            get
            {
                return _ex;
            }
        }
    }//public class GamePlayDuplicatePlayerNameException

    public class GamePlayInvalidFirstNameException : Exception
    {
        private Exception _ex;

        public GamePlayInvalidFirstNameException()
        {

        }

        public GamePlayInvalidFirstNameException(Exception ex)
        {
            _ex = ex;
        }

        public Exception GetEx
        {
            get
            {
                return _ex;
            }
        }
    }//public class GamePlayInvalidFirstNameException

    public class GamePlayInvalidNickNameException : Exception
    {
        private Exception _ex;

        public GamePlayInvalidNickNameException()
        {

        }

        public GamePlayInvalidNickNameException(Exception ex)
        {
            _ex = ex;
        }

        public Exception GetEx
        {
            get
            {
                return _ex;
            }
        }
    }//public class GamePlayInvalidNickNameException

    public class PlayerDTOMissingAnswersException : Exception
    {
        private Exception _ex;

        public PlayerDTOMissingAnswersException()
        {

        }

        public PlayerDTOMissingAnswersException(Exception ex)
        {
            _ex = ex;
        }

        public Exception GetEx
        {
            get
            {
                return _ex;
            }
        }
    }//public class PlayerDTOMissingAnswers

}