using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RapidLoadouts.YAMLStuff;

public class YAMLUtils
{
    internal static void WriteConfigFileFromResource(string configFilePath)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "RapidLoadouts.YAMLStuff.Example.yml";

        using Stream resourceStream = assembly.GetManifestResourceStream(resourceName)!;
        if (resourceStream == null)
        {
            throw new FileNotFoundException($"Resource '{resourceName}' not found in the assembly.");
        }

        using StreamReader reader = new StreamReader(resourceStream);
        string contents = reader.ReadToEnd();

        File.WriteAllText(configFilePath, contents);
    }

    internal static void ReadYaml(string yamlInput)
    {
        IDeserializer deserializer = new DeserializerBuilder().Build();
        RapidLoadoutsPlugin.RL_yamlData = deserializer.Deserialize<List<ItemSet>>(yamlInput)!;
        // log the yaml data
        RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug($"yamlData:\n{yamlInput}");
    }
    
    // Write the yaml data to the file
    internal static void WriteYaml(string yamlPath)
    {
        ISerializer serializer = new SerializerBuilder().Build();
        string yamlOutput = serializer.Serialize(RapidLoadoutsPlugin.RL_yamlData);
        File.WriteAllText(yamlPath, yamlOutput);
    }

    internal void LoadCustomItemSets()
    {
        if (!File.Exists(RapidLoadoutsPlugin.yamlPath))
        {
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogWarning($"Could not find {RapidLoadoutsPlugin.yamlFileName} at {RapidLoadoutsPlugin.yamlPath}");
            return;
        }

        try
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            List<ItemSets.ItemSet> customSets = deserializer.Deserialize<List<ItemSets.ItemSet>>(File.ReadAllText(RapidLoadoutsPlugin.yamlPath));

            // Assuming we have a way to add the new item sets
            // AddItemSets(customSets);
        }
        catch (Exception e)
        {
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError($"Failed to load custom item sets: {e.Message}");
        }
    }
}

public static class RegexUtilities
{
    private static readonly Regex AlphanumericRegex = new Regex(@"[^a-zA-Z0-9]", RegexOptions.Compiled);

    public static string TrimInvalidCharacters(string input)
    {
        return AlphanumericRegex.Replace(input, string.Empty);
    }
}