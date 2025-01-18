using Godot;
using System;

public partial class SceneManager : Node
{

    public CompressedTexture2D CursorTexture { get; private set; }

    private Node currentScene;

    public override void _Ready()
    {
        // global scripts are loaded into the tree first,
        // and the project's main scene is loaded last
        currentScene = GetTree().Root.GetChild(-1);

        CursorTexture = ResourceLoader.Load<CompressedTexture2D>("res://assets/sprites/WhiteCrosshair-5.png");
    }

    public void GoToScene(string path)
    {
        CallDeferred(MethodName.DeferredGoToScene, path);
    }

    public void ReloadScene()
    {
        CallDeferred(MethodName.DeferredGoToScene, currentScene.SceneFilePath);
    }

    private void DeferredGoToScene(string path)
    {
        // NOTE: There is more than one way to load a scene into the
        //       scene tree. For a simpler game with simple levels,
        //       we can just delete the current scene and load the
        //       next at the same time.
        //
        //       See https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html

        currentScene.Free();

        //TODO should we cached the packed scene?

        // these are equivalent...
        PackedScene scene = GD.Load<PackedScene>(path);
        // but this one doesn't compile
        //PackedScene scene = ResourceLoader.Load(path, "PackedScene", ResourceLoader.CacheMode.CACHE_MODE_REUSE);

        currentScene = scene.Instantiate();

        GetTree().Root.AddChild(currentScene);

        // this is really important and is what SceneTree.change_scene_to_file() would do
        GetTree().CurrentScene = currentScene;
    }

    public void PauseGame()
    {
        GetTree().Paused = true;
    }

    public void UnpauseGame()
    {
        GetTree().Paused = false;
    }

    public void QuitGame()
    {
        // notify all nodes
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);

        GetTree().Quit();
    }

}