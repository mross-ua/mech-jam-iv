using Godot;
using System;

namespace MechJamIV;

public partial class SceneManager : Node
{

    private static SceneManager Instance { get; set; } = null!;

    private static Node currentScene = null!;

    private static readonly ConfigFile reloadConfig = new();

    public override void _Ready()
    {
        Instance ??= this;

        // global scripts are loaded into the tree first,
        // and the project's main scene is loaded last
        currentScene = GetTree().Root.GetChild(-1);
    }

    public override void _Notification(int what)
    {
        // do *not* call base method (per docs)
        //base._Notification(what);

        if (what == NotificationWMCloseRequest)
        {
            if (OS.IsStdOutVerbose())
            {
                GD.Print($"{nameof(SceneManager)} received request to close window.");

                GD.Print($"Quitting game...");
            }

            // NOTE: This gets called before the rest of the scene tree
            //       gets the notification (since this is an autoload).
            //       In future, we may have to force a notification order.

            // BUG: This is causing a memory leak and sometimes the game
            //      process doesn't seem to terminate (even though the
            //      window closes). The Godot editor deals with it okay
            //      but the VS Code debugger remains running.

            //TODO revisit this after 4.7
            //     https://github.com/godotengine/godot/pull/116711
            Instance.GetTree().Quit();
        }
    }

    public static void GoToScene(string path)
    {
        reloadConfig.Clear();

        if (currentScene is World source)
        {
            source.Save(reloadConfig);
        }

        Instance.CallDeferred(MethodName.DeferredGoToScene, path, reloadConfig);
    }

    public static void ReloadScene()
    {
        Instance.CallDeferred(MethodName.DeferredGoToScene, currentScene.SceneFilePath, reloadConfig);
    }

    private static void DeferredGoToScene(string path, ConfigFile config)
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

        if (currentScene is World target)
        {
            // BUG #56: Set minimum player health/ammo

            target.Loaded += previousScene.Free;

            target.DeferredLoad(config);
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
    }

}
