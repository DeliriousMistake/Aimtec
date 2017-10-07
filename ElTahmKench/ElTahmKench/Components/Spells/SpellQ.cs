using Aimtec;
using Aimtec.SDK.Damage;
using Aimtec.SDK.Events;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Prediction.Skillshots;
using Aimtec.SDK.TargetSelector;
using Aimtec.SDK.Util.Cache;

namespace ElTahmKench.Components.Spells
{
    using System;
    using System.Linq;

    using ElTahmKench.Enumerations;
    using ElTahmKench.Utils;

    using Aimtec;
    using Aimtec.SDK;

    /// <summary>
    ///     The spell Q.
    /// </summary>
    internal class SpellQ : ISpell
    {
        #region Properties
   
        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 0.25f;

        /// <summary>
        ///     Spell has collision.
        /// </summary>
        internal override bool Collision => true;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 800f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.Line;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 2000f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.Q;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 70f;

        #endregion

        #region Methods

        /// <summary>
        ///     The on combo callback.
        /// </summary>
        internal override void OnCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(this.Range);

                if (Misc.HasDevouredBuff && Misc.LastDevouredType == DevourType.Enemy)
                {
                    target = null;
                }

                if (Game.TickCount - Misc.LastDevourer <= 500) //Misc.GetPassiveStacks(target) != 3
                {
                    return;
                }

                if (target != null)
                {
                    if (ObjectManager.GetLocalPlayer().Distance(target) <= this.Range)
                    {
                        var prediction = this.SpellObject.GetPrediction(target);
                        if (prediction.HitChance >= HitChance.Medium)
                        {
                            this.SpellObject.Cast(target);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellQ.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The on mixed callback.
        /// </summary>
        internal override void OnMixed()
        {
            this.OnCombo();
        }

        /// <summary>
        ///     The on last hit callback.
        /// </summary>
        internal override void OnLastHit()
        {
            var minion =
                GameObjects.EnemyMinions
                    .Where(obj => obj.Distance(ObjectManager.GetLocalPlayer().ServerPosition) < this.Range &&
                                  ObjectManager.GetLocalPlayer()
                                      .GetSpellDamage(ObjectManager.GetLocalPlayer(), this.SpellSlot) > obj.Health)
                    .MinOrDefault(obj => obj.Health);

            if (minion != null)
            {
                this.SpellObject.Cast(minion);
            }
        }

        #endregion
    }
}