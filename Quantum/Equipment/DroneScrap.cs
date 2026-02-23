using RoR2;
using Quantum.Configuration;
using System;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using HarmonyLib;
using Quantum.Items;
using RoR2.ExpansionManagement;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.Equipment
{
    public static partial class EquipInit
    {
        public static DroneScrap DroneScrap = new DroneScrap
        (
            "DroneScrap",
            false,
            false
        );
    }

    /// <summary>
    ///     // Ver.1
    ///     There aren't enough equips to compete with Recycler for build control, and with all the new additions to drones in AC, it feels right to have an option for creating your own
    ///     Especially good for guaranteeing value from bad legendary drops, since red printers are so uncommon
    ///     Naturally avoids too many drones by having higher costs at lower tiers
    /// </summary>
    public class DroneScrap : EquipBase
    {
        public override bool Enabled
        {
            get
            {
                return DroneScrap_Enabled.Value;
            }
        }
        public override float Cooldown
        {
            get
            {
                return  DroneScrap_Cooldown.Value;
            }
        }
        public override GameObject equipPrefab => OverwritePrefabMaterials();
        public override Sprite equipIcon => Main.Assets.LoadAsset<Sprite>("Assets/icons/droneScrapEquip.png");
        public Material material0 => Addressables.LoadAssetAsync<Material>("RoR2/Base/goolake/matGoolakeStoneTrimLightSand.mat").WaitForCompletion();
        public Material material1 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Drones/matBackupDrone.mat").WaitForCompletion();
        public Material material2 => Addressables.LoadAssetAsync<Material>("RoR2/Base/GoldOnHit/matBoneCrown.mat").WaitForCompletion();
        public Material material3 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitan.mat").WaitForCompletion();
        public Material material4 => Addressables.LoadAssetAsync<Material>("RoR2/DLC3/Drone Tech/Dronetech_glow2.mat").WaitForCompletion();
        public Material material5 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Vagrant/matVagrantCannonGreen.mat").WaitForCompletion();
        public Material material6 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Vagrant/matVagrantCannonRed.mat").WaitForCompletion();
        public Material material7 => Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/matVoidmetalTrim.mat").WaitForCompletion();
        public Material material8 => Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarDagger/matLunarDagger.mat").WaitForCompletion();
        public Material material9 => Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidTerrain.mat").WaitForCompletion();
        public Material material10 => Addressables.LoadAssetAsync<Material>("RoR2/Base/MultiShopEquipmentTerminal/matMultishopEquipment.mat").WaitForCompletion();
        public Material material11 => Addressables.LoadAssetAsync<Material>("RoR2/DLC3/Drones/bombardmentMat.mat").WaitForCompletion();
        public ExpansionDef alloyedExpansion => Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC3/DLC3.asset").WaitForCompletion();
        public List<DroneDef> CommonDroneDefs = new List<DroneDef>
        {
            Addressables.LoadAssetAsync<DroneDef>("RoR2/Base/Drones/Drone1.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<DroneDef>("RoR2/Base/Drones/Drone2.asset").WaitForCompletion(),
        };
        public List<DroneDef> CommonDroneDefsDLC3 = new List<DroneDef>
        {
            Addressables.LoadAssetAsync<DroneDef>("RoR2/DLC3/Drones/JunkDrone.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<DroneDef>("RoR2/DLC3/Drones/HaulerDrone.asset").WaitForCompletion(),
        };
        public List<DroneDef> UncommonDroneDefs = new List<DroneDef>
        {
            Addressables.LoadAssetAsync<DroneDef>("RoR2/Base/Drones/FlameDrone.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<DroneDef>("RoR2/Base/Drones/MissileDrone.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<DroneDef>("RoR2/Base/Drones/EmergencyDrone.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<DroneDef>("RoR2/Base/Drones/EquipmentDrone.asset").WaitForCompletion()
        };
        public List<DroneDef> UncommonDroneDefsDLC3 = new List<DroneDef>
        {
            Addressables.LoadAssetAsync<DroneDef>("RoR2/DLC3/Drones/CleanupDrone.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<DroneDef>("RoR2/DLC3/Drones/RechargeDrone.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<DroneDef>("RoR2/DLC3/Drones/JailerDrone.asset").WaitForCompletion()
        };
        public List<DroneDef> LegendaryDroneDefs = new List<DroneDef>
        {
            Addressables.LoadAssetAsync<DroneDef>("RoR2/Base/Drones/MegaDrone.asset").WaitForCompletion()
        };
        public List<DroneDef> LegendaryDroneDefsDLC3 = new List<DroneDef>
        {
            Addressables.LoadAssetAsync<DroneDef>("RoR2/DLC3/Drones/CopycatDrone.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<DroneDef>("RoR2/DLC3/Drones/BombardmentDrone.asset").WaitForCompletion()
        };

        // TODO mod support

        private GameObject _indicatorPrefab = null;
        public GameObject IndicatorPrefab
        {
            get
            {
                if (_indicatorPrefab == null)
                {
                    _indicatorPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/RecyclerIndicator"), "droneScrapIndicatorPrefab", false);
                }
                return _indicatorPrefab;
            }
            set;
        }

        public DroneScrap(string _name, bool canBeRandomlyTriggered = true, bool enigmaCompatible = true) :
        base(_name, canBeRandomlyTriggered, enigmaCompatible){}
        // Config
        public static ConfigItem<bool> DroneScrap_Enabled = new ConfigItem<bool>
        (
            "Equipment: Build It Kit",
            "Equipment enabled",
            "Should this equipment appear in runs?",
            true
        );
        public static ConfigItem<float> DroneScrap_Cooldown = new ConfigItem<float>
        (
            "Equipment: Build It Kit",
            "Equipment cooldown",
            "Seconds until the equipment cooldown is finished.",
            45f,
            1f,
            120f,
            1f
        );
        public static ConfigItem<int> DroneScrap_CommonItemsNeeded = new ConfigItem<int>
        (
            "Equipment: Build It Kit",
            "Common items per drone",
            "Required items for a common drone to be fabricated.",
            2,
            1f,
            5f,
            1f
        );
        public static ConfigItem<int> DroneScrap_UncommonItemsNeeded = new ConfigItem<int>
        (
            "Equipment: Build It Kit",
            "Uncommon items per drone",
            "Required items for an uncommon drone to be fabricated.",
            2,
            1f,
            5f,
            1f
        );
        public static ConfigItem<int> DroneScrap_LegendaryItemsNeeded = new ConfigItem<int>
        (
            "Equipment: Build It Kit",
            "Legendary items per drone",
            "Required items for a legendary drone to be fabricated.",
            1,
            1f,
            5f,
            1f
        );

        public GameObject OverwritePrefabMaterials()
        {
            GameObject ret = Main.Assets.LoadAsset<GameObject>("Assets/prefabs/droneScrapEquip.prefab");

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
                material8,
                material9,
                material10,
                material11
            };
            ret.GetComponentInChildren<MeshRenderer>().SetMaterialArray(materials);

            return ret;
        }

        // Tokens
        public override void FormatDescriptionTokens()
        {
            string descriptionToken = EquipmentDef.descriptionToken;

            LanguageAPI.AddOverlay
            (
                descriptionToken,
                String.Format
                (
                    Language.currentLanguage.GetLocalizedStringByToken(descriptionToken),
                    DroneScrap_CommonItemsNeeded.Value,
                    DroneScrap_UncommonItemsNeeded.Value,
                    DroneScrap_LegendaryItemsNeeded.Value
                )
            );
        }

        // Hooks
        public override void RegisterHooks()
        {
            // Show target on applicable items
            On.RoR2.EquipmentSlot.UpdateTargets += (orig, self, equipmentIndex, userShouldAnticipateTarget) =>
            {
                if (equipmentIndex == EquipmentIndex && self.stock > 0f)
                {
                    self.currentTarget = new EquipmentSlot.UserTargetInfo(self.FindPickupController(self.GetAimRay(), 10f, 30f, true, false));
                    GenericPickupController pickupController = self.currentTarget.pickupController;
                    bool foundValidItem = false;

                    if (pickupController && self.currentTarget.transformToIndicateAt)
                    {
                        UniquePickup uniquePickup = pickupController.pickup;
                        PickupDef pickupDef = PickupCatalog.GetPickupDef(uniquePickup.pickupIndex);
                        ItemDef pickupItemDef;

                        if (!uniquePickup.isTempItem && pickupDef != null && pickupDef.itemIndex != ItemIndex.None)
                        {
                            pickupItemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);

                            if 
                            (
                                pickupItemDef &&
                                (pickupItemDef.tier == ItemTier.Tier1
                                || pickupItemDef.tier == ItemTier.Tier2
                                || pickupItemDef.tier == ItemTier.Tier3)
                                && pickupItemDef.itemIndex != ItemInit.DroneScrapCommon.ItemIndex
                                && pickupItemDef.itemIndex != ItemInit.DroneScrapUncommon.ItemIndex
                                && pickupItemDef.itemIndex != ItemInit.DroneScrapLegendary.ItemIndex
                            )
                            {
                                foundValidItem = true;
                                self.targetIndicator.visualizerPrefab = IndicatorPrefab;
                            }
                        }
                    }

                    self.targetIndicator.active = foundValidItem;
                    self.targetIndicator.targetTransform = foundValidItem ? self.currentTarget.transformToIndicateAt : null;
                }
                else
                {
                    orig(self, equipmentIndex, userShouldAnticipateTarget);
                }
            };

            // Convert item
            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, equipmentDef) =>
            {
                if (!NetworkServer.active)
                {
                    Debug.LogWarning("[Server] function 'System.Boolean RoR2.EquipmentSlot::PerformEquipmentAction(RoR2.EquipmentDef)' called on client");
                    return false;
                }
                if (self.equipmentDisabled)
                {
                    return false;
                }
                if (equipmentDef == EquipmentDef)
                {
                    self.UpdateTargets(EquipmentIndex, userShouldAnticipateTarget: false);
                    GenericPickupController pickupController = self.currentTarget.pickupController;

                    if (pickupController)
                    {
                        UniquePickup uniquePickup = pickupController.pickup;
                        PickupDef pickupDef = PickupCatalog.GetPickupDef(uniquePickup.pickupIndex);
                        ItemDef pickupItemDef;

                        if (!uniquePickup.isTempItem && pickupDef != null && pickupDef.itemIndex != ItemIndex.None)
                        {
                            pickupItemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);

                            if
                            (
                                pickupItemDef &&
                                (pickupItemDef.tier == ItemTier.Tier1
                                || pickupItemDef.tier == ItemTier.Tier2
                                || pickupItemDef.tier == ItemTier.Tier3)
                                && pickupItemDef.itemIndex != ItemInit.DroneScrapCommon.ItemIndex
                                && pickupItemDef.itemIndex != ItemInit.DroneScrapUncommon.ItemIndex
                                && pickupItemDef.itemIndex != ItemInit.DroneScrapLegendary.ItemIndex
                            )
                            {
                                self.subcooldownTimer = 0.2f;

                                ItemIndex convertToItemIndex = ItemIndex.None;
                                switch(pickupItemDef.tier)
                                {
                                    case ItemTier.Tier1:
                                        convertToItemIndex = ItemInit.DroneScrapCommon.ItemIndex;
                                        break;
                                    case ItemTier.Tier2:
                                        convertToItemIndex = ItemInit.DroneScrapUncommon.ItemIndex;
                                        break;
                                    case ItemTier.Tier3:
                                        convertToItemIndex = ItemInit.DroneScrapLegendary.ItemIndex;
                                        break;
                                    default:
                                        // Log.Error("This run is ass. Session Terminated");
                                        // Application.Quit();
                                        break;
                                }

                                pickupController.pickup = pickupController.pickup.WithPickupIndex(PickupCatalog.FindPickupIndex(convertToItemIndex));
                                pickupController.ForcePickupDisplayUpdate();

                                EffectData effectData = new EffectData()
                                {
                                    origin = pickupController.transform.position
                                };
                                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniRecycleEffect"), effectData, true);

                                return true;
                            }
                        }
                    }

                    return false;
                }

                return orig(self, equipmentDef);
            };

            // Spawn drone on pickup
            On.RoR2.Inventory.GiveItemPermanent_ItemIndex_int += (orig, self, itemIndex, count) =>
            {
                orig(self, itemIndex, count);

                if 
                (
                    self.GetComponent<CharacterMaster>()
                    && self.GetComponent<CharacterMaster>().GetBody()
                    && (itemIndex == ItemInit.DroneScrapCommon.ItemIndex
                    || itemIndex == ItemInit.DroneScrapUncommon.ItemIndex
                    || itemIndex == ItemInit.DroneScrapLegendary.ItemIndex)
                )
                {
                    CharacterMaster characterMaster = self.GetComponent<CharacterMaster>();
                    CharacterBody characterBody = characterMaster.GetBody();

                    int scrapCount = self.GetItemCountPermanent(itemIndex);
                    bool overThreshold = false;
                    List<DroneDef> droneDefs = new();

                    if (itemIndex == ItemInit.DroneScrapCommon.ItemDef.itemIndex)
                    {
                        overThreshold = scrapCount >= DroneScrap_CommonItemsNeeded.Value;
                        droneDefs.AddRange(CommonDroneDefs);
                        if (Run.instance.IsExpansionEnabled(alloyedExpansion))
                        {
                            droneDefs.AddRange(CommonDroneDefsDLC3);
                        }
                    }
                    else if (itemIndex == ItemInit.DroneScrapUncommon.ItemDef.itemIndex)
                    {
                        overThreshold = scrapCount >= DroneScrap_UncommonItemsNeeded.Value;
                        droneDefs.AddRange(UncommonDroneDefs);
                        if (Run.instance.IsExpansionEnabled(alloyedExpansion))
                        {
                            droneDefs.AddRange(UncommonDroneDefsDLC3);
                        }
                    }
                    else
                    {
                        overThreshold = scrapCount >= DroneScrap_LegendaryItemsNeeded.Value;
                        droneDefs.AddRange(LegendaryDroneDefs);
                        if (Run.instance.IsExpansionEnabled(alloyedExpansion)) 
                        { 
                            droneDefs.AddRange(LegendaryDroneDefsDLC3); 
                        }
                    }

                    if (overThreshold)
                    {
                        Util.ShuffleList(droneDefs);
                        DroneDef chosenDroneDef = droneDefs.First();
                        
                        Log.Info(chosenDroneDef.name);

                        CharacterMaster characterMasterSummon = new MasterSummon
                        {
                            masterPrefab = chosenDroneDef.masterPrefab,
                            position = new Vector3(characterBody.corePosition.x, characterBody.corePosition.y + 2f, characterBody.corePosition.z),
                            rotation = Quaternion.identity,
                            summonerBodyObject = characterBody.gameObject,
                            ignoreTeamMemberLimit = true,
                            useAmbientLevel = true,
                            enablePrintController = true
                        }.Perform();

                        if (chosenDroneDef == RoR2Content.DroneDefs.EquipmentDrone)
                        {
                            List<EquipmentDef> validEquipmentDefs = EquipmentCatalog.equipmentDefs.ToList();
                            validEquipmentDefs.RemoveAll(def => def.canBeRandomlyTriggered == false);
                            validEquipmentDefs.RemoveAll(def => def.equipmentIndex == EquipmentIndex.None);
                            Util.ShuffleList(validEquipmentDefs);

                            characterMasterSummon.inventory.SetEquipmentIndex(validEquipmentDefs.First().equipmentIndex);
                        }

                        EffectData effectData = new EffectData()
                        {
                            origin = characterBody.corePosition
                        };
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniRecycleEffect"), effectData, true);

                        self.RemoveItemPermanent(itemIndex, scrapCount);
                    }
                }
            };
        }

        // IDR
        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            GameObject ItemDisplayPrefab = Helpers.PrepareItemDisplayModel(PrefabAPI.InstantiateClone(ItemInit.DroneScrapCommon.itemPrefab, EquipmentDef.name + "Display", false));
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();

            #region IDR
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0F, -0.00004F, 0.2538F),
                        localAngles = new Vector3(0F, 0F, 180F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0F, -0.0325F, 0.13983F),
                        localAngles = new Vector3(306.028F, 0F, 180F),
                        localScale = new Vector3(1F, 1F, 0.8F)
                    }
                }
            );
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Hat",
                        localPos = new Vector3(-0.00006F, 0.06092F, -0.02317F),
                        localAngles = new Vector3(339.2563F, 355.5712F, 1.6066F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0.00001F, 0.47329F, -2.53093F),
                        localAngles = new Vector3(0F, 180F, 0F),
                        localScale = new Vector3(7.55081F, 7.55081F, 7.55081F)
                    }
                }
            );
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0F, -0.00002F, 0.3979F),
                        localAngles = new Vector3(0F, 270F, 180F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0F, 0.00008F, -0.36404F),
                        localAngles = new Vector3(0F, 270F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0F, 0.07132F, 0.25553F),
                        localAngles = new Vector3(0F, 270F, 180F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Base",
                        localPos = new Vector3(0F, -0.64104F, 0.06493F),
                        localAngles = new Vector3(0F, 270F, 90.00002F),
                        localScale = new Vector3(2.03188F, 2.03188F, 2.03188F)
                    }
                }
            );
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0F, -0.00001F, -0.49876F),
                        localAngles = new Vector3(0F, 270F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0F, 1.92186F, 4.57324F),
                        localAngles = new Vector3(68.10463F, 0F, 0F),
                        localScale = new Vector3(7.38603F, 7.38603F, 7.38603F)
                    }
                }
            );
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0F, 0.00002F, -0.2643F),
                        localAngles = new Vector3(348.8108F, 180F, 180F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "FootR",
                        localPos = new Vector3(0.00001F, 0.13002F, -0.07988F),
                        localAngles = new Vector3(324.9968F, 180F, 180F),
                        localScale = new Vector3(1F, 1F, 2.05424F)
                    }
                }
            );
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0.00001F, 0.11035F, -0.35021F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlSeeker", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pack",
                        localPos = new Vector3(0.16809F, 0.06523F, -0.19531F),
                        localAngles = new Vector3(315.5359F, 270F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlFalseSon", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0F, 0.15214F, 0F),
                        localAngles = new Vector3(0F, 180F, 0F),
                        localScale = new Vector3(2.43958F, 1.63636F, 1.63636F)
                    }
                }
            );
            rules.Add("mdlChef", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.00001F, -0.36478F, -0.19438F),
                        localAngles = new Vector3(0F, 0F, 90F),
                        localScale = new Vector3(1.19805F, 1.19805F, 1.19805F)
                    }
                }
            );
            rules.Add("mdlDroneTech", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Head",
                        localPos = new Vector3(-0.28859F, 0F, 0F),
                        localAngles = new Vector3(0F, 0F, 90F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlDrifter", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "BagFrontPocket",
                        localPos = new Vector3(0F, 0.45212F, 0F),
                        localAngles = new Vector3(0F, 0F, 0F),
                        localScale = new Vector3(1.24082F, 1.24082F, 1.24082F)
                    }
                }
            );
            #endregion

            return rules;
        }
    }
}