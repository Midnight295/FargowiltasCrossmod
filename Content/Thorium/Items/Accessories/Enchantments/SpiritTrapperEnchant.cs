﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod("ThoriumMod")]
    public class SpiritTrapperEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Blue;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.SpiritTrapperEnch = true;
            DLCPlayer.SpiritTrapperEnchItem = Item;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.AngryGhost>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.AngryGhost>(), 0, 0, player.whoAmI);
            }
            
        }
    }
}