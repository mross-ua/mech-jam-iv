using Godot;
using System;

public partial class ResourceManager : Node
{

    private static ResourceManager Instance { get; set; } = null!;

    public static CompressedTexture2D CursorTexture { get; private set; } = null!;

    public override void _Ready()
    {
        Instance ??= this;

        CursorTexture = ResourceLoader.Load<CompressedTexture2D>("res://assets/sprites/WhiteCrosshair-5.png");
    }

}