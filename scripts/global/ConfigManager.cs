using Godot;
using System;

public partial class ConfigManager : Node
{

    private static ConfigManager Instance { get; set; } = null!;

    #region Pickup

    public static int MedkitHealth { get; private set; }

    public static int MissileAmmo { get; private set; }

    public static int RifleAmmo { get; private set; }

    #endregion

    #region UI

    public static float MouseSensitivity { get; private set; }

    #endregion

    #region Hazard

    public static int SpikeDamage { get; private set; }

    #endregion

    #region World

    public static int SpawnHealth { get; private set; }

    #endregion

    public override void _Ready()
    {
        Instance ??= this;

        Load("res://game.cfg");
    }

    private static void Load(string path)
    {
        ConfigFile config = new();

        Error err = config.Load(path);

        if (err != Error.Ok)
        {
            throw new InvalidOperationException($"Config '{path}' not found");
        }

        string section = "Pickup";
        MedkitHealth = (int)config.GetValue(section, nameof(MedkitHealth), 50);
        MissileAmmo = (int)config.GetValue(section, nameof(MissileAmmo), 1);
        RifleAmmo = (int)config.GetValue(section, nameof(RifleAmmo), 30);

        section = "Hazard";
        SpikeDamage = (int)config.GetValue(section, nameof(SpikeDamage), 10);

        section = "World";
        SpawnHealth = (int)config.GetValue(section, nameof(SpawnHealth), 10);

        section = "UI";
        MouseSensitivity = (float)config.GetValue(section, nameof(MouseSensitivity), 300.0f);
    }

}