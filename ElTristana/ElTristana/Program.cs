using Aimtec;
using Aimtec.SDK.Events;
using ElTristana;

namespace Twitch
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEvents.GameStart += GameEvents_GameStart;
        }

        private static void GameEvents_GameStart()
        {
            if (ObjectManager.GetLocalPlayer().ChampionName != "Tristana")
            {
                return;
            }

            var Tristana = new Tristana();
        }
    }
}
