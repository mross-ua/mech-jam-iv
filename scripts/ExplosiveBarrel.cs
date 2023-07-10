using Godot;
using System;

public partial class ExplosiveBarrel : Barrel
{

	[Export]
	public int Health { get; set; } = 10;

	#region Node references

	private CharacterAnimator characterAnimator;

    #endregion

    public override void _Ready()
    {
		characterAnimator = GetNode<CharacterAnimator>("CharacterAnimator");
    }

	protected virtual void AnimateDeath() => characterAnimator.AnimateDeath();

	public override async void HurtAsync(int damage, Vector2 normal)
	{
		if (Health <= 0)
		{
			return;
		}

		base.HurtAsync(damage, normal);

		Health = Math.Max(0, Health - damage);

		EmitSignal(SignalName.Hurt, damage);

		if (Health <= 0)
		{
			AnimateDeath();

			await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

			QueueFree();
		}
	}

}
