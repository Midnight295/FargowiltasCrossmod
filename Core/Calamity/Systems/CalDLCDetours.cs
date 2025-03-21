﻿using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using CalamityMod.World;
using CalamityMod;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using CalamityMod.Items;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls;
using Luminance.Core.Hooking;
using Fargowiltas.NPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Content.Projectiles;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Ranged;
using Fargowiltas.Items;
using CalamityMod.Items.Potions;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using CalamityMod.CalPlayer;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Systems;
using CalamityMod.Enums;
using Fargowiltas.Items.Vanity;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.UI.Elements;
using Terraria.Localization;
using CalamityMod.Skies;
using Terraria.Graphics.Effects;
using FargowiltasSouls.Content.UI;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCDetours : ICustomDetourProvider
    {
        // AI override
        // GlobalNPC
        private static readonly MethodInfo CalamityPreAIMethod = typeof(CalamityGlobalNPC).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityOtherStatChangesMethod = typeof(CalamityGlobalNPC).GetMethod("OtherStatChanges", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityPreDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PreDraw", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityPostDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PostDraw", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityBossHeadSlotMethod = typeof(CalamityGlobalNPC).GetMethod("BossHeadSlot", LumUtils.UniversalBindingFlags);
        // NPCStats
        private static readonly MethodInfo CalamityGetNPCDamageMethod = typeof(NPCStats).GetMethod("GetNPCDamage", LumUtils.UniversalBindingFlags);
        // GlobalProjectile
        private static readonly MethodInfo CalamityProjectilePreAIMethod = typeof(CalamityGlobalProjectile).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityProjectileCanDamageMethod = typeof(CalamityGlobalProjectile).GetMethod("CanDamage", LumUtils.UniversalBindingFlags);

        // Misc compatibility, fixes and balance
        private static readonly MethodInfo FMSVerticalSpeedMethod = typeof(FlightMasteryWings).GetMethod("VerticalWingSpeeds", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo FMSHorizontalSpeedMethod = typeof(FlightMasteryWings).GetMethod("HorizontalWingSpeeds", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo LifeForceVerticalSpeedMethod = typeof(LifeForce).GetMethod("VerticalWingSpeeds", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo LifeForceHorizontalSpeedMethod = typeof(LifeForce).GetMethod("HorizontalWingSpeeds", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo IsFargoSoulsItemMethod = typeof(Squirrel).GetMethod("IsFargoSoulsItem", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo BrimstoneMonsterCanHitPlayerMethod = typeof(BrimstoneMonster).GetMethod("CanHitPlayer", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo FargoSoulsOnSpawnProjMethod = typeof(FargoSoulsGlobalProjectile).GetMethod("OnSpawn", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TryUnlimBuffMethod = typeof(Fargowiltas.Items.FargoGlobalItem).GetMethod("TryUnlimBuff", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo NatureChampAIMethod = typeof(NatureChampion).GetMethod("AI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TerraChampAIMethod = typeof(TerraChampion).GetMethod("AI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CheckTempleWallsMethod = typeof(Golem).GetMethod("CheckTempleWalls", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo DukeFishronPreAIMethod = typeof(DukeFishron).GetMethod("SafePreAI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TungstenIncreaseWeaponSizeMethod = typeof(TungstenEffect).GetMethod("TungstenIncreaseWeaponSize", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TungstenNerfedProjMetod = typeof(TungstenEffect).GetMethod("TungstenNerfedProj", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TungstenNeverAffectsProjMethod = typeof(TungstenEffect).GetMethod("TungstenNeverAffectsProj", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo ModifyHurtInfo_CalamityMethod = typeof(CalamityPlayer).GetMethod("ModifyHurtInfo_Calamity", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo MinimalEffects_Method = typeof(ToggleBackend).GetMethod("MinimalEffects", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo BRDialogueTick_Method = typeof(BossRushDialogueSystem).GetMethod("Tick", LumUtils.UniversalBindingFlags);
        //private static readonly MethodInfo BRSceneWeight_Method = typeof(BossRushScene).GetMethod("GetWeight", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo FargoPlayerPreKill_Method = typeof(FargoSoulsPlayer).GetMethod("PreKill", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CanToggleEternity_Method = typeof(Masochist).GetMethod("CanToggleEternity", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo SoulTogglerOnActivate_Method = typeof(SoulTogglerButton).GetMethod("OnActivate", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo GetAdrenalineDamage_Method = typeof(CalamityUtils).GetMethod("GetAdrenalineDamage", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo DetermineDrawEligibility_Method = typeof(BossRushSky).GetMethod("DetermineDrawEligibility", LumUtils.UniversalBindingFlags);

        // AI override
        // GlobalNPC
        public delegate bool Orig_CalamityPreAI(CalamityGlobalNPC self, NPC npc);
        public delegate void Orig_CalamityOtherStatChanges(CalamityGlobalNPC self, NPC npc);
        public delegate bool Orig_CalamityPreDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        public delegate void Orig_CalamityPostDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        public delegate void Orig_CalamityBossHeadSlot(CalamityGlobalNPC self, NPC npc, ref int index);
        // NPCStats
        public delegate void Orig_CalamityGetNPCDamage(NPC npc);
        // GlobalProjectile
        public delegate bool Orig_CalamityProjectilePreAI(CalamityGlobalProjectile self, Projectile projectile);
        public delegate bool? Orig_CalamityProjectileCanDamage(CalamityGlobalProjectile self, Projectile projectile);

        // Misc compatibility, fixes and balance
        public delegate void Orig_FMSVerticalSpeed(FlightMasteryWings self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend);
        public delegate void Orig_FMSHorizontalSpeed(FlightMasteryWings self, Player player, ref float speed, ref float acceleration);
        public delegate void Orig_LifeForceVerticalSpeed(LifeForce self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend);
        public delegate void Orig_LifeForceHorizontalSpeed(LifeForce self, Player player, ref float speed, ref float acceleration);
        public delegate bool Orig_IsFargoSoulsItem(Item item);
        public delegate bool Orig_BrimstoneMonsterCanHitPlayer(BrimstoneMonster self, Player player);
        public delegate void Orig_FargoSoulsOnSpawnProj(FargoSoulsGlobalProjectile self, Projectile projectile, IEntitySource source);
        public delegate void Orig_TryUnlimBuff( Item item, Player player);
        public delegate void Orig_NatureChampAI(NatureChampion self);
        public delegate void Orig_TerraChampAI(TerraChampion self);
        public delegate bool Orig_CheckTempleWalls(Vector2 pos);
        public delegate bool Orig_DukeFishronPreAI(DukeFishron self, NPC npc);
        public delegate float Orig_TungstenIncreaseWeaponSize(FargoSoulsPlayer modPlayer);
        public delegate bool Orig_TungstenNerfedProj(Projectile projectile);
        public delegate bool Orig_TungstenNeverAffectsProj(Projectile projectile);
        public delegate void Orig_ModifyHurtInfo_Calamity(CalamityPlayer self, ref Player.HurtInfo info);
        public delegate void Orig_MinimalEffects(ToggleBackend self);
        public delegate void Orig_BRDialogueTick();
        //public delegate void Orig_BRSceneWeight();

        public delegate bool Orig_FargoPlayerPreKill(FargoSoulsPlayer self, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
        public delegate bool Orig_CanToggleEternity();
        public delegate void Orig_SoulTogglerOnActivate(SoulTogglerButton self);
        public delegate float Orig_GetAdrenalineDamage(CalamityPlayer mp);
        public delegate bool Orig_DetermineDrawEligibility();

        void ICustomDetourProvider.ModifyMethods()
        {
            // AI override
            // GlobalNPC
            HookHelper.ModifyMethodWithDetour(CalamityPreAIMethod, CalamityPreAI_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityOtherStatChangesMethod, CalamityOtherStatChanges_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityPreDrawMethod, CalamityPreDraw_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityPostDrawMethod, CalamityPostDraw_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityBossHeadSlotMethod, CalamityBossHeadSlot_Detour);
            // NPCStats
            HookHelper.ModifyMethodWithDetour(CalamityGetNPCDamageMethod, CalamityGetNPCDamage_Detour);
            // GlobalProjectile
            HookHelper.ModifyMethodWithDetour(CalamityProjectilePreAIMethod, CalamityProjectilePreAI_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityProjectileCanDamageMethod, CalamityProjectileCanDamage_Detour);

            // Misc compatibility, fixes and balance
            HookHelper.ModifyMethodWithDetour(FMSVerticalSpeedMethod, FMSVerticalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(FMSHorizontalSpeedMethod, FMSHorizontalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(LifeForceVerticalSpeedMethod, LifeForceVerticalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(LifeForceHorizontalSpeedMethod, LifeForceHorizontalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(IsFargoSoulsItemMethod, IsFargoSoulsItem_Detour);
            HookHelper.ModifyMethodWithDetour(BrimstoneMonsterCanHitPlayerMethod, BrimstoneMonsterCanHitPlayer_Detour);
            HookHelper.ModifyMethodWithDetour(FargoSoulsOnSpawnProjMethod, FargoSoulsOnSpawnProj_Detour);
            HookHelper.ModifyMethodWithDetour(TryUnlimBuffMethod, TryUnlimBuff_Detour);
            HookHelper.ModifyMethodWithDetour(NatureChampAIMethod, NatureChampAI_Detour);
            HookHelper.ModifyMethodWithDetour(TerraChampAIMethod, TerraChampAI_Detour);
            HookHelper.ModifyMethodWithDetour(CheckTempleWallsMethod, CheckTempleWalls_Detour);
            HookHelper.ModifyMethodWithDetour(DukeFishronPreAIMethod, DukeFishronPreAI_Detour);
            HookHelper.ModifyMethodWithDetour(TungstenIncreaseWeaponSizeMethod, TungstenIncreaseWeaponSize_Detour);
            HookHelper.ModifyMethodWithDetour(TungstenNerfedProjMetod, TungstenNerfedProj_Detour);
            HookHelper.ModifyMethodWithDetour(TungstenNeverAffectsProjMethod, TungstenNeverAffectsProj_Detour);
            HookHelper.ModifyMethodWithDetour(ModifyHurtInfo_CalamityMethod, ModifyHurtInfo_Calamity_Detour);
            HookHelper.ModifyMethodWithDetour(MinimalEffects_Method, MinimalEffects_Detour);
            HookHelper.ModifyMethodWithDetour(BRDialogueTick_Method, DialogueReplacement);
            //HookHelper.ModifyMethodWithDetour(BRSceneWeight_Method, );
            HookHelper.ModifyMethodWithDetour(FargoPlayerPreKill_Method, FargoPlayerPreKill_Detour);
            HookHelper.ModifyMethodWithDetour(CanToggleEternity_Method, CanToggleEternity_Detour);
            HookHelper.ModifyMethodWithDetour(SoulTogglerOnActivate_Method, SoulTogglerOnActivate_Detour);
            HookHelper.ModifyMethodWithDetour(GetAdrenalineDamage_Method, GetAdrenalineDamage_Detour);
            HookHelper.ModifyMethodWithDetour(DetermineDrawEligibility_Method, DetermineDrawEligibility_Detour);
        }
        #region GlobalNPC
        internal static bool CalamityPreAI_Detour(Orig_CalamityPreAI orig, CalamityGlobalNPC self, NPC npc)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;

            int defDamage = npc.defDamage; // do not fuck with defDamage please

            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
                BossRushEvent.BossRushActive = false;
            }
            bool result = orig(self, npc);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                BossRushEvent.BossRushActive = wasBossRush;
                npc.defDamage = defDamage; // do not fuck with defDamage please
            }
            return result;
        }

        internal static void CalamityOtherStatChanges_Detour(Orig_CalamityOtherStatChanges orig, CalamityGlobalNPC self, NPC npc)
        {
            orig(self, npc);
            if (!CalDLCWorldSavingSystem.E_EternityRev)
                return;
            switch (npc.type)
            {
                case NPCID.DetonatingBubble:
                    if (NPC.AnyNPCs(NPCID.DukeFishron))
                        npc.dontTakeDamage = false;
                    break;
                default:
                    break;
            }
        }

        internal static bool CalamityPreDraw_Detour(Orig_CalamityPreDraw orig, CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisableNPC = CalamityLists.DestroyerIDs.Contains(npc.type) || npc.type == NPCID.SkeletronPrime;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev && shouldDisableNPC;

            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                BossRushEvent.BossRushActive = false;
            }
            bool result = orig(self, npc, spriteBatch, screenPos, drawColor);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                BossRushEvent.BossRushActive = wasBossRush;
            }
            return result;
        }

        private static readonly List<int> DisablePostDrawNPCS = new(CalamityLists.DestroyerIDs)
            {
            NPCID.WallofFleshEye,
            NPCID.Creeper,
            NPCID.SkeletronPrime
            };

        internal static void CalamityPostDraw_Detour(Orig_CalamityPostDraw orig, CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool shouldDisableNPC = DisablePostDrawNPCS.Contains(npc.type);
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev && shouldDisableNPC;
            if (shouldDisable)
            {
                return;
            }
            orig(self, npc, spriteBatch, screenPos, drawColor);
        }

        internal static void CalamityBossHeadSlot_Detour(Orig_CalamityBossHeadSlot orig, CalamityGlobalNPC self, NPC npc, ref int index)
        {
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            if (shouldDisable)
                return;
            orig(self, npc, ref index);
        }
        #endregion

        #region NPCStats
        internal static void CalamityGetNPCDamage_Detour(Orig_CalamityGetNPCDamage orig, NPC npc)
        {
            // Prevent vanilla bosses and their segments from having their damage overriden by Calamity
            bool countsAsBoss = npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
            if (npc.type < NPCID.Count && (countsAsBoss || CalamityLists.bossHPScaleList.Contains(npc.type)))
                return;
            orig(npc);
        }
        #endregion

        #region GlobalProjectile
        internal static bool CalamityProjectilePreAI_Detour(Orig_CalamityProjectilePreAI orig, CalamityGlobalProjectile self, Projectile projectile)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            int damage = projectile.damage;
            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
            }
            bool result = orig(self, projectile);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                projectile.damage = damage;
            }
            return result;
        }

        public static bool? CalamityProjectileCanDamage_Detour(Orig_CalamityProjectileCanDamage orig, CalamityGlobalProjectile self, Projectile projectile)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
                BossRushEvent.BossRushActive = false;
            }
            bool? result = orig(self, projectile);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                BossRushEvent.BossRushActive = wasBossRush;
            }
            return result;
        }
        #endregion

        #region Misc
        public static bool NonFargoBossAlive() => Main.npc.Any(n => n.Alive() && n.boss && n.ModNPC != null && n.ModNPC.Mod != ModCompatibility.SoulsMod.Mod);
        internal static void FMSVerticalSpeed_Detour(Orig_FMSVerticalSpeed orig, FlightMasteryWings self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            orig(self, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
            if (NonFargoBossAlive())
            {
                player.wingsLogic = ArmorIDs.Wing.LongTrailRainbowWings;
                if (!DownedBossSystem.downedYharon) // pre yharon, use Silva Wings stats
                {
                    if (ascentWhenFalling > 0.95f)
                        ascentWhenFalling = 0.95f;
                    if (ascentWhenRising > 0.16f)
                        ascentWhenRising = 0.16f;
                    if (maxCanAscendMultiplier > 1.1f)
                        maxCanAscendMultiplier = 1.1f;
                    if (maxAscentMultiplier > 3.2f)
                        maxAscentMultiplier = 3.2f;
                    if (constantAscend > 0.145f)
                        constantAscend = 0.145f;
                }
                else // post yharon, use Drew's Wings stats
                {
                    if (ascentWhenFalling > 1f)
                        ascentWhenFalling = 1f;
                    if (ascentWhenRising > 0.17f)
                        ascentWhenRising = 0.17f;
                    if (maxCanAscendMultiplier > 1.2f)
                        maxCanAscendMultiplier = 1.2f;
                    if (maxAscentMultiplier > 3.25f)
                        maxAscentMultiplier = 3.25f;
                    if (constantAscend > 0.15f)
                        constantAscend = 0.15f;
                }
            }
        }
        internal static void FMSHorizontalSpeed_Detour(Orig_FMSHorizontalSpeed orig, FlightMasteryWings self, Player player, ref float speed, ref float acceleration)
        {
            orig(self, player, ref speed, ref acceleration);
            if (NonFargoBossAlive())
            {
                if (!DownedBossSystem.downedYharon) // pre yharon, use Silva Wings stats
                {
                    if (speed > 10.5f)
                        speed = 10.5f;
                    if (acceleration > 2.8f)
                        acceleration = 2.8f;
                }
                else // post yharon, use Drew's Wings stats
                {
                    if (speed > 11.5f)
                        speed = 11.5f;
                    if (acceleration > 2.9f)
                        acceleration = 2.9f;
                }
                   
                //ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(361, 11.5f, 2.9f);
            }
        }

        internal static void LifeForceVerticalSpeed_Detour(Orig_LifeForceVerticalSpeed orig, LifeForce self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            orig(self, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
            if (NonFargoBossAlive())
            {
                ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(240, 9.5f, 2.7f);
                if (ascentWhenFalling > 0.85f)
                    ascentWhenFalling = 0.85f;
                if (ascentWhenRising > 0.15f)
                    ascentWhenRising = 0.15f;
                if (maxCanAscendMultiplier > 1f)
                    maxCanAscendMultiplier = 1f;
                if (maxAscentMultiplier > 3f)
                    maxAscentMultiplier = 3f;
                if (constantAscend > 0.135f)
                    constantAscend = 0.135f;
            }
            else
                ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new Terraria.DataStructures.WingStats(1000);
        }
        internal static void LifeForceHorizontalSpeed_Detour(Orig_LifeForceHorizontalSpeed orig, LifeForce self, Player player, ref float speed, ref float acceleration)
        {
            orig(self, player, ref speed, ref acceleration);
            if (NonFargoBossAlive())
            {
                
                //ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(361, 11.5f, 2.9f);
            }
        }

        internal static bool IsFargoSoulsItem_Detour(Orig_IsFargoSoulsItem orig, Item item)
        {
            bool result = orig(item);
            if (item.ModItem is not null && item.ModItem.Mod == FargowiltasCrossmod.Instance)
                return true;
            return result;
        }
        internal static bool BrimstoneMonsterCanHitPlayer_Detour(Orig_BrimstoneMonsterCanHitPlayer orig, BrimstoneMonster self, Player player)
        {
            if (self.Type != ModContent.ProjectileType<BrimstoneMonster>())
            {
                return orig(self, player);
            }
            float distSQ = self.Projectile.DistanceSQ(player.Center);
            float radiusSQ = MathF.Pow(170f * self.Projectile.scale, 2);
            if (distSQ > radiusSQ)
                return false;
            return orig(self, player);
        }
        internal static void FargoSoulsOnSpawnProj_Detour(Orig_FargoSoulsOnSpawnProj orig, FargoSoulsGlobalProjectile self, Projectile proj, IEntitySource source)
        {
            if (proj.type == ModContent.ProjectileType<TitaniumRailgunScope>())
            {
                proj.FargoSouls().CanSplit = false;
            }
            
            orig(self, proj, source);
        }
        internal static void TryUnlimBuff_Detour(Orig_TryUnlimBuff orig, Item item, Player player)
        {
            if (item.type != ModContent.ItemType<AstralInjection>())
            {
                orig(item, player);

            }
            
        }
        internal static void NatureChampAI_Detour(Orig_NatureChampAI orig, NatureChampion self)
        {
            NPC npc = self.NPC;
            double originalSurface = Main.worldSurface;
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = 0;
            }
            orig(self);
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = originalSurface;
            }
        }
        internal static void TerraChampAI_Detour(Orig_TerraChampAI orig, TerraChampion self)
        {
            NPC npc = self.NPC;
            double originalSurface = Main.worldSurface;
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = 0;
            }
            orig(self);
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = originalSurface;
            }
        }
        internal static bool CheckTempleWalls_Detour(Orig_CheckTempleWalls orig, Vector2 pos)
        {
            
            if (BossRushEvent.BossRushActive)
            {
                return true;
            }
            return orig(pos);
        }
        internal static bool DukeFishronPreAI_Detour(Orig_DukeFishronPreAI orig, DukeFishron self, NPC npc)
        {
            if (BossRushEvent.BossRushActive && npc.HasValidTarget)
            {
                Main.player[npc.target].ZoneBeach = true;
            }
            bool result = orig(self, npc);
            if (BossRushEvent.BossRushActive && npc.HasValidTarget)
            {
                Main.player[npc.target].ZoneBeach = false;
            }
            return result;
        }
        internal static float TungstenIncreaseWeaponSize_Detour(Orig_TungstenIncreaseWeaponSize orig, FargoSoulsPlayer modPlayer)
        {
            float value = orig(modPlayer);
            if (modPlayer.Player.HeldItem == null)
                return value;
            if (CalDLCSets.Items.TungstenExclude[modPlayer.Player.HeldItem.type])
                return 1f;
            //if (modPlayer.Player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee))
            //    value -= (value - 1f) * 0.5f;
            return value;
        }
        internal static bool TungstenNerfedProj_Detour(Orig_TungstenNerfedProj orig, Projectile projectile)
        {
            bool value = orig(projectile); 
            /*
            if (!projectile.owner.IsWithinBounds(Main.maxPlayers))
                return value;
            Player player = Main.player[projectile.owner];
            if (!player.Alive())
                return value;
            if (player.HeldItem != null && player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee))
            {
                return true;
            }
            */
            return value;
        }
        internal static bool TungstenNeverAffectsProj_Detour(Orig_TungstenNeverAffectsProj orig, Projectile projectile)
        {
            bool value = orig(projectile);
            if (CalDLCSets.Projectiles.TungstenExclude[projectile.type])
                return true;
            return value;
        }
        internal static void ModifyHurtInfo_Calamity_Detour(Orig_ModifyHurtInfo_Calamity orig, CalamityPlayer self, ref Player.HurtInfo info)
        {
            bool chalice = self.chaliceOfTheBloodGod;
            if (self.Player.FargoSouls().GuardRaised || self.Player.FargoSouls().MutantPresence)
                self.chaliceOfTheBloodGod = false;
            orig(self, ref info);
            self.chaliceOfTheBloodGod = chalice;
        }
        internal static void MinimalEffects_Detour(Orig_MinimalEffects orig, ToggleBackend self)
        {
            orig(self);
            Player player = Main.LocalPlayer;
            player.SetToggleValue<OccultSkullCrownEffect>(true);
            player.SetToggleValue<PurityEffect>(true);
            player.SetToggleValue<TheSpongeEffect>(true);
            player.SetToggleValue<ChaliceOfTheBloodGodEffect>(true);
            player.SetToggleValue<YharimsGiftEffect>(true);
            player.SetToggleValue<DraedonsHeartEffect>(true);
            player.SetToggleValue<NebulousCoreEffect>(false);
            player.SetToggleValue<CalamityEffect>(true);

            player.SetToggleValue<AerospecJumpEffect>(true);

            player.SetToggleValue<NanotechEffect>(true);
            player.SetToggleValue<EclipseMirrorEffect>(true);
            player.SetToggleValue<AbyssalDivingSuitEffect>(true);
            player.SetToggleValue<NucleogenesisEffect>(true);
            player.SetToggleValue<ElementalQuiverEffect>(true);
            player.SetToggleValue<ElementalGauntletEffect>(true);
            player.SetToggleValue<EtherealTalismanEffect>(true);
            player.SetToggleValue<AmalgamEffect>(true);
            player.SetToggleValue<AsgardianAegisEffect>(true);
            player.SetToggleValue<RampartofDeitiesEffect>(true);

        }

        public static void DialogueReplacement(Orig_BRDialogueTick orig)
        {
            
            BossRushDialoguePhase phase = BossRushDialogueSystem.Phase;
            FieldInfo tierInfo = typeof(BossRushEvent).GetField("CurrentTier");
            if (tierInfo != null)
            {
                tierInfo.SetValue(tierInfo, 1);
            }
            else
            {
                //Main.NewText(BossRushEvent.BossRushStage);
            }
            //BossRushEvent.BossRushStage = 16;
            //DownedBossSystem.startedBossRushAtLeastOnce = true;
            //Main.NewText(BossRushEvent.Bosses[BossRushEvent.Bosses.Count - 1].EntityID);
            //Main.NewText(ModContent.NPCType<MutantBoss>());
            if (BossRushDialogueSystem.CurrentDialogueDelay > 0 && phase == BossRushDialoguePhase.Start)
            {
                BossRushDialogueSystem.CurrentDialogueDelay -= 5;
                if (BossRushDialogueSystem.CurrentDialogueDelay < 0)
                {
                    BossRushDialogueSystem.CurrentDialogueDelay = 0;
                }
            }
            if (!BossRushEvent.BossRushActive || BossRushDialogueSystem.Phase == BossRushDialoguePhase.Start || BossRushDialogueSystem.Phase == BossRushDialoguePhase.None)
            {
                
                orig();
                return;
            }
            int currSequenceLength = 0;
            int currLine = BossRushDialogueSystem.currentSequenceIndex;
            
            if (phase == BossRushDialoguePhase.StartRepeat)
            {
                currSequenceLength = 1;
            }
            if (phase == BossRushDialoguePhase.TierOneComplete)
            {
                currSequenceLength = 3;
            }
            
            if (BossRushDialogueSystem.CurrentDialogueDelay == 0)
            {
                if (phase == BossRushDialoguePhase.StartRepeat && currLine == 0)
                {
                    Main.NewText("Let's get started.", Color.Teal);
                    BossRushEvent.BossRushStage = 1;
                }
                if (phase == BossRushDialoguePhase.TierOneComplete)
                {
                    if (currLine == 0)
                        Main.NewText("This is boring.", Color.Teal);
                    //if (currLine == 1)
                        
                    if (currLine == 2)
                        Main.NewText("Let's cut to the chase.", Color.Teal);
                }
                BossRushDialogueSystem.CurrentDialogueDelay = 60;
                BossRushDialogueSystem.currentSequenceIndex += 1;
                
            }
            
            else
            {
                --BossRushDialogueSystem.CurrentDialogueDelay;
            }
            if (phase == BossRushDialoguePhase.End || phase == BossRushDialoguePhase.EndRepeat)
            {
                BossRushDialogueSystem.CurrentDialogueDelay = 0;
            }
            if (phase == BossRushDialoguePhase.TierOneComplete && currLine < 6)
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 0, 0.05f);
            }
            if ( phase == BossRushDialoguePhase.TierOneComplete && currLine > 6
                )
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 1, 0.001f);
            }
            if (BossRushEvent.BossRushSpawnCountdown < 180 && currLine < currSequenceLength) 
                BossRushEvent.BossRushSpawnCountdown = BossRushDialogueSystem.CurrentDialogueDelay + 180;
        }
        internal static bool FargoPlayerPreKill_Detour(Orig_FargoPlayerPreKill orig, FargoSoulsPlayer self, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool retval = orig(self, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
            if (!retval)
            {
                CalamityPlayer calPlayer = self.Player.Calamity();
                calPlayer.chaliceBleedoutBuffer = 0D;
                calPlayer.chaliceDamagePointPartialProgress = 0D;
                calPlayer.chaliceHitOriginalDamage = 0;
            }
            return retval;
        }

        internal static bool CanToggleEternity_Detour(Orig_CanToggleEternity orig)
        {
            orig();
            return false;
        }

        internal static void SoulTogglerOnActivate_Detour(Orig_SoulTogglerOnActivate orig, SoulTogglerButton self)
        {
            orig(self);
            self.OncomingMutant.TextHoldShift = $"{Language.GetTextValue("Mods.FargowiltasCrossmod.UI.ToggledWithCal")}]\n[c/787878:{self.OncomingMutant.TextHoldShift}";
        }

        internal static float GetAdrenalineDamage_Detour(Orig_GetAdrenalineDamage orig, CalamityPlayer mp)
        {
            float value = orig(mp);
            if (WorldSavingSystem.EternityMode)
                value = value * 0.5f + 0.5f;
            return value;
        }

        internal static bool DetermineDrawEligibility_Detour(Orig_DetermineDrawEligibility orig)
        {
            if (SkyManager.Instance["CalamityMod:BossRush"] != null && SkyManager.Instance["CalamityMod:BossRush"].IsActive())
                SkyManager.Instance.Deactivate("CalamityMod:BossRush", new object[0]);
            if (Filters.Scene["CalamityMod:BossRush"].IsActive())
                Filters.Scene["CalamityMod:BossRush"].Deactivate(new object[0]);
            /*
            if (useEffect != Filters.Scene["CalamityMod:BossRush"].IsActive())
            {
                if (useEffect)
                    Filters.Scene.Activate("CalamityMod:BossRush");
                else
                    Filters.Scene["CalamityMod:BossRush"].Deactivate(new object[0]);
            }
            */

            return false;
        }
        #endregion
    }
}