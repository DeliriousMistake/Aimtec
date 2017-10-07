using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.TargetSelector;
using Aimtec.SDK.Util.Cache;

namespace ElTahmKench.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElTahmKench.Enumerations;
    using ElTahmKench.Utils;

    using Aimtec;
    using Aimtec.SDK;

    /// <summary>
    ///     The my menu class.
    /// </summary>
    internal class MyMenu
    {

        /// <summary>
        ///     List with default activated spells.
        /// </summary>
        public static readonly List<string> TrueStandard = new List<string> { "Stun", "Charm", "Flee", "Fear", "Taunt", "Polymorph", "Suppression" };


        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MyMenu" /> class.
        /// </summary>
        internal MyMenu()
        {
            RootMenu = new Menu("ElTahmKench", "ElTahmKench", true);

            //RootMenu.AddSubMenu(GetTargetSelectorNode());
            // RootMenu.AddSubMenu(GetOrbwalkerNode());

            SpellManager.Orbwalker.Attach(RootMenu);

            RootMenu.Attach();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the root menu.
        /// </summary>
        internal static Menu RootMenu { get; set; }

        #endregion

        #region Methods


        /// <summary>
        ///     The generate spell menu method.
        /// </summary>
        /// <param name="spellSlot">
        ///     The spell slot.
        /// </param>
        internal static void GenerateSpellMenu(SpellSlot spellSlot)
        {
            try
            {
                var spellSlotName = spellSlot.ToString();
                var spellSlotNameLower = spellSlotName.ToLower();

                var node = new Menu("spellmenu" + spellSlotNameLower, spellSlot + " Settings");

                if (!spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeCombo = new Menu(spellSlotNameLower + "combomenu", "Combo");
                    {
                        nodeCombo.Add(
                            new MenuBool("combo" + spellSlotNameLower + "use", "Use " + spellSlotName));
                        nodeCombo.Add(
                            new MenuSlider("combo" + spellSlotNameLower + "mana", "Min. Mana", 5));

                        if (spellSlotNameLower.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                        {
                            nodeCombo.Add(new MenuBool("combominionuse", "Eat minion", false));
                        }
                    }

                    node.Add(nodeCombo);
                }

                if (spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeShield = new Menu(spellSlotNameLower + "shieldmenu", "Shield");
                    {
                        nodeShield.Add(new MenuBool("shield" + spellSlotNameLower + "use", "Use " + spellSlotName));
                        nodeShield.Add(new MenuSlider("ehealthpercentage", "Use " + spellSlotName + " on health percentage", 20));
                        nodeShield.Add(new MenuSlider("shield" + spellSlotNameLower + "mana", "Min. Mana", 5));
                    }

                    node.Add(nodeShield);
                }

                if (!spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeMixed = new Menu(spellSlotNameLower + "mixedmenu", "Mixed");
                    {
                        nodeMixed.Add(
                            new MenuBool("mixed" + spellSlotNameLower + "use", "Use " + spellSlotName));
                        nodeMixed.Add(
                            new MenuSlider("mixed" + spellSlotNameLower + "mana", "Min. Mana", 50));
                    }

                    node.Add(nodeMixed);
                }

                if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeLastHit = new Menu(spellSlotNameLower + "lasthitmenu", "LastHit");
                    {
                        nodeLastHit.Add(
                            new MenuBool("lasthit" + spellSlotNameLower + "use", "Use " + spellSlotName));
                        nodeLastHit.Add(
                            new MenuSlider("lasthit" + spellSlotNameLower + "mana", "Min. Mana", 50));
                    }

                    node.Add(nodeLastHit);
                }

                if (spellSlotNameLower.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeAlly = new Menu(spellSlotNameLower + "allymenu", "Ally settings");
                    {
                        nodeAlly.Add(new MenuBool("allydangerousults", "Use " + spellSlotName + " on dangerous ults"));
                        nodeAlly.Add(new MenuBool("walktotarget", "Orbwalk to target"));
                        nodeAlly.Add(new MenuBool("sep3-0-2", String.Empty));
                        nodeAlly.Add(new MenuBool("allylowhpults", "Use " + spellSlotName + " on low HP allies"));
                        nodeAlly.Add(new MenuSlider("allylowhpultsslider", "Use " + spellSlotName + " Ally Health percentage", 50));
                        nodeAlly.Add(new MenuSeperator("sep3-2", String.Empty));
                        nodeAlly.Add(new MenuBool("allycc", "Use " + spellSlotName + " when ally is cc'd"));

                        nodeAlly.Add(new MenuSeperator("sep3-0", String.Empty));

                        foreach (var allies in GameObjects.AllyHeroes.Where(a => !a.IsMe))
                        {
                            nodeAlly.Add(new MenuBool($"won{allies.ChampionName}", "Use on " + allies.ChampionName));
                        }
                    }

                    node.Add(nodeAlly);

                    var nodeBuff = new Menu("buffstuff", "Buff types");
                    {
                        foreach (var buffType in Misc.DevourerBuffTypes.Select(x => x.ToString()))
                        {
                            nodeBuff.Add(new MenuBool($"buffscc{buffType}", buffType, TrueStandard.Contains($"{buffType}")));
                        }

                        node.Add(nodeBuff);
                    }
                }

                RootMenu.Add(node);
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@MyMenu.cs: Can not generate menu for spell - {0}", e);
                throw;
            }
        }


        #endregion
    }
}