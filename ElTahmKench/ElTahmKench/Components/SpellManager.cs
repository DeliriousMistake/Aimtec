namespace ElTahmKench.Components
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using ElTahmKench.Components.Spells;
    using ElTahmKench.Enumerations;
    using ElTahmKench.Utils;

    using Aimtec;
    using Aimtec.SDK;

    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu.Components;

    /// <summary>
    ///     The spell manager.
    /// </summary>
    internal class SpellManager
    {
        #region Fields

        /// <summary>
        ///     The spells.
        /// </summary>
        internal readonly List<ISpell> spells = new List<ISpell>();

        /// <summary>
        ///     Gets or sets the champion spells.
        /// </summary>
        internal Dictionary<string, List<SpellSlot>> ChampionSpells { get; } = new Dictionary<string, List<SpellSlot>>();

        #endregion

        #region Constructors and Destructors

        public static Orbwalker Orbwalker = new Orbwalker();
    
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellManager" /> class.
        /// </summary>
        internal SpellManager()
        {
            try
            {
                this.LoadSpells(new List<ISpell>() { new SpellQ(), new SpellW(), new SpellE() });
                Misc.SpellW = new SpellW();
                Misc.SpellQ = new SpellQ();
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: Can not initialize the spells - {0}", e);
                throw;
            }

            Game.OnUpdate += this.Game_OnUpdate;
            BuffManager.OnAddBuff += this.OnBuffAdd;
            BuffManager.OnRemoveBuff += this.OnBuffRemove;
        }

        /// <summary>
        ///    The OnBuffRemove event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void OnBuffRemove(Obj_AI_Base sender, Buff args)
        {
            try
            {
                if (!sender.IsMe)
                {
                    return;
                }

                if (args.Name.Equals(Misc.DevouredBuffName))
                {
                    Misc.LastDevouredType = DevourType.None;
                    Misc.LastDevourer = Game.TickCount;
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: OnBuffRemove: {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The OnBuffAdd event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void OnBuffAdd(Obj_AI_Base sender, Buff args)
        {
            try
            {
                if (args.Name.Equals(Misc.DevouredCastBuffName))
                {
                    var hero = sender as Obj_AI_Hero;
                    if (hero != null)
                    {
                        Misc.LastDevouredType = hero.IsAlly ? DevourType.Ally : DevourType.Enemy;                   
                        return;
                    }

                    var minion = sender as Obj_AI_Minion;
                    if (minion != null)
                    {
                        Misc.LastDevouredType = DevourType.Minion;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: OnBuffAdd: {0}", e);
                throw;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The is the spell active method.
        /// </summary>
        /// <param name="spellSlot">
        ///     The spell slot.
        /// </param>
        /// <param name="orbwalkingMode">
        ///     The orbwalking mode.
        /// </param>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        private static bool IsSpellActive(SpellSlot spellSlot, OrbwalkerMode orbwalkingMode)
        {
            if (Orbwalker.Implementation.GetActiveMode() != orbwalkingMode) //slot
            {
                return false;
            }

            try
            {
                var orbwalkerModeLower = Orbwalker.ModeName.ToLower();
                var spellSlotNameLower = spellSlot.ToString().ToLower();

                if (orbwalkerModeLower.Equals("lasthit")
                     && !spellSlotNameLower.Equals("q") || orbwalkerModeLower.Equals("mixed") && spellSlotNameLower.Equals("e")
                                                               || orbwalkerModeLower.Equals("combo") && spellSlotNameLower.Equals("e"))
                {
                    return false;
                }

                return MyMenu.RootMenu[orbwalkerModeLower + spellSlotNameLower + "use"].Enabled &&
                       MyMenu.RootMenu[orbwalkerModeLower + spellSlotNameLower + "mana"].As<MenuSlider>().Value
                       <= ObjectManager.GetLocalPlayer().ManaPercent();
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: Can not get spell active state for slot {0} - {1}", spellSlot.ToString(), e);
                throw;
            }
        }

        /// <summary>
        ///     The game on update callback.
        /// </summary>
        private void Game_OnUpdate()
        {
            if (ObjectManager.GetLocalPlayer().IsDead || MenuGUI.IsChatOpen() || MenuGUI.IsShopOpen() || ObjectManager.GetLocalPlayer().IsRecalling()) return;

            this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalker.Implementation.Combo))
                .ToList()
                .ForEach(spell => spell.OnCombo());

           this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalker.Implementation.LastHit))
                .ToList()
                .ForEach(spell => spell.OnLastHit());

            this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalker.Implementation.Mixed))
                .ToList()
                .ForEach(spell => spell.OnMixed());

            this.spells.ToList().ForEach(spell => spell.OnUpdate());
        }

        /// <summary>
        ///     The load spells method.
        /// </summary>
        /// <param name="spellList">
        ///     The spells.
        /// </param>
        private void LoadSpells(IEnumerable<ISpell> spellList)
        {
            foreach (var spell in spellList)
            {
                MyMenu.GenerateSpellMenu(spell.SpellSlot);
                this.spells.Add(spell);
            }
        }

        #endregion
    }
}