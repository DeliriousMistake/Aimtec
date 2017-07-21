using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.TargetSelector;
using Aimtec.SDK.Damage;

namespace Twitch
{
    internal class Utility
    {
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        /// <summary>
        ///     Return true if unit is under ally turret range.
        /// </summary>
        ///     <returns></returns>
        public static bool UnderAllyTurret(Obj_AI_Base unit)
        {
            return UnderAllyTurret(unit.Position);
        }

        public static bool UnderAllyTurret(Vector3 position)
        {
            return
                ObjectManager.Get<Obj_AI_Turret>()
                    .Any(turret => turret.IsValidTarget(950, false, checkRangeFrom: position) && turret.IsAlly);
        }
        
        /// <summary>
        ///     Returns true if the unit is under tower range.
        /// </summary>
        public static bool UnderTurret(Obj_AI_Base unit)
        {
            return UnderTurret(unit.Position, true);
        }

        /// <summary>
        ///     Returns true if the unit is under turret range.
        /// </summary>
        public static bool UnderTurret(Obj_AI_Base unit, bool enemyTurretsOnly)
        {
            return UnderTurret(unit.Position, enemyTurretsOnly);
        }

        public static bool UnderTurret(Vector3 position, bool enemyTurretsOnly)
        {
            return
                ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.IsValidTarget(950, enemyTurretsOnly, checkRangeFrom: position));
        }

        public static double CalculateEDamage(Obj_AI_Hero target)
        {
            var countBuffs = target.BuffManager.GetBuffCount("TwitchDeadlyVenom");
            return (countBuffs * new double[] {15, 20, 25, 30, 35}[Player.SpellBook.GetSpell(SpellSlot.E).Level - 1]
                    + 0.2 * Player.TotalAbilityDamage
                    + 0.25 * Player.FlatPhysicalDamageMod) +
                   new double[] {20, 35, 50, 65, 80}[Player.SpellBook.GetSpell(SpellSlot.E).Level - 1];
        }

        public static float GetPoisonStacks(Obj_AI_Base target)
        {
            return target.BuffManager.GetBuffCount("TwitchDeadlyVenom");
        }

        public static float GetPoisonDamage(Obj_AI_Base target)
        {
            if (target == null || !target.HasBuff("TwitchDeadlyVenom") || target.IsInvulnerable
                || target.HasBuff("KindredRNoDeathBuff") || target.BuffManager.HasBuffOfType(BuffType.SpellShield))
            {
                return 0;
            }

            double baseDamage = CalculateEDamage(target as Obj_AI_Hero);

  
           /* if (Player.HasBuff("SummonerExhaust"))
                baseDamage *= 0.6;

      
            if (Player.HasBuff("urgotentropypassive"))
                baseDamage *= 0.85;

         
            var bondofstoneBuffCount = target.GetBuffCount("MasteryWardenOfTheDawn");
            if (bondofstoneBuffCount > 0)
                baseDamage *= 1 - 0.06 * bondofstoneBuffCount;

           
            var phantomdancerBuff = Player.BuffManager.GetBuff("itemphantomdancerdebuff");
            if (phantomdancerBuff != null && phantomdancerBuff.Caster == target)
                baseDamage *= 0.88;

            
            if (target.HasBuff("FerociousHowl"))
                baseDamage *= 0.6 - new[] { 0.1, 0.2, 0.3 }[target.SpellBook.GetSpell(SpellSlot.R).Level - 1];

            if (target.HasBuff("Tantrum"))
                baseDamage -= new[] { 2, 4, 6, 8, 10 }[target.SpellBook.GetSpell(SpellSlot.E).Level - 1];

            if (target.HasBuff("BraumShieldRaise"))
                baseDamage *= 1
                              - new[] { 0.3, 0.325, 0.35, 0.375, 0.4 }[target.SpellBook.GetSpell(SpellSlot.E).Level - 1];

            if (target.HasBuff("GalioIdolOfDurand"))
                baseDamage *= 0.5;

            if (target.HasBuff("GarenW"))
                baseDamage *= 0.7;

            if (target.HasBuff("GragasWSelf"))
                baseDamage *= 1 - new[] { 0.1, 0.12, 0.14, 0.16, 0.18 }[target.SpellBook.GetSpell(SpellSlot.W).Level - 1];

            
            if (target.HasBuff("KatarinaEReduction"))
                baseDamage *= 0.85;*/


            return (float)baseDamage;

        }

        public static float GetRealHealth(Obj_AI_Base target)
        {
            return target.Health + 250 + (target.PhysicalShield > 0 ? target.PhysicalShield : 0);
        }

        public static bool IsPoisonKillable(Obj_AI_Base target)
        {
            return GetPoisonDamage(target) >= GetRealHealth(target);
        }
    }
}
