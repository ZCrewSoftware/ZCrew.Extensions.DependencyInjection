using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace RegistrationSample.Printing;

/// <summary>
///     Registers services against a fresh <see cref="IServiceCollection"/> and prints the registration code next to the
///     descriptors it produced. Output is plain ASCII so it renders anywhere.
/// </summary>
internal static class RegistrationPrinter
{
    /// <summary>
    ///     Prints the scenario heading, its registration snippet, and a table of the resulting service descriptors.
    /// </summary>
    public static void Print(
        string title,
        Action<IServiceCollection> register,
        [CallerArgumentExpression(nameof(register))] string code = ""
    )
    {
        var services = new ServiceCollection();
        register.Invoke(services);

        WriteHeading(title);
        WriteCode(code);
        WriteTable(services);
        Console.WriteLine();
    }

    private static void WriteHeading(string title)
    {
        Console.WriteLine(title);
        Console.WriteLine(new string('=', title.Length));
    }

    private static void WriteCode(string code)
    {
        var codeBody = code.Substring(code.IndexOf('{') + 1, code.LastIndexOf('}') - code.IndexOf('{') - 1);
        Console.WriteLine(codeBody);
    }

    private static void WriteTable(IServiceCollection services)
    {
        string[] headers = ["Lifetime", "Key", "Service", "Implementation"];
        var rows = services.Select(ToRow).ToList();

        var widths = new int[headers.Length];
        for (var column = 0; column < headers.Length; column++)
        {
            widths[column] = headers[column].Length;
            foreach (var row in rows)
            {
                widths[column] = Math.Max(widths[column], row[column].Length);
            }
        }

        WriteRow(headers, widths, pad: true);
        WriteRow([.. widths.Select(width => new string('-', width + 2))], widths, pad: false); // + 2 for padding
        foreach (var row in rows)
        {
            WriteRow(row, widths, pad: true);
        }
    }

    private static void WriteRow(string[] cells, int[] widths, bool pad)
    {
        var padded = cells.Select((cell, column) => cell.PadRight(widths[column]));
        if (pad)
        {
            padded = padded.Select(cell => $" {cell} ");
        }
        Console.WriteLine(string.Join("|", padded).TrimEnd());
    }

    private static string[] ToRow(ServiceDescriptor descriptor)
    {
        return
        [
            descriptor.Lifetime.ToString(),
            descriptor.IsKeyedService ? $"{descriptor.ServiceKey}" : "",
            FriendlyName(descriptor.ServiceType),
            DescribeImplementation(descriptor),
        ];
    }

    private static string DescribeImplementation(ServiceDescriptor descriptor)
    {
        if (descriptor.IsKeyedService)
        {
            if (descriptor.KeyedImplementationType is { } keyedType)
            {
                return FriendlyName(keyedType);
            }

            if (descriptor.KeyedImplementationInstance is { } keyedInstance)
            {
                return $"{FriendlyName(keyedInstance.GetType())} (instance)";
            }

            return "(factory)";
        }

        if (descriptor.ImplementationType is { } type)
        {
            return FriendlyName(type);
        }

        if (descriptor.ImplementationInstance is { } instance)
        {
            return $"{FriendlyName(instance.GetType())} (instance)";
        }

        return "(factory)";
    }

    private static string FriendlyName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var name = type.Name;
        var backtick = name.IndexOf('`');
        if (backtick >= 0)
        {
            name = name[..backtick];
        }

        var arguments = string.Join(", ", type.GetGenericArguments().Select(FriendlyName));
        return $"{name}<{arguments}>";
    }
}
