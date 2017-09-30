using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Util.Cache;

namespace HealthTracker
{
    internal class Healthtracker
    {
        public static Menu Menu = new Menu("tracker", "Tracker", true);

        private int HudOffsetRight
        {
            get { return Menu["xpos"].As<MenuSlider>().Value; }
        }

        private int HudOffsetTop
        {
            get { return Menu["ypos"].As<MenuSlider>().Value; }
        }

        private bool HudActive
        {
            get { return Menu["trackhealth"].Enabled; }
        }

        private void DrawRect(float x, float y, int width, float height, float thickness, Color color)
        {
            for (var i = 0; i < height; i++)
            {
                Render.Line(x, y + i, x + width, y + i, color);
            }
        }

        public Healthtracker()
        {
            try
            {
                Menu.Add(new MenuBool("trackhealth", "Health Tracker"));
                Menu.Add(new MenuSlider("xpos", "X Position", 250, 0, 2000));
                Menu.Add(new MenuSlider("ypos", "Y Position", 100, 0, 2000));

                Menu.Attach();

                Render.OnPresent += Render_OnPresent;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // Whatever for now
        private void Render_OnPresent()
        {
            if (!HudActive)
            {
                return;
            }

            float i = 0;
            foreach (var hero in GameObjects.EnemyHeroes)
            {
                var champion = hero.ChampionName;
                if (champion.Length > 20)
                {
                    champion = champion.Remove(7) + "...";
                }

                var healthPercent = (int)(hero.Health / hero.MaxHealth * 100);
                var championInfo = $"{champion} ({healthPercent}%)";
                const int Height = 25;

                var timeR = hero.SpellBook.GetSpell(SpellSlot.R).CooldownEnd - Game.ClockTime;
                var ultText = timeR <= 0
                    ? "READY"
                    : (timeR < 10 ? timeR.ToString("N1") : ((int)timeR).ToString()) + "s";

                if (hero.SpellBook.GetSpell(SpellSlot.R).Level == 0)
                {
                    ultText = "N/A";
                }

                championInfo += $" - R: {ultText}";

                // Draws the rectangle
                DrawRect(
                    Render.Width - this.HudOffsetRight,
                    this.HudOffsetTop + i,
                    200,
                    Height,
                    1,
                    Color.FromArgb(175, 51, 55, 51));

                DrawRect(
                    Render.Width - this.HudOffsetRight + 2,
                    this.HudOffsetTop + i - -2,
                    healthPercent <= 0 ? 100 : healthPercent * 2 - 4,
                    Height - 4,
                    1,
                    healthPercent < 30 && healthPercent > 0
                        ? Color.FromArgb(255, 250, 0, 23)
                        : healthPercent < 50
                            ? Color.FromArgb(255, 230, 169, 14)
                            : Color.FromArgb(255, 2, 157, 10));

                // Draws the championnames
                /*Render.Text((int)((Render.Width - HudOffsetRight) + 20f), (int)
                    (HudOffsetTop + i + 12),
                    (int)(hero.Health / hero.MaxHealth * 100) > 0 ? Color.AliceBlue : Color.Red, championInfo,
                    RenderTextFlags.VerticalCenter | RenderTextFlags.SingleLine);*/

                Render.Text((int)((float)(Render.Width - HudOffsetRight + 10) + 10f), (int)
                    (HudOffsetTop + i + 11),
                    (int)(hero.Health / hero.MaxHealth * 100) > 0 ? Color.AliceBlue : Color.Red, championInfo,
                    RenderTextFlags.Center | RenderTextFlags.Bottom | RenderTextFlags.VerticalCenter);

                i += 20f + 5;
            }
        }
    }
}
