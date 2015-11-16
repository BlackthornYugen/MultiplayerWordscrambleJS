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
		// The function returns the name of the person hosting the game 
		// Exception: game is already being hosted by someone else
		[OperationContract]
        string HostGame(string playerName, string hostAddress, string wordToScramble);

		// Player ‘playerName’ tries to join the game
		// The function returns a Word object containing the host’s (un)scrambled words
		// Exception: maximum number of players reached
		// Exception: host cannot join the game
		// Exception: nobody is hosting the game
		[OperationContract]
		Word Join(string playerName);

		// Player ‘playerName’ guesses word ‘guessedWord’ compared with word ‘unscrambledWord’
		// Returns true if ‘guessedWord’ is identical to ‘unscrambledWord’ or false otherwise
		// The function returns the name of the person hosting the game 
		// Exception: user is not playing the game 
		[OperationContract]
		bool GuessWord(string playerName, string guessedWord, string unscrambledWord);
	}

	[DataContract]
	public class Word
	{
		[DataMember]
		public string UnscrambledWord; // word typed by the game’s host
		[DataMember]
		public string ScrambledWord;
	}
}