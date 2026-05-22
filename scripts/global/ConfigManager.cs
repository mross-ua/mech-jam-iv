using Godot;
using System;

namespace MechJamIV;

public partial class ConfigManager : Node
{

    private static ConfigManager Instance { get; set; } = null!;

    #region Pickup

    public static int MedkitHealth { get; private set; }

    public static int MissileAmmo { get; private set; }

    public static int RifleAmmo { get; private set; }

    #endregion

    #region World

    public static int SpikeDamage { get; private set; }

    public static int SpawnHealth { get; private set; }

    #endregion

    #region UI

    public static float MouseSensitivity { get; private set; }

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
        MedkitHealth = config.GetValue(section, nameof(MedkitHealth), 50).AsInt32();
        MissileAmmo = config.GetValue(section, nameof(MissileAmmo), 1).AsInt32();
        RifleAmmo = config.GetValue(section, nameof(RifleAmmo), 30).AsInt32();

        section = "World";
        SpikeDamage = config.GetValue(section, nameof(SpikeDamage), 10).AsInt32();
        SpawnHealth = config.GetValue(section, nameof(SpawnHealth), 10).AsInt32();

        section = "UI";
        MouseSensitivity = config.GetValue(section, nameof(MouseSensitivity), 300.0f).AsSingle();
    }

}
