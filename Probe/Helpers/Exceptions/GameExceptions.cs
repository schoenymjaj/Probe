using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Probe.Helpers.Exceptions
{
    public class GameDoesNotExistException : Exception
    {
        private Exception _ex;

        public GameDoesNotExistException()
        {
        }

        public GameDoesNotExistException(Exception ex)
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

    }//public class GameDoesNotExistException 

    public class GameNotActiveException : Exception
    {
        private Exception _ex;

        public GameNotActiveException()
        {

        }

        public GameNotActiveException(Exception ex)
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
    }//public class GameNotActiveException

    public class GameDuplicatePlayerNameException : Exception
    {
        private Exception _ex;

        public GameDuplicatePlayerNameException()
        {

        }

        public GameDuplicatePlayerNameException(Exception ex)
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
    }//public class GameDuplicatePlayerNameException

    public class GameInvalidPlayerNameException : Exception
    {
        private Exception _ex;

        public GameInvalidPlayerNameException()
        {

        }

        public GameInvalidPlayerNameException(Exception ex)
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
    }//public class GameInvalidPlayerNameException


    public class GameInvalidFirstNameException : Exception
    {
        private Exception _ex;

        public GameInvalidFirstNameException()
        {

        }

        public GameInvalidFirstNameException(Exception ex)
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
    }//public class GameInvalidFirstNameException

    public class GameInvalidNickNameException : Exception
    {
        private Exception _ex;

        public GameInvalidNickNameException()
        {

        }

        public GameInvalidNickNameException(Exception ex)
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
    }//public class GameInvalidNickNameException

    public class GameInvalidLastNameException : Exception
    {
        private Exception _ex;

        public GameInvalidLastNameException()
        {

        }

        public GameInvalidLastNameException(Exception ex)
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
    }//public class GameInvalidLastNameException

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

    public class InvalidGameAnswersException : Exception
    {
        private Exception _ex;

        public InvalidGameAnswersException()
        {

        }

        public InvalidGameAnswersException(Exception ex)
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
    }//public class InvalidGameAnswersException

    public class GameAnswersTooLateException : Exception
    {
        private Exception _ex;

        public GameAnswersTooLateException()
        {

        }

        public GameAnswersTooLateException(Exception ex)
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
    }//public class InvalidGameAnswersException

    public class GameAnswersTooEarlyException : Exception
    {
        private Exception _ex;

        public GameAnswersTooEarlyException()
        {

        }

        public GameAnswersTooEarlyException(Exception ex)
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
    }//public class GameAnswersTooEarlyException

    public class GamePlayerInActiveException : Exception
    {
        private Exception _ex;

        public GamePlayerInActiveException()
        {

        }

        public GamePlayerInActiveException(Exception ex)
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
    }//public class GamePlayerInActiveException

    public class GameHasNoQuestionsException : Exception
    {
        private Exception _ex;

        public GameHasNoQuestionsException()
        {

        }

        public GameHasNoQuestionsException(Exception ex)
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
    }//public class GameHasNoQuestionsException

    public class GameHasPlayersException : Exception
    {
        private Exception _ex;

        public GameHasPlayersException()
        {

        }

        public GameHasPlayersException(Exception ex)
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
    }//public class GameHasPlayersException

    public class GameStartGTEndDateException : Exception
    {
        private Exception _ex;

        public GameStartGTEndDateException()
        {

        }

        public GameStartGTEndDateException(Exception ex)
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
    }//public class GameStartGTEndDateException

    public class GameEndDateIsPassedException : Exception
    {
        private Exception _ex;

        public GameEndDateIsPassedException()
        {

        }

        public GameEndDateIsPassedException(Exception ex)
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
    }//public class GameEndDateIsPassedException

    public class GameInSuspendModeException : Exception
    {
        private Exception _ex;

        public GameInSuspendModeException()
        {

        }

        public GameInSuspendModeException(Exception ex)
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
    }//public class GameInSuspendModeException

    public class GameIsActiveException : Exception
    {
        private Exception _ex;

        public GameIsActiveException()
        {

        }

        public GameIsActiveException(Exception ex)
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
    }//public class GameIsActiveException

}