using RoR2;
using Quantum.Configuration;
using System;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Quantum.Items;
using RoR2.ExpansionManagement;
using System.Collections.Generic;
using System.Linq;
using RoR2.Orbs;
using RoR2.ContentManagement;

namespace Quantum.Equipment
{
    public static partial class EquipInit
    {
        public static Microwave Microwave = new Microwave
        (
            "Microwave",
            false,
            false
        );
    }

    /// <summary>
    ///     // Ver.1
    ///     Enough health equipments exist, so here is one that synergizes with high shields instead
    ///     It also gives you a taste of Tesla Coil without needing to pick it up, which can be good for clearing out any large hordes around you
    /// </summary>
    public class Microwave : EquipBase
    {
        public override bool Enabled
        {
            get
            {
                return Microwave_Enabled.Value;
            }
        }
        public override float Cooldown
        {
            get
            {
                return Microwave_Cooldown.Value;
            }
        }
        public override GameObject equipPrefab => OverwritePrefabMaterials();
        public override Sprite equipIcon => Main.Assets.LoadAsset<Sprite>("Assets/icons/microwave.png");
        public Material material0 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/TrimSheets/matTrimSheetMetal.mat").WaitForCompletion();
        public Material material1 => Addressables.LoadAssetAsync<Material>("RoR2/Base/GoldOnHit/matBoneCrown.mat").WaitForCompletion();
        public Material material2 => Addressables.LoadAssetAsync<Material>("RoR2/DLC3/Items/SharedSuffering/matSharedSufferingEmission.mat").WaitForCompletion();
        public Material material3 => Addressables.LoadAssetAsync<Material>("RoR2/DLC3/Items/SharedSuffering/matSharedSufferingEmission.mat").WaitForCompletion();
        public Material material4 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitanPebble.mat").WaitForCompletion();
        public Material material5 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitanPebble.mat").WaitForCompletion();
        public Material material6 => Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidMetalTrimGrassy.mat").WaitForCompletion();
        public Material material7 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matDebugRed.mat").WaitForCompletion();
        public Material material8 => Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitanPebble.mat").WaitForCompletion();
        public Material material9 => Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarGolem/matLunarGolemMuzzleBlast.mat").WaitForCompletion();
        public Material material10 => Addressables.LoadAssetAsync<Material>("RoR2/DLC2/FalseSonBoss/matPrimeDevastatorLightningTether1.mat").WaitForCompletion();
        public Material material11 => Addressables.LoadAssetAsync<Material>("RoR2/DLC2/FalseSonBoss/matPrimeDevastatorLightningTether1.mat").WaitForCompletion();
        private Material _zapOverlay;
        public Material ZapOverlay
        {
            get
            {
                if (_zapOverlay == null)
                {
                    _zapOverlay = Addressables.LoadAssetAsync<Material>("RoR2/DLC2/FalseSonBoss/matPrimeDevastatorLightningTether1.mat").WaitForCompletion();
                }
                return _zapOverlay;
            }
            set;
        }
        private GameObject _zapEffect;
        public GameObject ZapEffect
        {
            get
            {
                if (_zapEffect == null)
                {
                    _zapEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazerSupplyDropNova.prefab").WaitForCompletion();
                }
                return _zapEffect;
            }
            set;
        }
        private GameObject _zapEffect2;
        public GameObject ZapEffect2
        {
            get
            {
                if (_zapEffect2 == null)
                {
                    _zapEffect2 = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Jellyfish/JellyfishNova.prefab").WaitForCompletion(), "quantumZapEffect2");
                    UnityEngine.Object.Destroy(_zapEffect2.transform.GetChild(4).gameObject);
                }
                return _zapEffect2;
            }
            set;
        }

