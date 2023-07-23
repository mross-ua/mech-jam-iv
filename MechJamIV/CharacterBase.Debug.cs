#if DEBUG


namespace MechJamIV;

public abstract partial class CharacterBase
{

    public override void _Draw()
    {
        DrawLine(Vector2.Zero, GetMovementDirection() * 25, Colors.Red);
    }

}

#endif
