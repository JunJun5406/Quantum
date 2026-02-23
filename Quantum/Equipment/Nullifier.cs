using RoR2;
using RoR2.UI;
using Quantum.Configuration;
using System;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using HarmonyLib;
using System.Linq;
using TMPro;

namespace Quantum.Equipment
{
    public static partial class EquipInit
    {
        public static Nullifier Nullifier = new Nullifier
        (
            "Nullifier",
            false,
            false
        );
    }

    /// <summary>
    ///     // Ver.1
    ///     Meant to be a supplement to Nautilus, providing another way to get the new void boss items
    ///     Also allows better control of which void items you get while not giving youe extra; recycler alternative
    /// </summary>
    public class Nullifier : EquipBase
    {
        public override bool Enabled
        {
            get
            {
                return Nullifier_Enabled.Value;
            }
        }
        public override float Cooldown
        {
            get
            {
                return  Nullifier_Cooldown.Value;
            }
        }
        public override GameObject equipPrefab => OverwritePrefabMaterials();
        public override Sprite equipIcon => Main.Assets.LoadAsset<Sprite>("Assets/icons/nullifier.png");
        public Material material0 => Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarChest/matLunarChest.mat").WaitForCompletion();
        public Material material1 => Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarGolem/matLunarGolemRock.mat").WaitForCompletion();
        public Material material2 => Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarChest/matLunarChest.mat").WaitForCompletion();
        public Material material3 => Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidCoral.mat").WaitForCompletion();
        public Material material4 => Addressables.LoadAssetAsync<Material>("RoR2/DLC1/TreasureCacheVoid/matKeyVoid.mat").WaitForCompletion();
        public Material material5 => Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidBlinkTrail.mat").WaitForCompletion();
        private GameObject _indicatorPrefab = null;
        public GameObject IndicatorPrefab
        {
            get
            {
                if (_indicatorPrefab == null)
                {
                    _indicatorPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "nullIndicatorPrefab", false);
                    UnityEngine.Object.Destroy(_indicatorPrefab.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>());
                    _indicatorPrefab.GetComponentInChildren<SpriteRenderer>().sprite = Main.Assets.LoadAsset<Sprite>("Assets/icons/nullifierIndicator.png");
                    _indicatorPrefab.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                    _indicatorPrefab.GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.identity;
                    _indicatorPrefab.GetComponentInChildren<TextMeshPro>().color = new Color(1f, 0.31f, 0.976f);
                    _indicatorPrefab.GetComponentInChildren<TextMeshPro>().rectTransform.localPosition = new Vector3(1f, 0f, 0f);
                    while (_indicatorPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.length > 0) _indicatorPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.RemoveKey(0);
                    _indicatorPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0f, 2f);
                    _indicatorPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(0.5f, 1f);
                    _indicatorPrefab.GetComponentInChildren<ObjectScaleCurve>().overallCurve.AddKey(1f, 1f);
                }
                return _indicatorPrefab;
            }
            set;
        }
        private GameObject _explodeEffect;
        public GameObject ExplodeEffect
        {
            get
            {
                if (_explodeEffect == null)
                {
                    _explodeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/CritGlassesVoid/CritGlassesVoidExecuteEffect.prefab").WaitForCompletion();
                }
                return _explodeEffect;
            }
            set;
        }

        public Nullifier(string _name, bool canBeRandomlyTriggered = true, bool enigmaCompatible = true) : 
        base(_name, canBeRandomlyTriggered, enigmaCompatible){}

        // Config
        public static ConfigItem<bool> Nullifier_Enabled = new ConfigItem<bool>
        (
            "Equipment: Nullifier",
            "Equipment enabled",
            "Should this equipment appear in runs?",
            true
        );
        public static ConfigItem<float> Nullifier_Cooldown = new ConfigItem<float>
        (
            "Equipment: Nullifier",
            "Equipment cooldown",
            "Seconds until the equipment cooldown is finished.",
            60f,
            1f,
            120f,
            1f
        );


        public GameObject OverwritePrefabMaterials()
        {
            GameObject ret = Main.Assets.LoadAsset<GameObject>("Assets/prefabs/nullifier.prefab");

            Material[] materials =
            {
                material0,
                material1,
                material2,
                material3,
                material4,
                material5
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
                    Language.currentLanguage.GetLocalizedStringByToken(descriptionToken)
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
                            ItemDef ConversionItemDef = GetVoidConversion(pickupItemDef);

                            if 
                            (
                                pickupItemDef 
                                && ConversionItemDef
                                && (ConversionItemDef.tier == ItemTier.VoidTier1
                                || ConversionItemDef.tier == ItemTier.VoidTier2
                                || ConversionItemDef.tier == ItemTier.VoidTier3
                                || ConversionItemDef.tier == ItemTier.VoidBoss)
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
                            ItemDef ConversionItemDef = GetVoidConversion(pickupItemDef);

                            if 
                            (
                                ConversionItemDef && 
                                (ConversionItemDef.tier == ItemTier.VoidTier1
                                || ConversionItemDef.tier == ItemTier.VoidTier2
                                || ConversionItemDef.tier == ItemTier.VoidTier3
                                || ConversionItemDef.tier == ItemTier.VoidBoss)
                            )
                            {
                                self.subcooldownTimer = 0.2f;
                                
                                pickupController.pickup = pickupController.pickup.WithPickupIndex(PickupCatalog.FindPickupIndex(ConversionItemDef.itemIndex));
                                pickupController.ForcePickupDisplayUpdate();

                                EffectData effectData = new EffectData()
                                {
                                    origin = pickupController.transform.position
                                };
                                EffectManager.SpawnEffect(ExplodeEffect, effectData, true);

                                return true;
                            }
                        }
                    }

                    return false;
                }

                return orig(self, equipmentDef);
            };
        }

        public ItemDef GetVoidConversion(ItemDef pickupItemDef)
        {
            if (!pickupItemDef)
            {
                return null;
            }
            foreach (ItemDef.Pair pair in ItemCatalog.GetItemPairsForRelationship(DLC1Content.ItemRelationshipTypes.ContagiousItem))
            {
                if (pair.itemDef1 == pickupItemDef || pair.itemDef2 == pickupItemDef)
                {
                    return pair.itemDef1 == pickupItemDef ? pair.itemDef2 : pair.itemDef1;
                }
            }
            return null;
        }

        // IDR
        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            GameObject ItemDisplayPrefab = Helpers.PrepareItemDisplayModel(PrefabAPI.InstantiateClone(equipPrefab, EquipmentDef.name + "Display", false));
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();

            #region IDR
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.15102F, 0.58803F, 0.00949F),
                        localAngles = new Vector3(79.80753F, 254.9617F, 354.9219F),
                        localScale = new Vector3(1.83365F, 1.37563F, 1.03639F)
                    }
                }
            );
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0.19451F, 0.33624F, 0.10903F),
                        localAngles = new Vector3(12.50401F, 129.5965F, 25.3747F),
                        localScale = new Vector3(1F, 1F, 0.8F)
                    }
                }
            );
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "MuzzleShotgun",
                        localPos = new Vector3(-0.00009F, 0.00008F, -0.62314F),
                        localAngles = new Vector3(0F, 180F, 270F),
                        localScale = new Vector3(1.16058F, 1.0957F, 1.04499F)
                    }
                }
            );
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(2.42325F, 2.97614F, 0.00004F),
                        localAngles = new Vector3(0F, 180F, 0F),
                        localScale = new Vector3(5.89671F, 5.89671F, 5.89671F)
                    }
                }
            );
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "CannonHeadR",
                        localPos = new Vector3(-0.14386F, 0.26282F, 0.18951F),
                        localAngles = new Vector3(90F, 53.19468F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "LowerArmR",
                        localPos = new Vector3(0.00002F, -0.00003F, 0.20274F),
                        localAngles = new Vector3(90F, 0F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.15102F, 0.58803F, 0.00949F),
                        localAngles = new Vector3(80.58149F, 158.2307F, 245.5665F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "WeaponPlatform",
                        localPos = new Vector3(0.00003F, 0.08532F, 0.48911F),
                        localAngles = new Vector3(90F, 0F, 0F),
                        localScale = new Vector3(1.44278F, 1.44278F, 1.44278F)
                    }
                }
            );
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0.27765F, 0.43124F, 0F),
                        localAngles = new Vector3(0F, 180F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "LowerArmR",
                        localPos = new Vector3(0.00002F, 2.33054F, 1.60938F),
                        localAngles = new Vector3(90F, 0F, 0F),
                        localScale = new Vector3(7.38603F, 7.38603F, 7.38603F)
                    }
                }
            );
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.15102F, 0.58803F, 0.00949F),
                        localAngles = new Vector3(79.36234F, 242.552F, 342.715F),
                        localScale = new Vector3(1.83365F, 1.37563F, 1.03639F)
                    }
                }
            );
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Backpack",
                        localPos = new Vector3(-0.21197F, 0.32245F, -0.14205F),
                        localAngles = new Vector3(22.1867F, 129.5542F, 5.53329F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "ForeArmL",
                        localPos = new Vector3(-0.08994F, 0.59968F, 0.15717F),
                        localAngles = new Vector3(90F, 0F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlSeeker", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "LowerArmR",
                        localPos = new Vector3(0.14325F, 0.55108F, -0.00001F),
                        localAngles = new Vector3(90F, 90F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlFalseSon", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.00001F, 0.94533F, -0.28242F),
                        localAngles = new Vector3(90F, 180F, 0F),
                        localScale = new Vector3(1.83365F, 1.37563F, 1.03639F)
                    }
                }
            );
            rules.Add("mdlChef", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.33366F, 0F, -0.35395F),
                        localAngles = new Vector3(85.73878F, 0.71592F, 90.00011F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlDroneTech", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "HandL",
                        localPos = new Vector3(-0.37943F, -0.04341F, 0.14472F),
                        localAngles = new Vector3(0F, 90F, 90F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlDrifter", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "BagPocketL",
                        localPos = new Vector3(-0.14644F, 0F, 0F),
                        localAngles = new Vector3(0F, 180F, 0F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            #endregion
            
            return rules;
        }
    }
}