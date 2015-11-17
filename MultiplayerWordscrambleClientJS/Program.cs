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

            String playerName = Question("Player's name?");

            if (!proxy.IsGameBeingHosted())
            {
                if (YesOrNo("Welcome " + playerName + "! Do you want to host the game?"))
                {
                    string inputWord = Question("Type the word to scramble.");
                    string scrambledWord;
                    try
                    {
                        scrambledWord = proxy.HostGame(playerName, inputWord);
                        canPlayGame = false;
                        Console.WriteLine("You're hosting the game with word '" + inputWord + "' scrambled as '" + scrambledWord + "'");
                        Console.ReadKey();
                    }
                    catch (FaultException<GameBeingHostedFault> fault)
                    {
                        InformUser(fault.Detail.Message + " ", wait: false);
                    } 
                }
            }
            if (canPlayGame)
            {
                if (YesOrNo("Do you want to play the game?"))
                {
                    Word gameWords = null;
                    do
                    {
                        try
                        {
                            gameWords = proxy.Join(playerName);

                        }
                        catch (FaultException<MaximumNumberOfPlayersReachedFault> fault)
                        {
                            InformUser(fault.Detail.Message, wait: false);
                        }
                        catch (FaultException<PlayerAlreadyInGameFault> fault)
                        {
                            InformUser(fault.Detail.Message, wait: false);
                        }
                        catch (FaultException<HostCannotJoinTheGameFault> fault)
                        {
                            InformUser(fault.Detail.Message, wait: false);
                        }
                        catch (FaultException<GameIsNotBeingHostedFault> fault)
                        {
                            InformUser(fault.Detail.Message, wait: false);
                        }
                    } while (gameWords == null && YesOrNo("You failed to join, try again?"));

                    if (gameWords != null) GameLoop(proxy, playerName, gameWords);
                }
            }
        }

        private static void GameLoop(WordScrambleGameClient proxy, String playerName, Word gameWords)
        {
            InformUser("Can you unscramble this word? => " + gameWords.ScrambledWord, wait: false);
            bool gameOver = false;
            while (!gameOver)
            {
                gameWords.UnscrambledWord = Console.ReadLine();

                try
                {
                    gameOver = proxy.GuessWord(playerName, gameWords);
                }
                catch (FaultException<WordMismatchFault> fault)
                {
                    InformUser(fault.Detail.Message); // Inform user but let them try to keep playing
                } 
                catch (FaultException<PlayerNotPlayingTheGameFault> fault)
                {
                    try
                    {
                        gameWords = proxy.Join(playerName); // Try joining and trying again
                        gameOver = proxy.GuessWord(playerName, gameWords);
                    }
                    catch (Exception exception)
                    {   // If anything else bad happends, we can return the first fault, the second exception and error out to console.
                        InformUser(fault.Message);
                        InformUser(exception.GetBaseException().Message);
                        throw fault;
                    }
                }
                
                if (!gameOver)
                {
                    InformUser("Nope, try again...", wait: false);
                }
            }
            InformUser("You WON!!!");
        }

        private static bool YesOrNo(string question = null)
        {
            if (question != null) Console.WriteLine(question);
            return Console.ReadLine().ToLower().Trim().CompareTo("yes") == 0;
        }

        private static string Question(string question = null)
        {
            Console.WriteLine(question);
            return Console.ReadLine();
        }

        private static void InformUser(string message, bool newline = true, bool wait = true)
        {
            if (newline)
                Console.WriteLine(message);
            else
                Console.Write(message);

            if (wait) 
                Console.ReadKey();
        }
    }
}
