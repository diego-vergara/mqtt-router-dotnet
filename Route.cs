namespace MqttRouter;

using System.Text.RegularExpressions;

public enum ParameterType
{
    String = 0,
    Int,
}
public class Parameter
{
    public string Name { get; set; } = string.Empty;
    public string Raw { get; set; } = string.Empty;
    public int Start { get; set; }
    public int End { get; set; }
    public ParameterType Type { get; set; } = ParameterType.String;


    public override string ToString()
    {
        return $"{Raw} / {Name} / {Start} - {End}";
    }
}

public class Route
{
    public static string Template = @"^T$";
    public string Pattern { get; set; }
    public List<Parameter> Parameters { get; set; }
    public Regex Regex { get; private set; }

    public Action<string, Dictionary<string, dynamic>> Handler { get; set; }

    public Route(string pattern)
    {
        Parameters = SearchParameters(pattern);
        Pattern = GeneratePattern(pattern);
        Regex = new Regex(Pattern, RegexOptions.Compiled);

        Print.WriteLine($"📢 New Route Instance", ConsoleColor.DarkBlue);
        Print.WriteLine($"📢 Pattern: {pattern}", ConsoleColor.DarkBlue);
        Print.WriteLine($"📢 P.Regex: {Pattern}", ConsoleColor.DarkBlue);
        Print.WriteLine($"📢 Parameters founded: {Parameters.Count}", ConsoleColor.DarkBlue);
    }

    public Route(string pattern, Action<string, Dictionary<string, dynamic>> handler)
        : this(pattern)
    {
        Handler = handler;
    }

    public bool Match(string incommingPattern)
    {
        var match = Regex.Match(incommingPattern);
        var isMatch = match.Success;

        Print.WriteLine($"📢 (Match) {incommingPattern}", isMatch ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);

        if (isMatch)
        {
            var parameters = new Dictionary<string, dynamic>(Parameters.Count);
            foreach (var parameter in Parameters)
            {
                Group? group;
                if (match.Groups.TryGetValue(parameter.Name, out group))
                {
                    parameters.Add(group.Name, parameter.Type switch
                    {
                        ParameterType.Int => Convert.ToInt32(group.Value),
                        _ => group.Value
                    });
                }

            }
            Handler("payload", parameters);
        }
        return isMatch;
    }
    protected string GeneratePattern(string pattern)
    {
        var builded = pattern;
        foreach (var parameter in Parameters)
        {
            builded = builded.Replace($"{{{parameter.Raw}}}", GenerateTypePattern(parameter));
        }
        return Template.Replace("T", builded);
    }

    protected string GenerateTypePattern(Parameter parameter)
    {
        return parameter.Type switch
        {
            ParameterType.Int => $"(?<{parameter.Name}>\\d+)",
            _ => $"(?<{parameter.Name}>\\w+)"
        };
    }

    protected List<Parameter> SearchParameters(string pattern)
    {
        var parameters = new List<Parameter>();
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == '{')
            {
                var start = i;
                var end = pattern.IndexOf('}', start);
                if (end == -1)
                {
                    throw new ArgumentException($"Pattern bad format not found close parameter delimiter");
                }

                var parameterRaw = pattern.Substring(start + 1, (end - start) - 1);
                var parameter = new Parameter
                {
                    Name = parameterRaw,
                    Raw = parameterRaw,
                    Start = start,
                    End = end,
                    Type = ParameterType.String
                };

                var posType = pattern.IndexOf(':', start);
                if (posType != -1)
                {
                    var paramName = pattern.Substring(start + 1, (posType - start) - 1);
                    var paramType = pattern.Substring(posType + 1, (end - posType) - 1);

                    parameter.Name = paramName;
                    parameter.Type = paramType switch
                    {
                        "string" => ParameterType.String,
                        "int" => ParameterType.Int,
                        _ => ParameterType.String
                    };
                }

                parameters.Add(parameter);
                i = end;
            }
        }
        return parameters;
    }
}