﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Arielle.Modules
{
    public class AskQuestion : ModuleBase<SocketCommandContext>
    {
        Question randomQuestion;
        [Command("Question")]
        public async Task Question()
        {
            randomQuestion = GetRandomQuestion();
            await Context.Channel.SendMessageAsync($"Question: {randomQuestion.Text}");
            HandleAnswers();
        }

        private Question GetRandomQuestion()
        {
            Random random = new Random();
            int randomPosition = random.Next(0, Program.Questions.Count);
            return Program.Questions[randomPosition];

        }

        private void HandleAnswers()
        {
            Context.Client.MessageReceived += AnswerRecieved;
        }

        private Task AnswerRecieved(SocketMessage imsg)
        {
            var _ = Task.Run(async () =>
            {
                try
                {

                    if (imsg.Author.IsBot)
                        return;
                    var msg = imsg as SocketUserMessage;
                    
                    if (msg.Content != randomQuestion.Answer)
                        return;
                    await Context.Channel.SendMessageAsync($"Correct answer \"{randomQuestion.Answer}\" by \"{msg.Author.Username}\"");
                    for (int i=0; i<Program.Users.Count; i++)
                    {
                        if (Program.Users[i].ID == msg.Author.Id)
                        {
                            await Context.Channel.SendMessageAsync($"Current points: {Program.Users[i].Points}+1");
                            Program.Users[i].Points++;
                        }
                    }
                }
                catch
                {
                }
            });
            return Task.CompletedTask;
        }
    }
}