using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using RapidLoadouts.UI;
using RapidLoadouts.YAMLStuff;
using ServerSync;
using UnityEngine;

namespace RapidLoadouts
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class RapidLoadoutsPlugin : BaseUnityPlugin
    {
        internal const string ModName = "RapidLoadouts";
        internal const string ModVersion = "1.0.8";
        internal const string Author = "Azumatt";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        public static bool HasAuga;
        internal static string ConnectionError = "";

        private readonly Harmony _harmony = new(ModGUID);

        private LoadoutsButtonHolder _loadoutsButton = null!;

        public static readonly ManualLogSource RapidLoadoutsLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        internal static readonly string yamlFileName = SanitizeFileName($"{Author}.{ModName}_ItemSets.yml");

        internal static readonly string yamlPath = Paths.ConfigPath + Path.DirectorySeparatorChar + yamlFileName;
        internal static readonly CustomSyncedValue<string> AzuRL_yamlData = new(ConfigSync, "AzuRL_yamlData", "");

        internal static List<ItemSet?> RL_yamlData = new();

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public void Awake()
        {
            Localizer.Load();

            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On, "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            itemSetCostPrefab = config("1 - General", "ItemSetCostPrefab", "Coins", "Default Currency. This is the fallback currency that will be used when buying ItemSets from the ItemSet Window.");
            InventoryItemSetButtonPosition = config("2 - UI", "ItemSetButtonPosition", new Vector3(63.0f, -316.0f, -1.0f), "The last saved ItemSet Button's screen position.");
            ItemSetWindow = config("2 - UI", "ItemSetWindow", new Vector3(428f, -276f, -1.0f), "The last saved ItemSet Window's screen position.");


            if (!File.Exists(yamlPath))
            {
                YAMLUtils.WriteConfigFileFromResource(yamlPath);
            }

            AzuRL_yamlData.ValueChanged += OnValChangedUpdate; // check for file changes
            AzuRL_yamlData.AssignLocalValue(File.ReadAllText(yamlPath));

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void Start()
        {
            _loadoutsButton = gameObject.AddComponent<LoadoutsButtonHolder>();
            // If the chainloader has auga
            if (Chainloader.PluginInfos.ContainsKey("randyknapp.mods.auga"))
            {
                HasAuga = true;
            }
        }

        public static string SanitizeFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }


        private void OnDestroy()
        {
            Config.Save();
        }

        private void AutoDoc()
        {
#if DEBUG
            // Store Regex to get all characters after a [
            Regex regex = new(@"\[(.*?)\]");

            // Strip using the regex above from Config[x].Description.Description
            string Strip(string x) => regex.Match(x).Groups[1].Value;
            StringBuilder sb = new();
            string lastSection = "";
            foreach (ConfigDefinition x in Config.Keys)
            {
                // skip first line
                if (x.Section != lastSection)
                {
                    lastSection = x.Section;
                    sb.Append($"{Environment.NewLine}`{x.Section}`{Environment.NewLine}");
                }

                sb.Append($"\n{x.Key} [{Strip(Config[x].Description.Description)}]" +
                          $"{Environment.NewLine}   * {Config[x].Description.Description.Replace("[Synced with Server]", "").Replace("[Not Synced with Server]", "")}" +
                          $"{Environment.NewLine}     * Default Value: {Config[x].GetSerializedValue()}{Environment.NewLine}");
            }

            File.WriteAllText(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, $"{ModName}_AutoDoc.md"),
                sb.ToString());
#endif
        }


        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;

            FileSystemWatcher yamlwatcher = new(Paths.ConfigPath, yamlFileName);
            yamlwatcher.Changed += ReadYamlFiles;
            yamlwatcher.Created += ReadYamlFiles;
            yamlwatcher.Renamed += ReadYamlFiles;
            yamlwatcher.IncludeSubdirectories = true;
            yamlwatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            yamlwatcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                RapidLoadoutsLogger.LogDebugIfDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                RapidLoadoutsLogger.LogError($"There was an issue loading your {ConfigFileName}");
                RapidLoadoutsLogger.LogError("Please check your config entries for spelling and format!");
            }
        }

        private void ReadYamlFiles(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(yamlPath)) return;
            try
            {
                RapidLoadoutsLogger.LogDebugIfDebug("ReadConfigValues called");
                AzuRL_yamlData.AssignLocalValue(File.ReadAllText(yamlPath));
            }
            catch
            {
                RapidLoadoutsLogger.LogError($"There was an issue loading your {yamlFileName}");
                RapidLoadoutsLogger.LogError("Please check your entries for spelling and format!");
            }
        }

        private static void OnValChangedUpdate()
        {
            RapidLoadoutsLogger.LogDebugIfDebug("OnValChanged called");
            try
            {
                YAMLUtils.ReadYaml(AzuRL_yamlData.Value);
                ItemSetHelper.AddCreateLoadout(ItemSets.instance != null ? ItemSets.instance : null);
                if (Player.m_localPlayer != null && ItemSets.instance != null)
                    PurchasableLoadoutGui.FillList();
            }
            catch (Exception e)
            {
                RapidLoadoutsLogger.LogError($"Failed to deserialize {yamlFileName}: {e}");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;
        public static ConfigEntry<string> itemSetCostPrefab = null!;
        public static ConfigEntry<Vector3> InventoryItemSetButtonPosition = null!;
        public static ConfigEntry<Vector3> ItemSetWindow = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription = new(description.Description + (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"), description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            public bool? Browsable = false;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", UnityInput.Current.SupportedKeyCodes);
        }

        #endregion
    }

    // Logger extensions
    public static class LoggerExtensions
    {
        public static void LogDebugIfDebug(this ManualLogSource logger, string message)
        {
#if DEBUG
            logger.LogDebugIfDebug(message);
#endif
        }

        public static void LogErrorIfDebug(this ManualLogSource logger, string message)
        {
#if DEBUG
            logger.LogError(message);
#endif
        }

        public static void LogWarningIfDebug(this ManualLogSource logger, string message)
        {
#if DEBUG
            logger.LogWarning(message);
#endif
        }
    }
#if DEBUG
    [HarmonyPatch(typeof(ItemSets), nameof(ItemSets.Awake))]
    static class ItemSetsAwakePatch
    {
        static void Postfix(ItemSets __instance)
        {
            foreach (ItemSets.ItemSet? itemSet in __instance.m_sets)
            {
                if (itemSet != null)
                {
                    // Debug information for what is currently in the list and what is being considered for addition
                    RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebugIfDebug($"Attempting to add ItemSet {itemSet.m_name}");


                    RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebugIfDebug($"Adding ItemSet {itemSet.m_name}");

                    var set = new ItemSet
                    {
                        m_name = itemSet.m_name.Trim(),
                        m_items = itemSet.m_items?.Select(item => item == null
                            ? null
                            : new SetItem
                            {
                                m_item = item.m_item?.gameObject?.name ?? "Resin",
                                m_quality = item.m_quality,
                                m_stack = item.m_stack,
                                m_use = item.m_use,
                                m_hotbarSlot = item.m_hotbarSlot
                            }).ToList(),
                        m_skills = itemSet.m_skills?.Select(skill => skill == null
                            ? null
                            : new SetSkill
                            {
                                m_skill = skill.m_skill.ToString(),
                                m_level = skill.m_level
                            }).ToList(),
                        m_dropCurrent = true,
                        m_price = 999,
                        m_prefabCost = "Bronze",
                        m_setEffect = "Potion_eitr_minor",
                        m_setEffectAsGP = false
                    };

                    // Save the set to a yaml file
                    RapidLoadoutsPlugin.RL_yamlData.Add(set);
                    // Save the set to the file
                    YAMLUtils.WriteYaml(RapidLoadoutsPlugin.yamlPath);
                }
            }
        }
    }
#endif
}