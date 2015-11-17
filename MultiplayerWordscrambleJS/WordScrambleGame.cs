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
        private static string _userHostingTheGame = null;
        // the Word object that contains the scrambled and unscrambled words
        private static Word _gameWords;
        // the list of players playing the game
        private static List<string> _activePlayers = new List<string>();

        [OperationBehavior]
        public bool IsGameBeingHosted()
        {
            return _userHostingTheGame != null;
        }

        [OperationBehavior]
        public string HostGame(string playerName, string wordToScramble)
        {
            if (IsGameBeingHosted())
            {
                throw new FaultException<GameBeingHostedFault>(
                    new GameBeingHostedFault { HostPlayer = _userHostingTheGame });
            }

            _userHostingTheGame = playerName;
            _gameWords = new Word { UnscrambledWord = wordToScramble.Trim(), ScrambledWord = ScrambleWord(wordToScramble) };
            return _gameWords.ScrambledWord;
        }

        [OperationBehavior]
        public Word Join(string playerName)
        {
            if (_activePlayers.Count >= MAX_PLAYERS)
            {
                throw new FaultException<MaximumNumberOfPlayersReachedFault>(
                    new MaximumNumberOfPlayersReachedFault { Capacity = MAX_PLAYERS });
            }

            if (_activePlayers.Contains(playerName))
            {
                throw new FaultException<PlayerAlreadyInGameFault>(
                    new PlayerAlreadyInGameFault());
            }

            if (_userHostingTheGame == playerName)
            {
                throw new FaultException<HostCannotJoinTheGameFault>(
                    new HostCannotJoinTheGameFault());
            }

            if (!IsGameBeingHosted())
            {
                throw new FaultException<GameIsNotBeingHostedFault>(new GameIsNotBeingHostedFault());
            }

            _activePlayers.Add(playerName);

            return new Word { ScrambledWord = _gameWords.ScrambledWord };
        }

        [OperationBehavior]
        public bool GuessWord(string playerName, Word gameWords)
        {
            bool victorious = false;

            if (!IsGameBeingHosted())
            {
                throw new FaultException<GameIsNotBeingHostedFault>(new GameIsNotBeingHostedFault());
            }

            if (gameWords.ScrambledWord != _gameWords.ScrambledWord)
            {
                throw new FaultException<WordMismatchFault>(new WordMismatchFault { ClientScrambled = gameWords.ScrambledWord, ServerScrambled = _gameWords.ScrambledWord });
            }

            if (!_activePlayers.Contains(playerName))
            {
                throw new FaultException<PlayerNotPlayingTheGameFault>(new PlayerNotPlayingTheGameFault());
            }

            if (gameWords.UnscrambledWord == _gameWords.UnscrambledWord)
            {
                victorious = true;
                _activePlayers.Remove(playerName); // This player got it so they are removed from the game.
            }

            return victorious;
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
