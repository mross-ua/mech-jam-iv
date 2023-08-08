using Godot;
using System;

public partial class CreditsScreen : TitleScreen
{

    public override void _Ready()
    {
        base._Ready();

        GetNode<TextureButton>("MarginContainer/VBoxContainer/LinkToGitHubButton").Pressed += () =>
        {
            OS.ShellOpen("https://github.com/krazkidd/mech-jam-iv");
        };
    }

}
