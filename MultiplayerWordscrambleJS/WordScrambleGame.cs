using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MultiplayerWordscrambleJS
{
    [ServiceBehavior]
    public class WordScrambleGame : IWordScrambleGame
    {
        // the maximum number of players allowed playing simultaneously
        private const int MAX_PLAYERS = 5;
        // the user hosting the game. If it’s null nobody is hosting the game.
        private static string userHostingTheGame = null;
        // the Word object that contains the scrambled and unscrambled words
        private static Word gameWords;
        // the list of players playing the game
        private static List<string> activePlayers = new List<string>();

        [OperationBehavior]
        public bool IsGameBeingHosted()
        {
            return userHostingTheGame != null;
        }

        [OperationBehavior]
        public string HostGame(string playerName, string hostAddress, string wordToScramble)
        {
            if (IsGameBeingHosted())
            {
                throw new Exception();
            }

            return ScrambleWord(wordToScramble);
        }

        [OperationBehavior]
        public Word Join(string playerName)
        {
            // TO BE COMPLETED BY YOU: Add exception and program logic
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public bool GuessWord(string playerName, string guessedWord, string unscrambledWord)
        {
            // TO BE COMPLETED BY YOU: Add exception and program logic
            throw new NotImplementedException();
        }

        // Utility function to scramble a word
        private string ScrambleWord(string word)
        {
            char[] chars = word.ToArray();
            Random r = new Random(2011);
            for (int i = 0; i < chars.Length; i++)
            {
                int randomIndex = r.Next(0, chars.Length);
                char temp = chars[randomIndex];
                chars[randomIndex] = chars[i];
                chars[i] = temp;
            }
            return new string(chars);
        }
    }

}
