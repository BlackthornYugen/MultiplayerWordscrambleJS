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
        string HostGame(string playerName, string wordToScramble);

		// Player ‘playerName’ tries to join the game
        // The function returns a Word object containing the host’s (un)scrambled words
        // Exception: maximum number of players reached
        [FaultContract(typeof(MaximumNumberOfPlayersReachedFault))]
        // Exception: maximum number of players reached
        [FaultContract(typeof(PlayerAlreadyInGameFault))]
        // Exception: host cannot join the game
        [FaultContract(typeof(HostCannotJoinTheGameFault))] // TODO: Figure out when this happends
        // Exception: nobody is hosting the game
        [FaultContract(typeof(GameIsNotBeingHostedFault))]
		[OperationContract]
		Word Join(string playerName);

		// Player ‘playerName’ guesses word ‘guessedWord’ compared with word ‘unscrambledWord’
		// Returns true if ‘guessedWord’ is identical to ‘unscrambledWord’ or false otherwise
		// The function returns the name of the person hosting the game 
		// Exception: user is not playing the game 
        [FaultContract(typeof(PlayerNotPlayingTheGameFault))]
        // Exception: scrambled word on client doesn't match server
        [FaultContract(typeof(WordMismatchFault))]
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
    public class GameBeingHostedFault
    {
        string _message = "There is already a game being hosted by {0}.";

        [DataMember]
        public string HostPlayer { get; set; }

        [DataMember]
        public string Message
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
    public class MaximumNumberOfPlayersReachedFault
    {
        string _message = "You cannot join the game because the capacity of {0} was reached.";
        int _capacity;

        [DataMember]
        public int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }

        [DataMember]
        public string Message
        { 
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
    public class PlayerAlreadyInGameFault
    {
        [DataMember]
        public string Message = "The player has already joined that game.";
    }

    [DataContract]
    public class HostCannotJoinTheGameFault
    {
        [DataMember]
        public string Message = "The server lists you as the host. You may not join a game that you are hosting.";
    }

    [DataContract]
    public class GameIsNotBeingHostedFault
    {
        [DataMember]
        public string Message = "The server is not currently hosting a game.";
    }

    [DataContract]
    public class PlayerNotPlayingTheGameFault
    {
        [DataMember]
        public string Message = "The active game does not list you as a partisipant.";
    }



    [DataContract]
    public class WordMismatchFault
    {
        private string _message = "The client gave the scrambled word \"{0}\" but the server has it as \"{1}\".";

        [DataMember]
        public string Message
        {
            get
            {
                return String.Format(_message, ClientScrambled, ServerScrambled);
            }
            protected set { _message = value; }
        }

        [DataMember]
        public string ClientScrambled;

        [DataMember]
        public string ServerScrambled;
    }
}