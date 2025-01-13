using Godot;
using System;

public partial class PauseScreen : CanvasLayer
{

    #region Node references

    private Button continueButton;

    #endregion

    public override void _Ready()
    {
        //TODO this class shouldnt be the base (if continue button is optional); refactor this

        continueButton = GetNodeOrNull<Button>("Menu/ContinueButton");
        if (continueButton != null)
        {
            continueButton.Pressed += () => UnpauseGame(GetTree());
        }

        Button restartButton = GetNode<Button>("Menu/RestartButton");
        restartButton.Pressed += () => RestartScene();

        Button quitButton = GetNode<Button>("Menu/QuitButton");
        quitButton.Pressed += () => QuitGame();
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
        GetTree().Paused = true;

        Visible = true;
    }

    public void UnpauseGame(SceneTree sceneTree)
    {
        Visible = false;

        sceneTree.Paused = false;
    }

    public void QuitGame()
    {
        GetTree().Quit();
    }

    public virtual void RestartScene()
    {
        // once we change scenes, GetTree() will return null
        SceneTree currentSceneTree = GetTree();

        if (currentSceneTree.ReloadCurrentScene() == Error.Ok)
        {
            UnpauseGame(currentSceneTree);
        }
        else
        {
            GD.PrintErr($"Cannot reload current scene");
        }
    }

}
