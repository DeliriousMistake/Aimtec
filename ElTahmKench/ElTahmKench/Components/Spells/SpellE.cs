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
    using Aimtec.SDK.Prediction.Skillshots;

    /// <summary>
    ///     The spell E.
    /// </summary>
    internal class SpellE : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 0f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 900f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.Line;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 0f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.E;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 0f;

        #endregion

        #region Methods

        internal override void OnUpdate()
        {
            if (!MyMenu.RootMenu["shieldeuse"].Enabled)
            {
                return;
            }

            if (ObjectManager.GetLocalPlayer().ManaPercent() < MyMenu.RootMenu["shieldemana"].As<MenuSlider>().Value)
            {
                return;
            }

            if (ObjectManager.GetLocalPlayer().HealthPercent() <= MyMenu.RootMenu["ehealthpercentage"].As<MenuSlider>().Value 
                && ObjectManager.GetLocalPlayer().CountEnemyHeroesInRange(this.Range) > 0)
            {
                this.SpellObject.Cast();
            }
        }

        #endregion
    }
}