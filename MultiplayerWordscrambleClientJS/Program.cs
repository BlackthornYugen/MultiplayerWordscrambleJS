using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiplayerWordscrambleClientJS.WordScrambleMPClient;

namespace MultiplayerWordscrambleClientJS
{
    class Program
    {
        static void Main(string[] args)
        {
            WordScrambleGameClient proxy = new WordScrambleGameClient();

            bool canPlayGame = true;

            Console.WriteLine("Player's name?");
            String playerName = Console.ReadLine();

            if (!proxy.isGameBeingHosted())
            {
                Console.WriteLine("Welcome " + playerName +
                           "! Do you want to host the game?");
                if (Console.ReadLine().CompareTo("yes") == 0)
                {
                    Console.WriteLine("Type the word to scramble.");
                    string inputWord = Console.ReadLine();
                    string scrambledWord = proxy.hostGame(playerName, "", inputWord);
                    canPlayGame = false;
                    Console.WriteLine("You're hosting the game with word '" + inputWord + "' scrambled as '" + scrambledWord + "'");
                    Console.ReadKey();
                }
            }
            if (canPlayGame)
            {
                Console.WriteLine("Do you want to play the game?");
                if (Console.ReadLine().CompareTo("yes") == 0)
                {
                    Word gameWords = proxy.join(playerName);
                    Console.WriteLine("Can you unscramble this word? => " + gameWords.scrambledWord);
                    String guessedWord;
                    bool gameOver = false;
                    while (!gameOver)
                    {
                        guessedWord = Console.ReadLine();
                        gameOver = proxy.guessWord(playerName, guessedWord, gameWords.unscrambledWord);
                        if (!gameOver)
                        {
                            Console.WriteLine("Nope, try again...");
                        }
                    }
                    Console.WriteLine("You WON!!!");
                }
            }
        }
    }
}
