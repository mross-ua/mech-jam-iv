using Godot;
using System;

public partial class SceneManager : Node
{

    private static SceneManager Instance { get; set; } = null!;

    private static Node currentScene = null!;

    public override void _Ready()
    {
        Instance ??= this;

        // global scripts are loaded into the tree first,
        // and the project's main scene is loaded last
        currentScene = GetTree().Root.GetChild(-1);
    }

    public static void GoToScene(string path)
    {
        Instance.CallDeferred(MethodName.DeferredGoToScene, path);
    }

    public static void ReloadScene()
    {
        Instance.CallDeferred(MethodName.DeferredGoToScene, currentScene.SceneFilePath);
    }

    private static void DeferredGoToScene(string path)
    {
        // NOTE: There is more than one way to load a scene into the
        //       scene tree. For a simpler game with simple levels,
        //       we can just delete the current scene and load the
        //       next at the same time.
        //
        //       See https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html

        Node previousScene = currentScene;

        //TODO should we cache the packed scene?

        // these are equivalent...
        PackedScene scene = GD.Load<PackedScene>(path);
        // but this one doesn't compile
        //PackedScene scene = ResourceLoader.Load(path, "PackedScene", ResourceLoader.CacheMode.CACHE_MODE_REUSE);

        currentScene = scene.Instantiate();

        Instance.GetTree().Root.AddChild(currentScene);

        // this is really important and is what SceneTree.change_scene_to_file() would do
        Instance.GetTree().CurrentScene = currentScene;

        if (previousScene is World source && currentScene is World target)
        {
            // BUG #56: Set minimum player health/ammo
            // BUG #57: Restart level isn't getting the original values

            target.Updated += previousScene.Free;

            target.DeferredUpdateFrom(source);
        }
        else
        {
            previousScene.Free();
        }
    }

    public static void PauseGame()
    {
        Instance.GetTree().Paused = true;
    }

    public static void UnpauseGame()
    {
        Instance.GetTree().Paused = false;
    }

    public static void QuitGame()
    {
        // notify all nodes
        Instance.GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);

        Instance.GetTree().Quit();
    }

}