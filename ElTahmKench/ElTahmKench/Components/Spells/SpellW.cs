namespace ElTahmKench.Components.Spells
{
    using System;
    using System.Linq;

    using ElTahmKench.Enumerations;
    using ElTahmKench.Utils;

    using Aimtec;
    using Aimtec.SDK;

    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.Prediction.Skillshots;
    using Aimtec.SDK.TargetSelector;
    using Aimtec.SDK.Util.Cache;

    /// <summary>
    ///     The spell W.
    /// </summary>
    internal class SpellW : ISpell
    {
        #region Properties

        public float minionWRange = 700f;

        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive => SpellManager.Orbwalker.Mode == OrbwalkingMode.Combo;

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 0.5f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range
            =>
                Misc.HasDevouredBuff && Misc.LastDevouredType == DevourType.Minion
                    ? 700f
                    : 250f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.Line;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 950f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.W;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 75f;

        /// <summary>
        ///     Spell has collision.
        /// </summary>
        internal override bool Collision => true;

        #endregion

        #region Methods

        /// <summary>
        ///     The on combo callback.
        /// </summary>
        internal override void OnCombo()
        {
            try
            {
                var target = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(this.Range) && Misc.GetPassiveStacks(x) == 3 && (!x.IsInvulnerable || !x.MagicImmune))
                    .OrderBy(obj => obj.Distance(ObjectManager.GetLocalPlayer().ServerPosition))
                    .FirstOrDefault();

                if (target == null)
                {
                    return;
                }

                if (ObjectManager.GetLocalPlayer().Distance(target) + target.BoundingRadius <= this.Range + ObjectManager.GetLocalPlayer().BoundingRadius 
                    && Misc.LastDevouredType == DevourType.None)
                {
                    this.SpellObject.CastOnUnit(target);
                }
                else
                {
                    if (MyMenu.RootMenu["combominionuse"].Enabled)
                    {
                        if (!target.IsValidTarget(this.Range + 400) || (Misc.HasDevouredBuff && Misc.LastDevouredType != DevourType.Minion))
                        {
                            return;
                        }

                        // Check if the Player does not have the devourer buff.
                        if (!Misc.HasDevouredBuff)
                        {
                            // Get the minions in range
                            var minion = GameObjects.EnemyMinions.Where(n => n.Distance(ObjectManager.GetLocalPlayer().Position) < this.Range && !n.Name.ToLower().Contains("spiderling"))
                                .OrderBy(obj 
                                => obj.Distance(ObjectManager.GetLocalPlayer().ServerPosition))
                               .FirstOrDefault();

                            // Check if there are any minions.
                            if (minion != null)
                            {
                                // Cast W on the minion.
                                this.SpellObject.CastOnUnit(minion);
                            }
                        }
                        // Check if player has the devoured buff and that the last devoured type is a minion.
                        if (Misc.HasDevouredBuff && Misc.LastDevouredType == DevourType.Minion)
                        {
                            var prediction = this.SpellObject.GetPrediction(target);
                            if (prediction.HitChance >= HitChance.Medium)
                            {
                                // Spit the minion to the target location.
                                this.SpellObject.Cast(target);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellW.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The on mixed callback.
        /// </summary>
        internal override void OnMixed()
        {
            // Check if the Player does not have the devourer buff.
            if (!Misc.HasDevouredBuff)
            {
                // Gets the minion in melee range.
                var minion = GameObjects.EnemyMinions.Where(m => m.Distance(ObjectManager.GetLocalPlayer().ServerPosition) < this.Range)
                    .OrderBy(obj 
                        => obj.Distance(ObjectManager.GetLocalPlayer().ServerPosition))
                        .FirstOrDefault();
               
                // check if there are any minions.
                if (minion != null)
                {
                    // Cast W on the minion.
                    this.SpellObject.CastOnUnit(minion);
                }
            }
            
            // Check if player has the devoured buff and that the last devoured type is a minion.
            if (Misc.HasDevouredBuff && Misc.LastDevouredType == DevourType.Minion)
            {
                var target =
                    TargetSelector.GetTarget(this.minionWRange);

                if (target != null)
                {
                    // Spit the minion to the target location.
                    this.SpellObject.Cast(target, true);

                    /*var prediction = this.SpellObject.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.Medium)
                    {
                        // Spit the minion to the target location.
                        this.SpellObject.Cast(target);
                    }*/
                }
            }
        }

        #endregion
    }
}