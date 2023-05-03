using System.Text.Json;
using GptJunior.Modules;

namespace GptJunior;

public interface IAiAdaptor
{
    Task<dynamic?> Respond(dynamic request);
}

public class AiAdaptor<TIn, TOut> : IAiAdaptor
{
    private IGptProxy _gptProxy;
    
    public AiAdaptor(IGptProxy gptProxy)
    {
        _gptProxy = gptProxy;
    }

    public async Task<dynamic?> Respond(dynamic request)
    {
        string jsonStrRequest = JsonSerializer.Serialize(request);
        string jsonStrResponse = await _gptProxy.GetResponse(jsonStrRequest);
        TOut? dtoResponse = JsonSerializer.Deserialize<TOut>(jsonStrResponse);
        return dtoResponse;
    }
}

public static class AiAdaptorsFactory
{
    public static IAiAdaptor CreateInterfaceDeveloperAdaptor()
    {
        var gptProxy = GptProxiesFactory.CreateInterfaceDeveloper();

        var adaptor = new AiAdaptor<string, DevAnswer>(gptProxy);

        return adaptor;
    }
    
    public static IAiAdaptor CreateInterfaceFixerAdaptor()
    {
        var gptProxy = GptProxiesFactory.CreateInterfaceFixer();

        var adaptor = new AiAdaptor<FixRequest, FixAnswer>(gptProxy);

        return adaptor;
    }
}