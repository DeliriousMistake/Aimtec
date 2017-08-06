using System.Collections.Generic;
using Aimtec.SDK.Prediction.Collision;
using Aimtec.SDK.Util.Cache;

/*
    * don't brag abt money when ur mom flies economy 
    * don't brag abt money when ur mom flies economy 
    * don't brag abt money when ur mom flies economy 
    * don't brag abt money when ur mom flies economy 
    * don't brag abt money when ur mom flies economy 
    * don't brag abt money when ur mom flies economy 
 */

namespace Jinx
{
    using System;
    using Aimtec.SDK.Damage.JSON;

    using System.Drawing;
    using System.Linq;

    using Aimtec;

    using Aimtec.SDK;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction;
    using Aimtec.SDK.Prediction.Skillshots;

    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.TargetSelector;
    using Aimtec.SDK.Util;
    using Aimtec.SDK.Prediction.Health;
    using Spell = Aimtec.SDK.Spell;


    internal class Jinx
    {
        public static Menu Menu = new Menu("Jinx", "Simple Jinx", true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();
        public static HealthPrediction HealthPrediction = new HealthPrediction();

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell R;

        public Jinx()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1500);
            R = new Spell(SpellSlot.R, 3000);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.Line);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.Line);

            Console.WriteLine("Jinx pred");
            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo Options");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuSlider("switchq", "Q Splashrange enemies", 3, 2, 5, true));
                ComboMenu.Add(new MenuBool("usew", "Use W"));
                ComboMenu.Add(new MenuBool("user", "Use R", false));
            }

            Menu.Add(ComboMenu);

            Menu.Attach();
            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate()
        {
            switch (Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    OnCombo();
                    break;
            }

            OnKillSteal();
        }

        private float BonusRange => 670f + Player.BoundingRadius + 25 * Player.SpellBook.GetSpell(SpellSlot.Q).Level;
        private static Obj_AI_Hero _target;

        private void OnKillSteal()
        {
            var spellsReady = new List<Spell>(new[] { R }.Where(x => x.Ready && Menu["combo"]["user"].Enabled));

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                var spell = spellsReady
                    .FirstOrDefault(x => Player.GetSpellDamage(enemy, SpellSlot.R) > enemy.Health + enemy.PhysicalShield &&
                                enemy.IsValidTarget(R.Range));

                if (spell == null)
                {
                    continue;
                }

                if (enemy.IsValidTarget(Player.AttackRange + 200) || enemy.Health < Player.GetAutoAttackDamage(enemy))
                {
                    continue;
                }

                spell.Cast(enemy);
                return;
                
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(1100);
            if (target == null)
                return;

            if (Menu["combo"]["useq"].Enabled && Q.Ready)
            {
                var isUsingFishBones = Player.HasBuff("JinxQ");
                var powPowRange = 525f + Player.BoundingRadius;

                if (!isUsingFishBones)
                {
                    if (Player.Distance(target) > powPowRange)
                    {
                        Q.Cast();
                    }
                }
                else
                {
                    if (Player.Distance(target) < 525f + Player.BoundingRadius)
                    {
                        Q.Cast();
                    }
                }
            }

            if (Menu["combo"]["usew"].Enabled && W.Ready) //&& !target.IsUnderEnemyTurret()
            {
                // Don't W when target is killable with a basic attack or if target is in less than 500 range
                if (target.IsValidTarget(Player.AttackRange) &&
                    Player.GetAutoAttackDamage(target) > target.Health + target.PhysicalShield || target.Distance(Player) < 500)
                {
                    return;
                }

                if (target.IsValidTarget(W.Range) && target.Distance(Player) > BonusRange)
                {
                    W.Cast(target);
                }
            }
        }
    }
}
