using Aimtec;
using Aimtec.SDK.Events;

namespace Jinx
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEvents.GameStart += GameEvents_GameStart;
        }

        private static void GameEvents_GameStart()
        {
            if (ObjectManager.GetLocalPlayer().ChampionName != "Jinx")
            {
                return;
            }

            var Jinx = new Jinx();
        }
    }
}
