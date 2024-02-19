using System.Text.Json.Serialization;

namespace Cnblogs.SemanticKernel.Exntentions.OpenAI.ChatCompletions;

public class ChatCompletionRequest
{
    [JsonPropertyName("messages")]
    public IReadOnlyList<RequestMessage>? Messages { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 1;

    [JsonPropertyName("top_p")]
    public double TopP { get; set; } = 1;

    [JsonPropertyName("n")]
    public int? N { get; set; } = 1;

    [JsonPropertyName("presence_penalty")]
    public double PresencePenalty { get; set; } = 0;

    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; set; } = 0;

    [JsonPropertyName("model")]
    public required string Model { get; set; }

    [JsonPropertyName("tools")]
    public IReadOnlyList<Tool>? Tools { get; set; }

    [JsonPropertyName("tool_choice")]
    public string? ToolChoice { get; set; }
}

public class RequestMessage
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("tool_call_id")]
    public string? ToolCallId { get; set; }
}

public class Tool
{
    [JsonPropertyName("function")]
    public FunctionDefinition? Function { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class FunctionDefinition
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("parameters")]
    public ParameterDefinition Parameters { get; set; }

    public struct ParameterDefinition
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("required")]
        public string[]? Required { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, PropertyDefinition>? Properties { get; set; }

        public struct PropertyDefinition
        {
            [JsonPropertyName("type")]
            public required PropertyType Type { get; set; }
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PropertyType
        {
            Number,
            String,
            Boolean
        }
    }
}