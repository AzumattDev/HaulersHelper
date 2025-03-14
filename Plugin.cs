using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using ServerSync;
using UnityEngine;

namespace HaulersHelper
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("com.bepis.bepinex.configurationmanager", BepInDependency.DependencyFlags.SoftDependency)]
    public class HaulersHelperPlugin : BaseUnityPlugin
    {
        internal const string ModName = "HaulersHelper";
        internal const string ModVersion = "1.0.4";
        internal const string Author = "Azumatt";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        internal static string ConnectionError = "";

        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource HaulersHelperLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        internal static readonly List<string> Wagons = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonDetachDistanceTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<Vector3>> WagonAttachOffsetTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<Vector3>> WagonLineAttachOffsetTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonBreakForceTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonSpringTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonSpringDampingTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonBaseMassTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonItemWeightMassFactorTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonMinPitchTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonMaxPitchTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonMaxPitchVelTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonMaxVolTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonMaxVolVelTweakConfigs = new();
        internal static readonly Dictionary<string, ConfigEntry<float>> WagonAudioChangeSpeedTweakConfigs = new();

        internal static readonly Regex CleaningRegex = new("""['\["\]]""");
        internal static HaulersHelperPlugin ModContext = null!;

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public void Awake()
        {
            ModContext = this;
            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On, "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
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
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                HaulersHelperLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                HaulersHelperLogger.LogError($"There was an issue loading your {ConfigFileName}");
                HaulersHelperLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;

        internal ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription = new(description.Description + (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"), description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        internal ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        internal class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public string? Category;
            [UsedImplicitly] public int? Order;
            [UsedImplicitly] public bool? Browsable = true;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() => "# Acceptable values: " + string.Join(", ", UnityInput.Current.SupportedKeyCodes);
        }

        #endregion
    }
}