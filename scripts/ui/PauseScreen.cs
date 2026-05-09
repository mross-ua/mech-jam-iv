using Godot;
using System;

public partial class PauseScreen : CanvasLayer
{

    #region Node references

    protected static SceneManager SceneManager => SceneManager.Instance;

    private Button continueButton;

    #endregion

    public override void _Ready()
    {
        continueButton = GetNodeOrNull<Button>("Menu/ContinueButton");
        if (continueButton != null)
        {
            continueButton.Pressed += () => UnpauseGame();
        }

        Button restartButton = GetNode<Button>("Menu/RestartButton");
        restartButton.Pressed += () => RestartScene();

        Button quitButton = GetNode<Button>("Menu/QuitButton");
        quitButton.Pressed += () => SceneManager.QuitGame();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("quit") && continueButton != null)
        {
            CallDeferred(MethodName.UnpauseGame);

            GetViewport().SetInputAsHandled();
        }
    }

    public void PauseGame()
    {
        SceneManager.PauseGame();

        Visible = true;
    }

    public void UnpauseGame()
    {
        Visible = false;

        SceneManager.UnpauseGame();
    }

    public virtual void RestartScene()
    {
        SceneManager.ReloadScene();
    }

}
