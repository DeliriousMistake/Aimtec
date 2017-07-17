using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK;
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
