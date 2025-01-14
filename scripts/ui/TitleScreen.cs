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
        // once we change scenes, GetTree() will return null
        SceneTree currentSceneTree = GetTree();

        if (currentSceneTree.ChangeSceneToFile(NextScene) == Error.Ok)
        {
            UnpauseGame(currentSceneTree);
        }
        else
        {
            GD.PrintErr($"Cannot open scene file {NextScene}");
        }
    }

}
