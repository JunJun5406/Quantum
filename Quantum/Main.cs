using BepInEx;
using R2API.Utils;
using RiskOfOptions;
using RoR2;
using RoR2.ExpansionManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using System.Collections.Generic;
using System.Reflection;
using RiskOfOptions.Options;
using BepInEx.Configuration;
using System;
using Quantum.Equipment;
using Quantum.Items;
// using ShaderSwapper;

namespace Quantum
{
    [BepInPlugin(QUANTUM_GUID, QUANTUM_NAME, QUANTUM_VER)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.items", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string QUANTUM_GUID = "com.Hex3.Quantum";
        public const string QUANTUM_NAME = "Quantum";
        public const string QUANTUM_VER = "1.0.0";
        public static Main Instance;
        public static ExpansionDef Expansion;
        public static AssetBundle Assets;
        public static ConfigEntry<bool> Config_Enabled;

        public void Awake()
        {
            Log.Init(Logger);
            Log.Info($"Init {QUANTUM_NAME} {QUANTUM_VER}");

            Instance = this;

            Log.Info("Creating assets...");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Quantum.quantumvfx"))
            {
                Assets = AssetBundle.LoadFromStream(stream);
            }
            // base.StartCoroutine(Assets.UpgradeStubbedShadersAsync());

            Log.Info($"Creating config...");
            Config_Enabled = Instance.Config.Bind(new ConfigDefinition("CONFIG - IMPORTANT", "Enable custom config"), false, new ConfigDescription("Set to 'true' to enable custom configuration for this mod. False by default to allow balance changes to take effect.", null, Array.Empty<object>()));
            if (Compat.RiskOfOptions)
            {
                Log.Info($"Detected RiskOfOptions");
                ModSettingsManager.SetModDescription("Adds new vanilla-style equipment.");
                ModSettingsManager.SetModIcon(Assets.LoadAsset<Sprite>("Assets/icons/expansion.png"));
                ModSettingsManager.AddOption
                (
                    new CheckBoxOption
                    (
                        Config_Enabled,
                        true
                    )
                );
            }

            Log.Info($"Creating expansion...");
            Expansion = ScriptableObject.CreateInstance<ExpansionDef>();
            Expansion.name = QUANTUM_NAME;
            Expansion.nameToken = "QT_EXPANSION_NAME";
            Expansion.descriptionToken = "QT_EXPANSION_DESC";
            Expansion.iconSprite = Assets.LoadAsset<Sprite>("Assets/icons/expansionQt.png");
            Expansion.disabledIconSprite = Assets.LoadAsset<Sprite>("Assets/icons/expansionQt-inactive.png");
            Expansion.requiredEntitlement = null;
            ContentAddition.AddExpansionDef(Expansion);

            Log.Info($"Creating equipments...");
            EquipInit.Init();

            Log.Info($"Creating items...");
            ItemInit.Init();

            On.RoR2.RoR2Application.OnMainMenuControllerInitialized += (orig, self) =>
            {
                EquipInit.FormatDescriptions();
                ItemInit.FormatDescriptions();
                orig(self);
            };

            Log.Info($"Done");
        }
    }
}
