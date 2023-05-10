using System.Runtime.Serialization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;


namespace GptJunior;

public static class Helpers
{
    public static async Task<string> FormatCSharpCodeAsync(string code)
    {
        var workspace = new AdhocWorkspace();

        var tree = CSharpSyntaxTree.ParseText(code);
        var root = await tree.GetRootAsync();
        var formattedRoot = Microsoft.CodeAnalysis.Formatting.Formatter.Format(root, workspace);

        return formattedRoot.ToFullString();
    }
    
    public static List<string> CreateFunctionWrapper(string name)
    {
        return new List<string>
        {
            $"public void {name}()",
            "{",
            "}"
        };
    }
}