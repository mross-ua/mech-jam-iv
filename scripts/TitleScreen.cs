using Godot;
using System;

public partial class TitleScreen : PauseScreen
{

	[Export]
	public PackedScene NextScene { get; set; } = null;

	public override void _Ready()
	{
		GetTree().Paused = true;

		base._Ready();

		RestartClicked += () =>
		{
			GetTree().ChangeSceneToPacked(NextScene);

			GetTree().Paused = false;
		};
		QuitClicked += () =>
		{
			GetTree().Quit();
		};
	}

}
