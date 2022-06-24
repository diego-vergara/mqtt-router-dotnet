namespace MqttRouter;

public static class Print 
{
    public static void WriteLine(string text, ConsoleColor color = ConsoleColor.Black)
    {
        Console.BackgroundColor = color;
        Console.WriteLine(text);
        Console.BackgroundColor = ConsoleColor.Black;
    }

    public static void Write(string text, ConsoleColor color = ConsoleColor.Black)
    {
        Console.BackgroundColor = color;
        Console.Write(text);
        Console.BackgroundColor = ConsoleColor.Black;
    }

    public static void Ln() 
    {
        Console.WriteLine();
    }
}