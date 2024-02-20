using Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Plugins;
using System.Text.Json;

var builder = Kernel.CreateBuilder();
builder.Services.AddOpenAIChatCompletion("qwen-max", "sk-xxxxxx");
builder.Services.ConfigureHttpClientDefaults(b =>
    b.ConfigurePrimaryHttpMessageHandler(() => new BypassHandler()));
builder.Plugins.AddFromType<LightPlugin>();
Kernel kernel = builder.Build();

var history = new ChatHistory();
history.AddUserMessage("请开灯");
Console.WriteLine("User > " + history[0].Content);
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Enable auto function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var result = await chatCompletionService.GetChatMessageContentAsync(
    history,
    executionSettings: openAIPromptExecutionSettings,
    kernel: kernel);

Console.WriteLine("Assistant > " + result);
