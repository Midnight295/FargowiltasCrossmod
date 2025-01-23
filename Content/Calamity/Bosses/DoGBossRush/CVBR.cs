﻿using CalamityMod;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ProvidenceBossRush
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CVBR : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<CalamityMod.NPCs.CeaselessVoid.CeaselessVoid>();
        }
        public int attackTimer = 0;
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(attackTimer);
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            attackTimer = binaryReader.Read7BitEncodedInt();
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        
        public override bool PreAI(NPC npc)
        {
            if (npc.HasValidTarget) {
                if (npc.GetLifePercent() > 0.3f)
                {
                    npc.life = (int)(npc.lifeMax * 0.3f);
                }
            }
            return base.PreAI(npc);
        }
    }
}
