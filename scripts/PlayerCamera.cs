using Godot;
using System;
using MechJamIV;

public partial class PlayerCamera : Camera2D
{

	#region Node references
	private Player player;

	private ProgressBar healthBar;
	private GpuParticles2D immunityShield;

	#endregion

	public override void _Ready()
	{
		healthBar = GetNode<ProgressBar>("UI/HealthBar");
		immunityShield = GetNode<GpuParticles2D>("UI/HealthBar/TextureRect/ImmunityShield");
	}

	public void Track(Player p)
	{
		if (player != null)
		{
			// NOTE: We throw an exception so we don't have to figure
			//       out if we need to unregister event handlers from
			//       a previously tracked target.

			throw new NotSupportedException("A target is already being tracked.");
		}

		player = p;

		healthBar.Value = player.Health;

		player.Injured += (damage) => healthBar.Value = player.Health;
		player.Healed += (health) => healthBar.Value = player.Health;

		player.ImmunityShieldActivated += () => immunityShield.Visible = true;
		player.ImmunityShieldDeactivated += () => immunityShield.Visible = false;

		player.SetRemoteTarget(this);
	}

}
