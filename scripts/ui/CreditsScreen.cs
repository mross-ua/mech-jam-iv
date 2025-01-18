using Godot;
using System;

public partial class CreditsScreen : TitleScreen
{

    private static readonly string URL_GITHUB = "https://github.com/krazkidd/mech-jam-iv".URIEncode();

    #region Node references

    private TextureButton linkToGitHubButton;

    #endregion

    public override void _Ready()
    {
        base._Ready();

        //TODO this isn't working
        Input.SetDefaultCursorShape();

        linkToGitHubButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/LinkToGitHubButton");
        linkToGitHubButton.Pressed += () =>
        {
            if (OS.ShellOpen(URL_GITHUB) != Error.Ok)
            {
                GD.PrintErr($"Cannot open URL {URL_GITHUB}");
            }
        };
    }

}
