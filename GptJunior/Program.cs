// See https://aka.ms/new-console-template for more information

using GptJunior;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

var projectManager = ProjectManagersFactory.CreateProjectManager();
var gptDeveloper = GptDevelopersFactory.CreateGptDeveloper();

var gptJunior = new GptJunior.GptJunior(projectManager, gptDeveloper);
await gptJunior.Create("a function that add four to a number");

// var wow = GptJuniorsFactory.CreateGptJunior();
// wow.Create("a function that add four to a number");


// var second = GptDevelopersFactory.CreateGptDeveloper();
// var d = await second.Develop("a function that add four to a number");
// Console.WriteLine(d.FunctionName[0]);