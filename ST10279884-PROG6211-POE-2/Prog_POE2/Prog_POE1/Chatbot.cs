using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices.Expando;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Figgle; 

namespace Prog_POE1
{
    internal class Chatbot
    {
        private string userName;

        //Adding the context tracking fields
        private string lastTopic = null;
        private readonly int lastTipIndex = -1;

        private readonly Random random = new Random();

        //Adding the context tracking field for favorite topic 0
        private string favoriteTopic = null;
        private bool askedFavouriteTopic = false;

        private readonly Dictionary<string, string[]> topicTips = new Dictionary<string, string[]>
{
    { "phishing", new[]
        {
            "Always check the sender's email address before clicking any links.",
            "Never provide personal information in responses to unsolicited emails.",
            "Hover over links to see their actual destination before clicking.",
            "Be cautious of emails that create a sense of urgency or fear.",
            "Use spam filters and keep your software updated to reduce phishing risks."
        }
    },
    { "passwords", new[]
        {
            "Use a unique password for every account.",
            "Include uppercase, lowercase, numbers and symbols in your passwords.",
            "Consider using a password manager to generate and store strong passwords."
        }
    },
    { "cybersecurity", new[]
        {
            "Keep your operating system and software up to date to protect against vulnerabilities.",
            "Use strong, unique passwords for each of your accounts.",
            "Enable two-factor authentication wherever possible.",
            "Be cautious when clicking on links or downloading attachments from unknown sources.",
            "Regularly back up your important data to a secure location."
        }
    },
    { "firewalls", new[]
        {
            "Enable your firewall to block unauthorized access to your network.",
            "Regularly update your firewall rules to adapt to new threats.",
            "Use both hardware and software firewalls for layered protection.",
            "Monitor firewall logs to detect suspicious activity."
        }
    },
    { "ransomware", new[]
        {
            "Regularly back up your important files to an external drive or cloud storage.",
            "Do not open email attachments or click links from unknown sources.",
            "Keep your operating system and antivirus software up to date.",
            "Disable macros in documents received via email unless you trust the source."
        }
    },
    { "vpn", new[]
        {
            "Use a VPN when connecting to public Wi-Fi to protect your data from hackers.",
            "Choose a reputable VPN provider that does not log your activity.",
            "Enable the VPN's kill switch feature to prevent data leaks if the connection drops.",
            "Regularly update your VPN software to patch security vulnerabilities.",
            "Remember that while a VPN increases privacy, it does not make you completely anonymous online."
        }
    }
};

        private readonly Dictionary<string, string> topicExplanations = new Dictionary<string, string>
        {
            { "phishing", "Phishing is a type of cyberattack where attackers trick you into providing sensitive information, like passwords or credit card details, by pretending to be a trustworthy entity." },
            { "ransomware", "Ransomware is a type of malware that encrypts your files and demands a ransom to restore them." },
            { "firewall", "A firewall is a security system that monitors and controls incoming and outgoing network traffic based on predetermined security rules." },
            { "vpn", "A VPN (Virtual Private Network) encrypts your internet connection, providing privacy and security while browsing online." },
            { "password manager", "A password manager helps you securely store and manage your passwords, making it easier to use strong, unique passwords for each account." }
        };
        
        public void WelcomeMessage(string filepath)
        {
            try
            {
                SoundPlayer soundPlayer = new SoundPlayer(filepath);
                soundPlayer.Load();
                soundPlayer.Play();
            }
            catch (Exception ex) //exception incase the audio will not play
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Unable to play the audio file. Details: {ex.Message}");
                Console.ResetColor();
            }
            //Displaying the Ascii art logo/banner
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(FiggleFonts.Standard.Render("Welcome to Cybersecurity ChatBot!"));

            string banner = @"
                                                             ==========================================
                                                             ||     CYBERSECURITY AWARENESS BOT      ||
                                                             ==========================================
                                                             ||   ________                           ||
                                                             ||  |        |                          ||
                                                             ||  |  LOCK  |      [ENCRYPTED]         ||
                                                             ||  |________|      [SECURE ACCESS]     ||
                                                             ||     ||||                             ||
                                                             ||  ___||||___   [FIREWALL ENABLED]     ||
                                                             || |   SAFE   |                         ||
                                                             || |  ZONE    |   [THREATS MONITORED]   ||
                                                             || |__________|                         ||
                                                             ||                                      ||
                                                             ||     STATUS: SYSTEM LOCKED            ||
                                                             ========================================== ";

