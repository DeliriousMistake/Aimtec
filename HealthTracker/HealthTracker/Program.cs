namespace HealthTracker
{
    using Aimtec;
    using Aimtec.SDK.Events;
    class Program
    {
        static void Main(string[] args)
        {
            GameEvents.GameStart += GameEvents_GameStart;
        }

        private static void GameEvents_GameStart()
        {
            var healthTracker = new Healthtracker();
        }
    }
}
