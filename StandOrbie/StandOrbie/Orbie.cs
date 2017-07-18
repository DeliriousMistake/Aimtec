using System;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Orbwalking;

namespace StandOrbie
{
    internal class Orbie
    {
        public static Menu Menu = new Menu("Orbie", "Orbie", true);
        public static Orbwalker Orbwalker = new Orbwalker();

        public Orbie()
        {
            Orbwalker.Attach(Menu);
            Menu.Attach();
        }
    }
}
