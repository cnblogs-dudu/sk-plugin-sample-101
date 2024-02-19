using Cnblogs.SemanticKernel.Exntentions.OpenAI.ChatCompletions;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Cnblogs.SemanticKernel.Exntentions.OpenAI.ChatCompletions.FunctionDefinition.ParameterDefinition;

namespace Handlers;

public class BypassHandler() : DelegatingHandler(new HttpClientHandler())
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var json = await request.Content!.ReadAsStringAsync();
        // Console.WriteLine(json);
        var chatCompletionRequest = JsonSerializer.Deserialize<ChatCompletionRequest>(json);
        var function = chatCompletionRequest?.Tools.FirstOrDefault(x => x.Function.Description.Contains("开灯"))?.Function;
        var functionName = function.Name;
        var parameterName = function.Parameters.Properties.FirstOrDefault(x => x.Value.Type == PropertyType.Boolean).Key;
        Dictionary<string, bool> arguments = new()
        {
            { parameterName, true }
        };

        // return await base.SendAsync(request, cancellationToken);

        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var chatCompletion = new ChatCompletionResponse
        {
            Id = Guid.NewGuid().ToString(),
            Model = "fake-mode",
            Object = "chat.completion",
            Created = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Choices =
               [
                   new()
                   {
                       Index = 0,
                       Message = new ResponseMessage
                       {
                           Role = "assistant"
                       },
                       FinishReason = "stop"
                   }
               ]
        };

        var messages = chatCompletionRequest.Messages;
        var toolCallId = "76f8dead- b5ad-4e6d-b343-7f78d68fac8e";
        var toolCallIdMessage = messages.FirstOrDefault(x => x.Role == "tool" && x.ToolCallId == toolCallId);

        if (toolCallIdMessage != null && toolCallIdMessage.Content == "on")
        {
            chatCompletion.Choices[0].Message.Content = "客官，灯已开";
        }
        else if (messages.First(x => x.Role == "user").Content.Contains("开灯") == true)
        {
            chatCompletion.Choices[0].Message.Content = "";
            chatCompletion.Choices[0].Message.ToolCalls = new List<ToolCall>()
            {
                new ToolCall
                {
                    Id = toolCallId,
                    Type = "function",
                    Function = new FunctionCall
                    {
                        Name = function.Name,
                        Arguments = JsonSerializer.Serialize(arguments, GetJsonSerializerOptions())
                    }
                }
            };
        }

        json = JsonSerializer.Serialize(chatCompletion, GetJsonSerializerOptions());
        response.Content = new StringContent(json, Encoding.UTF8, "application/json");
        //Console.WriteLine(await response.Content.ReadAsStringAsync());
        return response;
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
    }
}