            Console.WriteLine(banner);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(FiggleFonts.Standard.Render("Questions Time!"));
            Console.WriteLine("     ====================================================");

            // Prompting the user for username with validation
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter your name (at least 3 characters, no special characters):");
                string input = Console.ReadLine()?.Trim();

                if (IsValidUsername(input))
                {
                    userName = input; // Storing the valid username
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid username. Please try again.");
                    Console.ResetColor();
                }
            }

            Console.WriteLine(FiggleFonts.Mini.Render($"Hello, {userName}! Welcome to the Cybersecurity Awareness Bot."));


            //Addition of the favorite topic question
            while (string.IsNullOrWhiteSpace(favoriteTopic))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Chatbot: What's your favorite cybersecurity topic (e.g., phishing, passwords, VPN, ransomware, firewall)?");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{userName}: ");
                string topicInput = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(topicInput))
                {
                    favoriteTopic = topicInput;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Chatbot: Great! I'll remember that you are interested in {favoriteTopic}.");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }
         }

        // method to help validate username
        private bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false; // Ensure it's not empty or whitespace
            if (username.Length < 3) return false; // Ensure that the it's at least 3 characters long
            if (username.Any(ch => !char.IsLetterOrDigit(ch) && ch != ' ')) return false; // Allow only letters, digits, and spaces
            return true;
        }

        public void UserQuestions()
        {
            string questionList =
                "Chatbot: What do you want to ask? Here are some options:\n" +
                "1. How are you?\n" +
                "2. What's your purpose?\n" +
                "3. What can I ask you about?\n" +
                "4. How do I stay safe online?\n" +
                "5. Can you give me a cybersecurity tips?\n" +
                "6. What is phishing?/phishing tips?\n" +
                "7. What is ransomware?\n" +
                "8. What is a firewall?\n" +
                "9. What is a VPN?\n" +
                "10. What is a password manager?/password tips\n" +
                "(Type 'exit' to stop asking questions)";

        // this display the question list only once
        Typewriter(questionList, ConsoleColor.Yellow);
            Console.WriteLine();

            while (true)
            {
                // Display the chatbot's message
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Chatbot: Enter a question?");
                Console.WriteLine();


                // Display the user's name as a separate prompt
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{userName}: ");
                string userQuestion = Console.ReadLine()?.Trim();
                
                
                //Makes sure that the question ends with a question mark
                if (!userQuestion.EndsWith("?") && !userQuestion.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chatbot: OOPS! That doesn't look like a question. Please ask a proper question."); //will show this if question mark is not applied
                    continue;
                }

                if (userQuestion.Equals("exit", StringComparison.OrdinalIgnoreCase)) // if the user types exit 
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Chatbot: Would you like to take a quiz? (yes/no)"); //asks the user if they want to take a quiz
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write($"{userName}: ");
                    string quizOption = Console.ReadLine()?.Trim().ToLower();

                    if (quizOption == "yes")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Chatbot: Starting the quiz...");
                        Console.ResetColor();

                        // Goes to the Quiz class
                        Quiz quiz = new Quiz(userName);
                        quiz.Start();

                        // Close the application after the quiz is completed
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Chatbot: Thank you for participating in the quiz! The application will now close.");
                        Console.ResetColor();
                        Environment.Exit(0);
                    }
                    else if (quizOption == "no")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Chatbot: Goodbye! Stay safe and secure!");
                        Console.ResetColor();
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Chatbot: Invalid option. Exiting the application."); //if the user types something else eg. Maybe
                        Console.ResetColor();

                        // Gracefully exit the application
                        Environment.Exit(0);
                    }

                    break; // Exiting the loop
                }


                // Get the chatbot response and display it
                string response = GetChatbotResponse(userQuestion);
                Typewriter(response, ConsoleColor.Yellow);
            }
        }

        private string GetRandomTip(string topic)
        {
            if (topicTips.ContainsKey(topic))
            {
                var tips = topicTips[topic];
                return tips[random.Next(tips.Length)];
            }
            return null;
        }
        string GetChatbotResponse(string userQuestion)
        {
            userQuestion = userQuestion.ToLower();

            // Sentiment detection (keep as is, or move to a helper for clarity)
            if (userQuestion.Contains("worried") || userQuestion.Contains("scared") || userQuestion.Contains("anxious") || userQuestion.Contains("overwhelmed"))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                    return $"Chatbot: I understand that {lastTopic} can be concerning, {userName}. Remember, staying informed and following best practices can help you stay safe. Would you like some tips or more information on {lastTopic}?";
                else
                    return $"Chatbot: It's normal to feel worried about cybersecurity, {userName}. I'm here to help you with any questions or concerns you have!";
            }
          

            // Always provide a tip about the favorite topic if the user asks for a tip about it
            if (!string.IsNullOrWhiteSpace(favoriteTopic) &&
                (userQuestion.Contains("tip") || userQuestion.Contains("tips")) &&
                userQuestion.Contains(favoriteTopic.ToLower()))
            {
                foreach (var topic in topicTips.Keys)
                {
                    if (favoriteTopic.ToLower().Contains(topic))
                        return $"Chatbot: {userName}, since you like {topic}, here's a tip: {GetRandomTip(topic)}";
                }
            }

            // Recall favorite topic
            if (userQuestion.Contains("my favorite topic") || userQuestion.Contains("my favourite topic"))
                return !string.IsNullOrWhiteSpace(favoriteTopic)
                    ? $"Chatbot: {userName}, you told me your favorite cybersecurity topic is {favoriteTopic}."
                    : $"Chatbot: I don't know your favorite topic yet. You can tell me!";

            // Handle confusion or request for more details
            if (userQuestion.Contains("more") || userQuestion.Contains("details") || userQuestion.Contains("explain") ||
                userQuestion.Contains("what do you mean") || userQuestion.Contains("not sure") || userQuestion.Contains("confused"))
            {
                if (!string.IsNullOrEmpty(lastTopic) && topicTips.ContainsKey(lastTopic))
                {
                    return $"Chatbot: Here's another tip about {lastTopic}: {GetRandomTip(lastTopic)}";
                }
                else if (lastTopic == "vpn")
                {
                    return "A VPN (Virtual Private Network) not only encrypts your internet connection but also hides your IP address, making your online activities more private and secure.";
                }
                else if (lastTopic == "password manager")
                {
                    return "Password managers can generate strong passwords for you and store them securely, so you don't have to remember each one.";
                }
                else
                {
                    return "Could you please specify which topic you'd like more details about?";
                }
            }

            // Main topic explanations
            foreach (var topic in topicExplanations.Keys)
            {
                if (userQuestion.Contains(topic))
                {
                    lastTopic = topic;
                    return $"Chatbot: {topicExplanations[topic]}{(topicTips.ContainsKey(topic) ? " Would you like some tips?" : "")}";
                }
            }

            // Random tip for topics
            foreach (var topic in topicTips.Keys)
            {
                if (userQuestion.Contains(topic + " tip") || userQuestion.Contains(topic + " tips"))
                {
                    lastTopic = topic;
                    return GetRandomTip(topic);
                }
            }

            // Generic responses
            if (userQuestion.Contains("how are you"))
                return $"Chatbot: I'm just lines of code, but I'm always ready to assist you!";
            if (userQuestion.Contains("purpose"))
                return $"Chatbot: My purpose is to provide cybersecurity tips and help you stay safe online!";
            if (userQuestion.Contains("ask you about"))
                return $"Chatbot: You can ask me about cybersecurity, online safety, and more!";
            if (userQuestion.Contains("stay safe online"))
                return $"Chatbot: Avoid clicking on suspicious links, use strong passwords, and enable two-factor authentication!";

            // Default fallback
            lastTopic = null;
            return "Chatbot: I'm not sure I understand. Can you try rephrasing?";
        }

        public static void Typewriter(string message, ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
                foreach (char c in message)
                {
                    Console.Write(c);
                    Thread.Sleep(25); // Simulates typing effect
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred while displaying the message: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}