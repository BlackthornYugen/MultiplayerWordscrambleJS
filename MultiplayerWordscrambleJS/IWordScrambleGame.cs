using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MultiplayerWordscrambleJS
{
	[ServiceContract]
	public interface IWordScrambleGame
	{
		// Returns true if the game is already being hosted or false otherwise 
		[OperationContract]
		bool IsGameBeingHosted();

		// User ‘userName’ tries to host the game with word ‘wordToScramble’
		// The function returns the scrambled word
		// Exception: game is already being hosted by someone else
        [FaultContract(typeof(GameBeingHostedFault))]
		[OperationContract]
        string HostGame(string playerName, string hostAddress, string wordToScramble);

		// Player ‘playerName’ tries to join the game
		// The function returns a Word object containing the host’s (un)scrambled words
		// Exception: maximum number of players reached
        [FaultContract(typeof(MaximumNumberOfPlayersReachedFault))]
		// Exception: host cannot join the game
		// Exception: nobody is hosting the game
		[OperationContract]
		Word Join(string playerName);

		// Player ‘playerName’ guesses word ‘guessedWord’ compared with word ‘unscrambledWord’
		// Returns true if ‘guessedWord’ is identical to ‘unscrambledWord’ or false otherwise
		// The function returns the name of the person hosting the game 
		// Exception: user is not playing the game 
		[OperationContract]
		bool GuessWord(string playerName, Word gameWords);
	}

	[DataContract]
	public class Word
	{
		[DataMember]
		public string UnscrambledWord; // word typed by the game’s host
		[DataMember]
		public string ScrambledWord;
	}

    [DataContract]
    public abstract class GameFault
    {
        public string Message { get; protected set; }
    }

    [DataContract]
    public class GameBeingHostedFault : GameFault
    {
        string _message = "There is already a game being hosted by {0}.";

        public string HostPlayer { get; set; }

        public new string Message
        {
            get
            {
                return string.Format(_message, HostPlayer);
            }
            protected set
            {
                _message = value;
            }
        }
    }

    [DataContract]
    public class MaximumNumberOfPlayersReachedFault : GameFault
    {
        string _message = "You cannot join the game because the capacity of {0} was reached.";
        int capacity;

        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }

        public new string Message { 
            get 
            {
                return string.Format(_message, Capacity);
            }
            protected set
            {
                _message = value;
            }
        }
    }

    [DataContract]
    public class HostCannotJoinTheGameFault : GameFault
    {
        public HostCannotJoinTheGameFault()
        {
            Message = "The server lists you as the host. You may not join a game that you are hosting.";
        }
    }

    [DataContract]
    public class GameIsNotBeingHostedFault : GameFault
    {
        public GameIsNotBeingHostedFault()
        {
            Message = "The server is not currently hosting a game.";
        }
    }

    [DataContract]
    public class PlayerNotPlayingTheGameFault : GameFault
    {
        public PlayerNotPlayingTheGameFault()
        {
            Message = "The active game does not list you as a partisipant.";
        }
    }



    [DataContract]
    public class WordMismatchFault : GameFault
    {
        public WordMismatchFault()
        {
            Message = "The active game does not list you as a partisipant.";
        }
    }
}