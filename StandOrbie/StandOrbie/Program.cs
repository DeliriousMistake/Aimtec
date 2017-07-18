namespace StandOrbie
{
    using Aimtec.SDK.Events;

    class Program
    {
        static void Main(string[] args)
        {
            GameEvents.GameStart += () => new Orbie();
        }
    }
}
