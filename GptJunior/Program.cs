// See https://aka.ms/new-console-template for more information

using GptJunior;

var projectManager = ProjectManagersFactory.CreateProjectManager();
var gptDeveloper = GptDevelopersFactory.CreateGptDeveloper();

var gptJunior = new GptJunior.GptJunior(projectManager, gptDeveloper);
await gptJunior.Create("a function that take a number and return the closest odd number that is higher than it");

