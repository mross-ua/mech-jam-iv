using Godot;
using System;
using System.Linq;

namespace MechJamIV;

public partial class Spawn : Node2D
{

    [Signal]
    public delegate void SpawnReachedEventHandler(Player player);

    [Export]
    public bool IsWorldSpawn { get; set; }

    private Tween? ModulationTween;

    #region Node references

    public Marker2D SpawnPointMarker { get; private set; } = null!;

    public CanvasItem tileMap = null!;
    private Timer healthGenTimer = null!;
    private Area2D area2D = null!;
    private CollisionShape2D collisionShape2D = null!;
    private GpuParticles2D gpuParticles2D = null!;

    #endregion

    public override void _Ready()
    {
        SpawnPointMarker = GetNode<Marker2D>("SpawnPoint");

        tileMap = GetNode<CanvasItem>("TileMap");

        healthGenTimer = GetNode<Timer>("HealthGenTimer");
        healthGenTimer.Timeout += Generate;

        area2D = GetNode<Area2D>("Area2D");
        area2D.BodyEntered += (body) =>
        {
            if (body is Player player)
            {
                gpuParticles2D.Visible = true;

                if (healthGenTimer.IsStopped())
                {
                    healthGenTimer.Start();
                }

                ModulationTween?.Kill();
                ModulationTween = CreateTween();
                ModulationTween.TweenProperty(tileMap, "self_modulate", Colors.Red, 0.5f)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.In);

                EmitSignal(SignalName.SpawnReached, player);
            }
        };
        area2D.BodyExited += (body) =>
        {
            if (body is Player player)
            {
                if (!area2D.GetOverlappingBodies().OfType<Player>().Any())
                {
                    gpuParticles2D.Visible = false;

                    healthGenTimer.Stop();

                    ModulationTween?.Kill();
                    ModulationTween = CreateTween();
                    ModulationTween.TweenProperty(tileMap, "self_modulate", Colors.White, 2.0f)
                        .SetTrans(Tween.TransitionType.Sine)
                        .SetEase(Tween.EaseType.Out);
                }
            }
        };

        collisionShape2D = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
        gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");
    }

    private void Generate()
    {
        foreach (Node2D body in area2D.GetOverlappingBodies())
        {
            if (body is Player player)
            {
                player.Heal(ConfigManager.SpawnHealth, false);
            }
        }
    }

}
