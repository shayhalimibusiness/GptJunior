namespace GptJunior;

public static class Tester
{
    public static string GetRandomNumbersString(int x)
    {
        List<int> randomNumbers = new List<int>();
        Random random = new Random();

        for (int i = 1; i <= x; i++)
        {
            int randomNumber = random.Next(0, 101);
            randomNumbers.Add(randomNumber);
            if (i == 4)
            {
                Console.WriteLine("The forth element : "+ randomNumber);
            }
        }

        string output = "";
        for (int i = 0; i < randomNumbers.Count; i++)
        {
            output += $"a{i + 1}: {randomNumbers[i]}, ";
        }

        return output.TrimEnd(',', ' ');
    }

    public static async Task<string> GetTokenLimit(int x)
    {
        var request = "Here is a list:\n" + GetRandomNumbersString(x)
        + "\n what is the forth element?";
        var gpt = new BaseReqGptProxy("");
        var response = await gpt.GetResponse(request);
        return response;
    }
}