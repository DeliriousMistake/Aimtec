using Aimtec.SDK.Events;
using Aimtec.SDK.Orbwalking;

namespace ElTahmKench
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using ElTahmKench.Components;
    using Aimtec;
    using Aimtec.SDK;

    /// <summary>
    ///     The program.
    /// </summary>
    public static class Program
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the orbwalker.
        /// </summary>
       internal static Orbwalker Orbwalker { get; set; }

       // internal static Orbwalker Orbwalker = new Orbwalker();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The EntryPoint of the solution.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        public static void Main(string[] args)
        {
            GameEvents.GameStart += () =>
            {
                if (ObjectManager.GetLocalPlayer().ChampionName.Equals("TahmKench"))
                {
                    Bootstrap();
                }
            };
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The bootstrapping method for the components.
        /// </summary>
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "They would not be used.")]
        private static void Bootstrap()
        {
            try
            {
                new MyMenu();
                new SpellManager();
            }
            catch (Exception e)
            {
                Console.WriteLine("@Program.cs: Can not bootstrap components - {0}", e);
                throw;
            }
        }

        #endregion
    }
}