namespace ElTristana
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.TargetSelector;
    using Aimtec.SDK.Util.Cache;

    using Spell = Aimtec.SDK.Spell;

    internal class Tristana 
    {
        private static readonly Dictionary<string, SpellSlot> DangerousSpells = new Dictionary<string, SpellSlot>
        {
            { "darius", SpellSlot.R },
            { "fiddlesticks", SpellSlot.R },
            { "skarner", SpellSlot.R },
            { "warwick", SpellSlot.R },
            { "zed", SpellSlot.R },
        };

        public static Menu Menu = new Menu("ElTristana", "ElTristana", true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();
        public string TristanaE = "TristanaECharge";

        public static Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 550) },
            { SpellSlot.W, new Spell(SpellSlot.W, 900) },
            { SpellSlot.E, new Spell(SpellSlot.E, 625) },
            { SpellSlot.R, new Spell(SpellSlot.R, 700) },
        };

        public Tristana()
        {
            Orbwalker.Attach(Menu);
            var ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Use Q"));
                ComboMenu.Add(new MenuBool("usee", "Use E "));
                ComboMenu.Add(new MenuBool("user", "Use R to killsteal"));
                ComboMenu.Add(new MenuBool("useqone", "Only Q if target has E"));
                ComboMenu.Add(new MenuBool("focuse", "Focus E target"));
                ComboMenu.Add(new MenuBool("finisher", "Use E + R finisher", false));
                ComboMenu.Add(new MenuSlider("ecombomana", "Minimum mana for E", 25));

                ComboMenu.Add(new MenuSeperator("sep1"));

                foreach (Obj_AI_Hero enemies in GameObjects.EnemyHeroes)
                    ComboMenu.Add(new MenuBool("useeon" + enemies.ChampionName.ToLower(), enemies.ChampionName));
            }

            Menu.Add(ComboMenu);
            Menu.Attach();

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.PreAttack += Orbwalker_PreAttack;
            Console.WriteLine("ElTristana loaded and ready for use!");
        }

        private void Render_OnPresent()
        {
            if (Player.IsDead)
            {
                return;
            }

            Obj_AI_Hero target =
                GameObjects.EnemyHeroes.FirstOrDefault(
                    e => e.HasBuff(TristanaE) && e.IsValidTarget(Player.AttackRange));

            if (target == null || !target.IsValid)
            {
                return;
            }

            float x = target.FloatingHealthBarPosition.X + 45;
            float y = target.FloatingHealthBarPosition.Y - 25;

            int stacks = target.BuffManager.GetBuffCount(TristanaE, true);
            if (stacks > -1)
            {
                for (int i = 0; 4 > i; i++)
                {
                    Render.Line(x + i * 20, y, x + i * 20 + 10, y, 10, false, i > stacks ? Color.DarkGray : Color.OrangeRed);
                }
            }
        }

        private void Orbwalker_PreAttack(object sender, PreAttackEventArgs e)
        {
            if (!Orbwalker.Mode.Equals(OrbwalkingMode.Combo) || !Orbwalker.Mode.Equals(OrbwalkingMode.Mixed))
            {
                return;
            }

            if (!Menu["combo"]["focuse"].Enabled)
            {
                return;
            }

            Obj_AI_Hero hero = e.Target as Obj_AI_Hero;
            if (hero == null || !hero.IsValid || !hero.IsEnemy)
            {
                return;
            }

            Obj_AI_Hero target =
                GameObjects.EnemyHeroes.FirstOrDefault(
                    t => t.HasBuff(TristanaE) && t.IsValidTarget(Player.AttackRange));

            if (target == null)
            {
                return;
            }

            Orbwalker.ForceTarget(hero);
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs e)
        {
            if (!spells[SpellSlot.R].Ready)
            {
                return;
            }

            Obj_AI_Hero hero = sender as Obj_AI_Hero;
            if (hero == null || !hero.IsValid || !hero.IsEnemy)
            {
                return;
            }

            SpellSlot spell;
            if (DangerousSpells.TryGetValue(hero.ChampionName.ToLower(), out spell) && e.SpellSlot.Equals(spell))
            {
                if (!hero.IsValidTarget(spells[SpellSlot.R].Range))
                {
                    return;
                }

                spells[SpellSlot.R].Cast(hero);
            }
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            switch (Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    OnCombo();
                    break;
            }

            Killsteal();

            // Fix levels
            spells[SpellSlot.Q].Range = 550 + 9 * (Player.Level - 1);
            spells[SpellSlot.E].Range = 625 + 9 * (Player.Level - 1);
            spells[SpellSlot.R].Range = 517 + 9 * (Player.Level - 1);
        }

        private void Killsteal()
        {
            var spellsReady = new List<Spell>(new[] { spells[SpellSlot.R] }.Where(x => x.Ready && Menu["combo"]["user"].Enabled));

            foreach (Obj_AI_Hero enemy in GameObjects.EnemyHeroes)
            {
                var spell = spellsReady
                    .FirstOrDefault(x => Player.GetSpellDamage(enemy, SpellSlot.R) > enemy.Health + enemy.PhysicalShield + 50 &&
                                         enemy.IsValidTarget(spells[SpellSlot.R].Range));

                if (spell == null)
                {
                    continue;
                }

                spell.Cast(enemy);
                return;
            }
        }

        private void OnCombo()
        {
            Obj_AI_Hero target =
                GameObjects.EnemyHeroes.FirstOrDefault(
                    e => e.HasBuff(TristanaE) && e.IsValidTarget(Player.AttackRange)) ??
                TargetSelector.GetTarget(Player.AttackRange);

            if (target == null || !target.IsValid)
            {
                return;
            }

            bool useE = Menu["combo"]["usee"].Enabled;
            bool useQ = Menu["combo"]["useqone"].Enabled;
            bool useER = Menu["combo"]["finisher"].Enabled;
            float comboMana = Menu["combo"]["ecombomana"].As<MenuSlider>().Value;

            if (spells[SpellSlot.E].Ready &&  useE && Player.ManaPercent() > comboMana)
            {
                Obj_AI_Hero findBestETarget = GameObjects.EnemyHeroes
                    .Where(e => Menu["combo"]["useeon" + e.ChampionName.ToLower()].Enabled &&
                                e.IsValidTarget(spells[SpellSlot.E].Range)).OrderBy(e => e.Health).FirstOrDefault();
  
                if (findBestETarget != null)
                {
                    spells[SpellSlot.E].Cast(findBestETarget);
                }
            }

            if (spells[SpellSlot.Q].Ready)
            {
                bool targetHasEBuff = target.BuffManager.HasBuff(TristanaE, true);
                if (useQ)
                {
                    if (targetHasEBuff && target.IsValidTarget(Player.AttackRange - 150))
                    {
                        spells[SpellSlot.Q].Cast();
                    }
                }
                else
                {
                    spells[SpellSlot.Q].Cast();
                }
            }

            if (useER && spells[SpellSlot.R].Ready)
            {
                bool targetHasEBuff = target.BuffManager.HasBuff(TristanaE, true);
                if (targetHasEBuff == false)
                {
                    return;
                }

                double r = Player.GetSpellDamage(target, SpellSlot.R);
                double e = Player.GetSpellDamage(target, SpellSlot.E);
                float realHealth = target.Health + target.PhysicalShield + 50;
                int stacks = target.BuffManager.GetBuffCount(TristanaE, true);
                bool isKillable = r + e * (0.3 * stacks + 1) > realHealth;

                if (isKillable)
                {
                    spells[SpellSlot.R].Cast(target);
                }
            }
        }
    }
}