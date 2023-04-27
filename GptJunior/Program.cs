// See https://aka.ms/new-console-template for more information

using GptJunior;

// var gitManager = GitManagersFactory.CreateGitManager();
// gitManager.CreateBranch("testingIt");
// gitManager.CommitChanges("Initial Commit.");

// var projectManager = ProjectManagersFactory.CreateProjectManager();
// var gptDeveloper = GptDevelopersFactory.CreateGptDeveloper();
//
// var gptJunior = new GptJunior.GptJunior(projectManager, gptDeveloper);

var gptJunior = GptJuniorsFactory.CreateGptJunior();
await gptJunior.Create("a function that take 3 numbers and return the 2 who are closest");

