public partial class TitleScreen : PauseScreen
{
    [Export(PropertyHint.File, "*.tscn,")]
    public string GameStartSceneFilename { get; set; }

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