using System.Text;

namespace GptJunior;

public static class Helpers
{
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