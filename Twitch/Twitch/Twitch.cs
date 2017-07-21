using System;
using System.Collections.Generic;
using Aimtec.SDK.Damage.JSON;

namespace Twitch
{
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


    // All credits to iJava
    internal class Twitch
    {
        public static Menu Menu = new Menu("Twitch", "Twitch", true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();
        public static HealthPrediction HealthPrediction = new HealthPrediction();

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        public Twitch()
        {

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            W.SetSkillshot(0.25f, 120f, 1440f, false, SkillshotType.Circle);

            Orbwalker.Attach(Menu);

            var ComboMenu = new Menu("combo", "Combo Options");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuBool("usee", "Use E Killable"));
                ComboMenu.Add(new MenuBool("usew", "Use W"));
                //ComboMenu.Add(new MenuBool("user", "Use R", false));
            }

            Menu.Add(ComboMenu);

            var MiscMenu = new Menu("misc", "Misc Options");
            {
                MiscMenu.Add(new MenuBool("savemanae", "Save Mana for E"));
                MiscMenu.Add(new MenuKeyBind("stealthrecall", "Stealth Recall", KeyCode.T, KeybindType.Press));
                MiscMenu.Add(new MenuBool("ebeforedeath", "E Before Death"));
                MiscMenu.Add(new MenuBool("wundertower", "Don't W Under Tower"));
                MiscMenu.Add(new MenuSliderBool("nowaa", "No W if x aa can kill", true, 2, 0, 10));
            }

            Menu.Add(MiscMenu);

            Menu.Attach();

            SpellBook.OnCastSpell += SpellBook_OnCastSpell;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.PostAttack += Orbwalker_PostAttack;
        }

        private void SpellBook_OnCastSpell(Obj_AI_Base sender, SpellBookCastSpellEventArgs e)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (e.Slot ==  SpellSlot.Recall && Player.SpellBook.GetSpellState(SpellSlot.Q) == SpellState.Ready &&
                Menu["misc"]["stealthrecall"].Enabled)
            {
                Player.SpellBook.CastSpell(SpellSlot.Q);
                DelayAction.Queue((int) Player.SpellBook.GetSpell(SpellSlot.Q).SpellData.SpellCastTime + 300,
                    () => Player.SpellBook.CastSpell(SpellSlot.Recall));
            }
        }

        private void Orbwalker_PostAttack(object sender, PostAttackEventArgs e)
        {
            var target = e.Target;
            if (target == null || !target.IsValidTarget(950f) || target.IsInvulnerable)
            {
                return;
            }

            if (Orbwalker.Mode == OrbwalkingMode.Combo)
            {
                if (!Menu["combo"]["usew"].Enabled || Player.SpellBook.GetSpellState(SpellSlot.W) != SpellState.Ready)
                    return;

                if (Menu["misc"]["savemanae"].Enabled && Player.Mana <= 50 + 70)
                    return;

                if (Menu["misc"]["wundertower"].Enabled && Utility.UnderTurret(Player, true))
                    return;

                if (target.Health < Player.GetAutoAttackDamage(target as Obj_AI_Hero) * Menu["misc"]["nowaa"].Value &&
                    Menu["misc"]["nowaa"].Enabled)
                    return;

               if (target.IsValidTarget(W.Range)) //&& !Player.HasBuff("TwitchHideInShadows")
               {
                   W.Cast(target.Position);
                }
            }
        }

        public static bool HasBuff(Obj_AI_Base from, string buffname)
        {
            return from.BuffManager.HasBuff(buffname);
        }

        private void Game_OnUpdate()
        {
            if (MenuGUI.IsChatOpen() || Player.IsDead)
            {
                return;
            }

            if (Player.IsDead)
                return;

            if (Menu["misc"]["stealthrecall"].Enabled)
                Player.SpellBook.CastSpell(SpellSlot.Recall);

            if (Menu["misc"]["ebeforedeath"].Enabled &&
                Player.SpellBook.GetSpellState(SpellSlot.E) == SpellState.Ready &&
                HealthPrediction.GetPrediction(ObjectManager.GetLocalPlayer(), (int) (Game.ClockTime + 1000.0)) <=
                50.0f)
                E.Cast();

            if (Menu["combo"]["usee"].Enabled && Player.SpellBook.GetSpellState(SpellSlot.E) == SpellState.Ready)
            {
                var target = TargetSelector.GetTarget(1100);
                if (target == null)
                    return;

                double baseDamage = Utility.CalculateEDamage(target);

                if (ObjectManager.Get<Obj_AI_Hero>().Any(x => x.IsValidTarget(1100) && baseDamage > x.Health))
                {
                    E.Cast();
                }
            }
         
            switch (Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                OnCombo();
                break;
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(1100);
            if (target == null)
                return;

            if (!Menu["combo"]["useq"].Enabled || Player.SpellBook.GetSpellState(SpellSlot.Q) != SpellState.Ready)
                return;

            if (Menu["misc"]["savemanae"].Enabled && Player.Mana >= 110)
                Q.Cast();
        }
    }
}
