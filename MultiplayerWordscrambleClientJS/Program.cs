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

            if (!proxy.IsGameBeingHosted())
            {
                Console.WriteLine("Welcome " + playerName +
                           "! Do you want to host the game?");
                if (Console.ReadLine().CompareTo("yes") == 0)
                {
                    Console.WriteLine("Type the word to scramble.");
                    string inputWord = Console.ReadLine();
                    string scrambledWord = proxy.HostGame(playerName, "", inputWord);
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
                    Word gameWords = proxy.Join(playerName);
                    Console.WriteLine("Can you unscramble this word? => " + gameWords.ScrambledWord);
                    bool gameOver = false;
                    while (!gameOver)
                    {
                        gameWords.UnscrambledWord = Console.ReadLine();
                        gameOver = proxy.GuessWord(playerName, gameWords);
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
