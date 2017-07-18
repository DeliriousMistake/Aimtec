namespace StandOrbie
{
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Orbwalking;

    class Program
    {
        public static Menu Menu = new Menu("Orbwalker", "Orbwalker", true);
        public static Orbwalker Orbwalker = new Orbwalker();

        static void Main(string[] args)
        {
            GameEvents.GameStart += () => Orbwalker.Attach(Menu);
        }
    }
}
