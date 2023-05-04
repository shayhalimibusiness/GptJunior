// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using GptJunior;
using GptJunior.IDE;
using GptJunior.Modules;

var gitMgr = GitManagersFactory.CreateGitManager();
gitMgr.CreateBranch("Test_Branch");

var ide = IdesFactory.CreateIde();
ide.Write("MyTest", ";");

gitMgr.CommitChanges("Initial Commit.");


Console.WriteLine("Done and Done");

#region History

// var gptJunior = GptJuniorsFactory.CreateGptJunior();
// await gptJunior.Create("an interface that generates a graph with 3 nodes and 2 edges.");

// string jsonStrResponse =
//     "{\n  \"Files\": [\n    {\n      \"FileName\": \"Graph.cs\",\n      \"DesignPattern\": 0,\n      \"FileContent\": \"public class GraphImpl : Graph { private List\\u003cNode\\u003e nodes \\u003d new List\\u003cNode\\u003e(); private List\\u003cTuple\\u003cNode, Node\\u003e\\u003e edges \\u003d new List\\u003cTuple\\u003cNode, Node\\u003e\\u003e(); public void AddNode(Node node) { nodes.Add(node); } public void AddEdge(Node node1, Node node2) { edges.Add(new Tuple\\u003cNode, Node\\u003e(node1, node2)); } public List\\u003cNode\\u003e GetNodes() { return nodes; } public List\\u003cTuple\\u003cNode, Node\\u003e\\u003e GetEdges() { return edges; } }\"\n    },\n    {\n      \"FileName\": \"Node.cs\",\n      \"DesignPattern\": 1,\n      \"FileContent\": \"public class Node { public int Id { get; set; } public Node(int id) { Id \\u003d id; } }\"\n    },\n    {\n      \"FileName\": \"GraphFactory.cs\",\n      \"DesignPattern\": 2,\n      \"FileContent\": \"public static class GraphFactory { public static Graph CreateGraph() { return new GraphImpl(); } }\"\n    },\n    {\n      \"FileName\": \"GraphTests.cs\",\n      \"DesignPattern\": 3,\n      \"FileContent\": \"public class GraphTests { [Test] public void TestCreateGraph() { Graph graph \\u003d GraphFactory.CreateGraph(); Assert.IsNotNull(graph); } [Test] public void TestAddNode() { Graph graph \\u003d GraphFactory.CreateGraph(); Node node1 \\u003d new Node(1); graph.AddNode(node1); Assert.AreEqual(1, graph.GetNodes().Count); } [Test] public void TestAddEdge() { Graph graph \\u003d GraphFactory.CreateGraph(); Node node1 \\u003d new Node(1); Node node2 \\u003d new Node(2); graph.AddNode(node1); graph.AddNode(node2); graph.AddEdge(node1, node2); Assert.AreEqual(1, graph.GetEdges().Count); } }\"\n    }\n  ]\n}";
// FixAnswer? dtoResponse = JsonSerializer.Deserialize<FixAnswer>(jsonStrResponse);
// Console.WriteLine(dtoResponse.ToString());

// var gptTester = GptProxiesFactory.CreateInterfaceDeveloper();
// var response =
//     await gptTester.GetResponse("Interface that can takes number and bring back the sum of all the numbers it got so far.");
// DevAnswer? jsonAns = JsonSerializer.Deserialize<DevAnswer>(response);
// Console.WriteLine(response);

// var res = await Tester.GetTokenLimit(800);
// Console.WriteLine(res);
// var str = Tester.GetRandomNumbersString(800);
// Console.WriteLine(str);

// var gitManager = GitManagersFactory.CreateGitManager();
// gitManager.CreateBranch("testingIt");
// gitManager.CommitChanges("Initial Commit.");

// var projectManager = ProjectManagersFactory.CreateProjectManager();
// var gptDeveloper = GptDevelopersFactory.CreateGptDeveloper();
//
// var gptJunior = new GptJunior.GptJunior(projectManager, gptDeveloper);

#endregion

