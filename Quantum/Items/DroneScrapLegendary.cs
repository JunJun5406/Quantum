using RoR2;
using System;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using HarmonyLib;
using UnityEngine.Networking;
using RoR2.Items;
using Quantum.Equipment;

namespace Quantum.Items
{
    public static partial class ItemInit
    {
        public static DroneScrapLegendary DroneScrapLegendary = new DroneScrapLegendary
        (
            "DroneScrapLegendary",
            [ItemTag.WorldUnique, ItemTag.EquipmentRelated, ItemTag.AIBlacklist, ItemTag.ExtractorUnitBlacklist, ItemTag.BrotherBlacklist],
            ItemTier.Tier3
        );
    }

    /// <summary>
    ///     // Ver.1
    /// </summary>
    public class DroneScrapLegendary : ItemBase
    {
        public override bool Enabled
        {
            get
            {
                return DroneScrap.DroneScrap_Enabled.Value;
            }
        }
        public override GameObject itemPrefab => OverwritePrefabMaterials();
        public Material material0 => Addressables.LoadAssetAsync<Material>("RoR2/Base/goolake/matGoolakeStoneTrimLightSand.mat").WaitForCompletion();
        public Material material1 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Drones/matBackupDrone.mat").WaitForCompletion();
        public Material material2 => Addressables.LoadAssetAsync<Material>("RoR2/GlobalContent/matArtifact.mat").WaitForCompletion();
        public Material material3 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitan.mat").WaitForCompletion();
        public Material material4 => Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidAsteroid.mat").WaitForCompletion();
        public Material material5 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Vagrant/matVagrantCannonGreen.mat").WaitForCompletion();
        public Material material6 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Vagrant/matVagrantCannonRed.mat").WaitForCompletion();
        public Material material7 => Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/matVoidmetalTrim.mat").WaitForCompletion();
        public Material material8 => Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarDagger/matLunarDagger.mat").WaitForCompletion();
        public override Sprite itemIcon => Main.Assets.LoadAsset<Sprite>("Assets/icons/droneScrapItemLegendary.png");

        public DroneScrapLegendary(string _name, ItemTag[] _tags, ItemTier _tier, bool _canRemove = true, bool _isConsumed = false, bool _hidden = false) : 
        base(_name, _tags, _tier, _canRemove, _isConsumed, _hidden){}

        public GameObject OverwritePrefabMaterials()
        {
            GameObject ret = Main.Assets.LoadAsset<GameObject>("Assets/prefabs/droneScrapItem3.prefab");

            Material[] materials =
            {
                material0,
                material1,
                material2,
                material3,
                material4,
                material5,
                material6,
                material7,
                material8
            };
            ret.GetComponentInChildren<MeshRenderer>().SetMaterialArray(materials);

            return ret;
        }

        
        // Tokens
        public override void FormatDescriptionTokens()
        {
            string descriptionToken = ItemDef.descriptionToken;
            string pickupToken = ItemDef.pickupToken;

            LanguageAPI.AddOverlay
            (
                descriptionToken,
                String.Format
                (
                    Language.currentLanguage.GetLocalizedStringByToken(descriptionToken),
                    DroneScrap.DroneScrap_LegendaryItemsNeeded.Value
                )
            );

            LanguageAPI.AddOverlay
            (
                pickupToken,
                String.Format
                (
                    Language.currentLanguage.GetLocalizedStringByToken(pickupToken),
                    DroneScrap.DroneScrap_CommonItemsNeeded.Value
                )
            );
        }

        // Hooks
        public override void RegisterHooks()
        {

        }
    }
}