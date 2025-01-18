using Godot;
using System;

public partial class TitleScreen : PauseScreen
{

    [Export(PropertyHint.File, "*.tscn,")]
    public string NextScene { get; set; }

    public override void _Ready()
    {
        base._Ready();

        PauseGame();
    }

    public override void RestartScene()
    {
        // HACK: This actually starts the game.

        SceneManager.GoToScene(NextScene);
    }

}