        public Microwave(string _name, bool canBeRandomlyTriggered = true, bool enigmaCompatible = true) :
        base(_name, canBeRandomlyTriggered, enigmaCompatible)
        { }
        // Config
        public static ConfigItem<bool> Microwave_Enabled = new ConfigItem<bool>
        (
            "Equipment: Quantum Microwave",
            "Equipment enabled",
            "Should this equipment appear in runs?",
            true
        );
        public static ConfigItem<float> Microwave_Cooldown = new ConfigItem<float>
        (
            "Equipment: Quantum Microwave",
            "Equipment cooldown",
            "Seconds until the equipment cooldown is finished.",
            45f,
            1f,
            120f,
            1f
        );
        public static ConfigItem<int> Microwave_ShockCount = new ConfigItem<int>
        (
            "Equipment: Quantum Microwave",
            "Number of shocks",
            "Number of times a shock will occur within the duration.",
            5,
            1f,
            10f,
            1f
        );
        public static ConfigItem<int> Microwave_OrbCount = new ConfigItem<int>
        (
            "Equipment: Quantum Microwave",
            "Lightning strikes per shock",
            "Number of lightning projectiles fired per shock.",
            3,
            1f,
            10f,
            1f
        );
        public static ConfigItem<float> Microwave_ShockDamage = new ConfigItem<float>
        (
            "Equipment: Quantum Microwave",
            "Shock damage",
            "Fractional damage of each lightning projectile.",
            2f,
            1f,
            10f,
            0.1f
        );
        public static ConfigItem<float> Microwave_Radius = new ConfigItem<float>
        (
            "Equipment: Quantum Microwave",
            "Shock radius",
            "Meters radius for enemies to be shocked. 35m = Tesla Coil range.",
            35f,
            5f,
            60f,
            1f
        );
        public static ConfigItem<float> Microwave_ShieldRegen = new ConfigItem<float>
        (
            "Equipment: Quantum Microwave",
            "Shield regen",
            "Flat shield regenerated per shock.",
            30f,
            0f,
            90f,
            1f
        );
        public static ConfigItem<float> Microwave_SpeedBoost = new ConfigItem<float>
        (
            "Equipment: Quantum Microwave",
            "Speed boost",
            "Fractional speed boost throughout duration.",
            0.5f,
            0f,
            1f,
            0.1f
        );
        public static ConfigItem<float> Microwave_Interval = new ConfigItem<float>
        (
            "Equipment: Quantum Microwave",
            "Shock interval",
            "Seconds interval between shocks. Multiply by number of shocks for the full duration.",
            1f,
            0.1f,
            2f,
            0.1f
        );
        public static ConfigItem<float> Microwave_ProcCoefficient = new ConfigItem<float>
        (
            "Equipment: Quantum Microwave",
            "Shock proc coefficient",
            "Proc coefficient per lightning zap.",
            0.2f,
            0f,
            1f,
            0.1f
        );

