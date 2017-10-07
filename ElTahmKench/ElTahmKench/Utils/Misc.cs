using Aimtec;
using Aimtec.SDK.Extensions;

namespace ElTahmKench.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElTahmKench.Components.Spells;
    using ElTahmKench.Enumerations;

    using Aimtec;
    using Aimtec.SDK;

    /// <summary>
    ///     The misc.
    /// </summary>
    internal static class Misc
    {
        #region Methods

        /// <summary>
        ///     Spell W.
        /// </summary>
        public static SpellW SpellW;

        /// <summary>
        ///     Spell Q.
        /// </summary>
        public static SpellQ SpellQ;

        /// <summary>
        ///     The last devoured target type
        /// </summary>
        internal static DevourType LastDevouredType;

        /// <summary>
        ///     Time
        /// </summary>
        internal static float LastDevourer;

        /// <summary>
        /// 
        /// </summary>
        internal static string DevouredBuffName = "tahmkenchwhasdevouredtarget";

        /// <summary>
        /// 
        /// </summary>
        internal static string DevouredCastBuffName = "tahmkenchwdevoured";

        /// <summary>
        ///     Player has the devoured buff.
        /// </summary>
        internal static bool HasDevouredBuff => ObjectManager.GetLocalPlayer().HasBuff(DevouredBuffName);

        /// <summary>
        ///     Gets the buff indexes handled.
        /// </summary>
        /// <value>
        ///     The buff indexes handled.
        /// </value>
        internal static Dictionary<int, List<int>> BuffIndexesHandled { get; } = new Dictionary<int, List<int>>();

        /// <summary>
        ///     The buffs types to devourer an enemy.
        /// </summary>
        public static BuffType[] DevourerBuffTypes = new[]
        {
            BuffType.Charm, BuffType.Flee,
            BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
            BuffType.Taunt, BuffType.Fear, BuffType.Knockback, BuffType.Knockup
        };

        /// <summary>
        ///     Gets the passive stacks.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        ///     <see cref="GetPassiveStacks" />
        /// </returns>
        internal static int GetPassiveStacks(Obj_AI_Hero target) => target.GetBuffCount("tahmkenchpdebuffcounter");

        /// <summary>
        ///     Searches for the min or default element.
        /// </summary>
        /// <typeparam name="T">
        ///     The type.
        /// </typeparam>
        /// <typeparam name="TR">
        ///     The comparing type.
        /// </typeparam>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="valuingFoo">
        ///     The comparing function.
        /// </param>
        /// <returns></returns>
        public static T MinOrDefault<T, TR>(this IEnumerable<T> container, Func<T, TR> valuingFoo)
            where TR : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }

            var minElem = enumerator.Current;
            var minVal = valuingFoo(minElem);

            while (enumerator.MoveNext())
            {
                var currVal = valuingFoo(enumerator.Current);

                if (currVal.CompareTo(minVal) < 0)
                {
                    minVal = currVal;
                    minElem = enumerator.Current;
                }
            }

            return minElem;
        }

        #endregion
    }
}