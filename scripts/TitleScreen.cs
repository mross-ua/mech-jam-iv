using Godot;
using System;

public partial class TitleScreen : PauseScreen
{

    protected string GameStartSceneFilename { get; set; } = "scenes/world_1.tscn";

    public override void _Ready()
    {
        base._Ready();

        PauseGame();
    }

    public override void RestartScene()
    {
        GetTree().ChangeSceneToFile(GameStartSceneFilename);

        UnpauseGame();
    }

}
