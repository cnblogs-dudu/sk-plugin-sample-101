using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Plugins;

public class LightPlugin
{
    public bool IsOn { get; set; } = false;

    [KernelFunction]
    [Description("帮看一下灯是开是关")]
    public string GetState() => IsOn ? "on" : "off";

    [KernelFunction]
    [Description("开灯或者关灯")]
    public string ChangeState(bool newState)
    {
        IsOn = newState;
        var state = GetState();
        Console.WriteLine(state == "on" ? $"[开灯啦]" : "[关灯咯]");
        return state;
    }
}