        public GameObject OverwritePrefabMaterials()
        {
            GameObject ret = Main.Assets.LoadAsset<GameObject>("Assets/prefabs/microwave.prefab");

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
                    Microwave_ShockCount.Value,
                    Microwave_OrbCount.Value,
                    Microwave_ShockDamage.Value * 100f,
                    Microwave_Radius.Value,
                    Microwave_ShieldRegen.Value,
                    Microwave_SpeedBoost.Value * 100f
                )
            );
        }

        // Hooks
        public override void RegisterHooks()
        {
            // Speed boost
            RecalculateStatsAPI.GetStatCoefficients += (orig, self) =>
            {
                if (orig.gameObject.GetComponent<MicrowaveBehavior>() && orig.gameObject.GetComponent<MicrowaveBehavior>().speedBoosting == true)
                {
                    self.moveSpeedMultAdd += Microwave_SpeedBoost.Value;
                }
            };

            // Equipment action
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
                if (equipmentDef == EquipmentDef && self.characterBody)
                {
                    self.characterBody.gameObject.AddComponent<MicrowaveBehavior>();
                    self.subcooldownTimer = 1f;
                    return true;
                }

                return orig(self, equipmentDef);
            };

            // Give chef sfx bank on picking up equipment
            On.RoR2.Inventory.SetEquipmentInternal_EquipmentState_uint_uint += (orig, self, equipmentState, slot, set) =>
            {
                if (self.TryGetComponent(out CharacterMaster characterMaster) && characterMaster.GetBody())
                {
                    CharacterBody characterBody = characterMaster.GetBody();

                    if (equipmentState.equipmentIndex == EquipmentIndex && !characterBody.gameObject.GetComponent<MicrowaveAkBankHolder>())
                    {
                        characterBody.gameObject.AddComponent<MicrowaveAkBankHolder>();
                    }
                }

                return orig(self, equipmentState, slot, set);
            };
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
                        childName = "Chest",
                        localPos = new Vector3(0.00226F, 0.21302F, -0.34452F),
                        localAngles = new Vector3(15.32695F, 228.0691F, 42.65648F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0.2115F, 0.07005F, -0.23287F),
                        localAngles = new Vector3(41.4261F, 188.747F, 51.59148F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(0.08705F, 0.05628F, -0.32992F),
                        localAngles = new Vector3(40.45255F, 213.653F, 42.69826F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.40979F, 1.17178F, -3.60093F),
                        localAngles = new Vector3(36.65229F, 233.5275F, 39.76857F),
                        localScale = new Vector3(7.55081F, 7.55081F, 7.55081F)
                    }
                }
            );
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.05618F, 0.18586F, -0.48888F),
                        localAngles = new Vector3(31.5852F, 229.8302F, 32.59277F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.06478F, 0.03669F, -0.28937F),
                        localAngles = new Vector3(49.94817F, 320.0387F, 28.50902F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(-0.03672F, 0.03121F, 0.33444F),
                        localAngles = new Vector3(38.62289F, 307.8535F, 271.3212F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Base",
                        localPos = new Vector3(0.34422F, -0.64101F, -0.30917F),
                        localAngles = new Vector3(0F, 270F, 90.00002F),
                        localScale = new Vector3(1.3781F, 1.3781F, 1.3781F)
                    }
                }
            );
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.06207F, -0.00633F, -0.43981F),
                        localAngles = new Vector3(46.19479F, 317.7169F, 32.70124F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0.3056F, 0.52725F, 3.26058F),
                        localAngles = new Vector3(30.39775F, 45.42774F, 28.89316F),
                        localScale = new Vector3(6F, 6F, 6F)
                    }
                }
            );
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.11671F, 0.05229F, -0.30919F),
                        localAngles = new Vector3(38.99648F, 247.9024F, 39.96483F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Backpack",
                        localPos = new Vector3(0.4506F, -0.02184F, 0.03402F),
                        localAngles = new Vector3(320.4368F, 123.0031F, 223.0636F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.00667F, -0.01699F, -0.28335F),
                        localAngles = new Vector3(40.20431F, 346.6881F, 38.21197F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlSeeker", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Pack",
                        localPos = new Vector3(0.16809F, 0.06523F, -0.19531F),
                        localAngles = new Vector3(57.44382F, 161.5099F, 337.5476F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlFalseSon", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "ClubExplosionPoint",
                        localPos = new Vector3(-0.03983F, 0.03561F, -0.34742F),
                        localAngles = new Vector3(322.6903F, 219.889F, 344.3528F),
                        localScale = new Vector3(1.5F, 1.5F, 1.5F)
                    }
                }
            );
            rules.Add("mdlChef", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Chest",
                        localPos = new Vector3(-0.01263F, -0.46406F, -0.22779F),
                        localAngles = new Vector3(24.81427F, 44.66003F, 158.656F),
                        localScale = new Vector3(1.19805F, 1.19805F, 1.19805F)
                    }
                }
            );
            rules.Add("mdlDroneTech", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "Head",
                        localPos = new Vector3(-0.21005F, 0.01938F, 0.02086F),
                        localAngles = new Vector3(310.5165F, 334.7779F, 69.79485F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            rules.Add("mdlDrifter", new RoR2.ItemDisplayRule[]{new RoR2.ItemDisplayRule{
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplayPrefab,
                        childName = "BagFrontPocket",
                        localPos = new Vector3(0F, 0.45212F, 0F),
                        localAngles = new Vector3(322.6133F, 352.8321F, 330.8905F),
                        localScale = new Vector3(1F, 1F, 1F)
                    }
                }
            );
            #endregion

            return rules;
        }

        public class MicrowaveLightningOrb : LightningOrb
        {
            public override void OnArrival()
            {
                if (!target)
                {
                    return;
                }
                HealthComponent healthComponent = target.healthComponent;
                if ((bool)healthComponent)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = damageValue;
                    damageInfo.attacker = attacker;
                    damageInfo.inflictor = inflictor;
                    damageInfo.force = Vector3.zero;
                    damageInfo.crit = isCrit;
                    damageInfo.procChainMask = procChainMask;
                    damageInfo.procCoefficient = procCoefficient;
                    damageInfo.position = target.transform.position;
                    damageInfo.damageColorIndex = damageColorIndex;
                    damageInfo.damageType = damageType;
                    if (isElectric)
                    {
                        damageInfo.damageType.damageTypeExtended = DamageTypeExtended.Electrical;
                    }
                    damageInfo.inflictedHurtbox = target;
                    healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);

                    if (attacker.TryGetComponent(out CharacterBody body) && body.healthComponent)
                    {
                        body.healthComponent.RechargeShield(Microwave_ShieldRegen.Value);
                    }

                    target.healthComponent.gameObject.TryGetComponent(out SetStateOnHurt setStateOnHurt);
                    if (setStateOnHurt)
                    {
                        setStateOnHurt.SetStun(1f);
                    }
                }
            }
        }

        public class MicrowaveAkBankHolder : MonoBehaviour
        {
            public AkBank akBank;

            public void Awake()
            {
                akBank = gameObject.AddComponent<AkBank>();
                AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.ChefBody_prefab)).Completed += x =>
                {
                    AkBank _bank = x.Result.GetComponent<AkBank>();
                    if (_bank)
                    {
                        akBank.data.WwiseObjectReference = _bank.data.WwiseObjectReference;
                    }
                };
            }
        }

        public class MicrowaveBehavior : MonoBehaviour
        {
            AkBank bank;
            CharacterBody characterBody;
            ModelLocator modelLocator;
            public float interval = Microwave_Interval.Value;
            public int shocksLeft = Microwave_ShockCount.Value;
            public int shockOrbCount = Microwave_OrbCount.Value;
            public float shockDamage = Microwave_ShockDamage.Value;
            public float shockRadius = Microwave_Radius.Value;
            public float procCoefficient = Microwave_ProcCoefficient.Value;
            public bool speedBoosting = true;
            private float intervalTimer = Microwave_Interval.Value - 0.8f;
            private float subShockTimer = Microwave_Interval.Value - 0.8f;

            public void Awake()
            {
                characterBody = gameObject.GetComponent<CharacterBody>();
                modelLocator = gameObject.GetComponent<ModelLocator>();

                if (!characterBody || !modelLocator)
                {
                    Destroy(this);
                }

                characterBody.RecalculateStats();

                TemporaryOverlay temporaryOverlay = gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = interval * shocksLeft;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.2f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = EquipInit.Microwave.ZapOverlay;
                temporaryOverlay.AddToCharacerModel(modelLocator.modelTransform.GetComponentInParent<CharacterModel>());
            }

            public void Start()
            {
                Util.PlaySound("Play_chef_skill4_boost_activate", gameObject);
            }

            public void FixedUpdate()
            {
                intervalTimer += Time.fixedDeltaTime;
                subShockTimer += Time.fixedDeltaTime;

                if (subShockTimer >= interval / 2f)
                {
                    Util.PlaySound("Play_item_use_BFG_zaps", gameObject);
                    subShockTimer = 0f;
                }

                if (intervalTimer >= interval)
                {
                    int orbsLeft = shockOrbCount;

                    List<Collider> colliders = Physics.OverlapSphere(characterBody.corePosition, shockRadius).ToList();
                    Util.ShuffleList(colliders);
                    foreach (Collider collider in colliders)
                    {
                        GameObject gameObject = collider.gameObject;
                        if (gameObject.GetComponentInChildren<CharacterBody>())
                        {
                            CharacterBody colliderBody = gameObject.GetComponentInChildren<CharacterBody>();
                            if (colliderBody.healthComponent && colliderBody.teamComponent && colliderBody.teamComponent.teamIndex != characterBody.teamComponent.teamIndex)
                            {
                                MicrowaveLightningOrb microwaveLightningOrb = new MicrowaveLightningOrb();
                                microwaveLightningOrb.lightningType = LightningOrb.LightningType.Tesla;
                                microwaveLightningOrb.duration = 0.5f;
                                microwaveLightningOrb.damageValue = characterBody.damage * shockDamage;
                                microwaveLightningOrb.attacker = this.gameObject;
                                microwaveLightningOrb.inflictor = this.gameObject;
                                microwaveLightningOrb.bouncesRemaining = 0;
                                microwaveLightningOrb.teamIndex = characterBody.teamComponent.teamIndex;
                                microwaveLightningOrb.isCrit = characterBody.RollCrit();
                                microwaveLightningOrb.range = shockRadius;
                                microwaveLightningOrb.procCoefficient = procCoefficient;
                                microwaveLightningOrb.target = colliderBody.mainHurtBox;
                                microwaveLightningOrb.origin = characterBody.corePosition;
                                OrbManager.instance.AddOrb(microwaveLightningOrb);

                                orbsLeft--;
                                if (orbsLeft <= 0)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    intervalTimer = 0f;

                    EffectData effectData = new EffectData()
                    {
                        origin = characterBody.corePosition,
                        scale = 2f
                    };
                    EffectManager.SpawnEffect(EquipInit.Microwave.ZapEffect, effectData, true);

                    EffectData effectData2 = new EffectData()
                    {
                        origin = characterBody.corePosition,
                        scale = 0.1f
                    };
                    EffectManager.SpawnEffect(EquipInit.Microwave.ZapEffect2, effectData2, true);

                    shocksLeft--;
                    if (shocksLeft <= 0)
                    {
                        speedBoosting = false;
                        characterBody.RecalculateStats();
                        Destroy(bank);
                        Destroy(this);
                    }
                }
            }
        }
    }
}