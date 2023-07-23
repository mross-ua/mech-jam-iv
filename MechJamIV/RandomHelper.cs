namespace MechJamIV;

public static class RandomHelper
{

    private static readonly Random Rnd = new ((int)DateTime.Now.Ticks);

    public static int GetInt() => Rnd.Next();

    public static int GetInt(int maxValue) => Rnd.Next(maxValue);

    public static float GetSingle() => Rnd.NextSingle();

}