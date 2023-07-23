namespace MechJamIV
{
    public static class RandomHelper
    {

        private static readonly Random Rnd = new ((int)DateTime.Now.Ticks);

        public static int GetInt()
        {
            return Rnd.Next();
        }

        public static int GetInt(int maxValue)
        {
            return Rnd.Next(maxValue);
        }

        public static float GetSingle()
        {
            return Rnd.NextSingle();
        }

    }
}