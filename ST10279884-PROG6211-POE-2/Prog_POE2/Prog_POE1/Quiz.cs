using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Figgle;

namespace Prog_POE1
{
    internal class Quiz
    {
        private int score = 0; // To keep track of the user's score
        private string userName; // Storing the user's name

        public Quiz(string name)
        {
            userName = name; // Assigning the user's name to the class variable
        }

        public void Start()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(FiggleFonts.Standard.Render(" Welcome to the CyberQuiz!"));
            Console.WriteLine("        =======================================================");

            // Ask MCQs
            AskMCQ(
                "Which of the following is an example of strong password characteristics?",
                new string[] { "a) 12345678", "b) Password1", "c) MySecureP@ssw0rd!", "d) None of the above" },
                "c");
            Console.WriteLine();
            AskMCQ(
                "What is the primary function of a VPN?",
                new string[] { "a) Encrypt data", "b) Hide IP address", "c) Both a and b", "d) None of the above" },
                "c");
            Console.WriteLine();

            AskMCQ(
                "Which type of malware holds your files hostage?",
                new string[] { "a) Spyware", "b) Ransomware", "c) Worms", "d) Adware" },
                "b");

            Console.WriteLine();
            AskMCQ(
                "What does phishing refer to?",
                new string[] { "a) A type of fishing", "b) A method to steal personal information", "c) A computer virus", "d) None of the above" },
                "b");
            Console.WriteLine();

            AskMCQ(
                "Which of the following is a secure way to store passwords?",
                new string[] { "a) Writing them down", "b) Using a password manager", "c) Saving them in a text file", "d) Memorizing all of them" },
                "b");

            // Ask open-ended questions
            AskOpenEnded(
                "What does 2FA stand for?",
                new string[] { "two-factor authentication",  },
                "It stands for Two-Factor Authentication."
            );
            

            AskOpenEnded(
                "Should you share your password with anyone?",
                new string[] { "no", "never" },
                "You should never share your password."
            );

            AskOpenEnded(
                "What kind of links should you avoid clicking?",
                new string[] { "suspicious", "unknown", "phishing" },
                "Avoid suspicious or unknown links."

            );

            AskOpenEnded(
            "What is the best way to verify a website's security?",
            new string[] { "https", "lock icon", "secure" },
            "Check for 'https' in the URL and a lock icon in the address bar."
            );

            AskOpenEnded(
             "Why is it important to update your software regularly?",
              new string[] { "security", "vulnerabilities", "bugs"},
              "Regular updates fix security vulnerabilities and bugs."
            );

            // Display final score
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"\nChatbot: Quiz complete! You scored {score}/10.\n");
            Console.ResetColor();
        }

        // Method for multiple-choice questions
        private void AskMCQ(string question, string[] options, string correctAnswer)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            TypewriterEffect($"Chatbot: {question}"); //Typewitter slow effect
            foreach (string option in options)
            {
                Console.WriteLine($"  {option}");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{userName} (Choose a, b, c, or d): ");
            string answer = Console.ReadLine()?.Trim().ToLower();

            if (answer == correctAnswer)
            {
                score++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Chatbot: Correct!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Chatbot: Incorrect.");
            }
        }

        // Method for open-ended questions
        private void AskOpenEnded(string question, string[] correctAnswers, string correctResponse)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            TypewriterEffect($"Chatbot: {question}"); //Typewitter slow effect
           
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{userName}: ");
            string answer = Console.ReadLine()?.Trim().ToLower();

            if (correctAnswers.Any(ans => answer.Contains(ans)))
            {
                score++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Chatbot: Correct!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chatbot: Incorrect. {correctResponse}");
            }
        }
        // Helper method for typewriter effect
        private void TypewriterEffect(string text, int delay = 25)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay); // Delay between each character
            }
            Console.WriteLine(); // Move to the next line after the text
        }
    }
}
