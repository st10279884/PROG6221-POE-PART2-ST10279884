using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prog_POE1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Chatbot chatbot = new Chatbot();
            string filepath = "C:\\Users\\carly\\source\\repos\\Prog_POE1\\(Audio) AI-Welcome-Message.wav"; 
            chatbot.WelcomeMessage(filepath); 
            chatbot.UserQuestions();
           

            Quiz quiz = new Quiz("User");
            quiz.Start();

        }
    }
}
