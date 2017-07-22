using System;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Orbwalking;

namespace StandOrbie
{
    internal class Orbie
    {
        public static Menu Menu = new Menu("Orbie", "Orbie", true);

        public Orbie()
        {
            Orbwalker.Implementation.Attach(Menu);
            Menu.Attach();
        }
    }
}
