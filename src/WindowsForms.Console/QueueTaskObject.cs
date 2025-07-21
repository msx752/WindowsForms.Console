namespace WindowsForms.Console;

internal readonly struct QueueTaskObject
{
    public QueueTaskObject(string message, Color? color, bool showTimeTag)
    {
        Message = message;
        Color = color;
        ShowTimeTag = showTimeTag;
    }

    public Color? Color { get; }
    public string Message { get; }
    public bool ShowTimeTag { get; }
}
