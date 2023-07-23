public partial class Grenade : ExplosiveBarrel
{
    public override int Health { get; set; } = 1;

    #region Node references

    private Timer explosionTimer;

    #endregion Node references

    public override void _Ready()
    {
        base._Ready();

        explosionTimer = GetNode<Timer>("ExplosionTimer");
        explosionTimer.Timeout += () => Hurt(Health, GlobalTransform.Origin, Vector2.Zero);
    }

    public void Prime() => explosionTimer.Start();
}