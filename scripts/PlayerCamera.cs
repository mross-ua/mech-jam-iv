using Godot;
using System;
using MechJamIV;

public partial class PlayerCamera : Camera2D
	,ITracker
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

	#region ITracker

	public void TrackPlayer(Player p)
	{
		if (player != null)
		{
			throw new NotSupportedException("A player instance is already being tracked.");
		}

		player = p;

		healthBar.Value = player.Health;

		player.Injured += (damage) => healthBar.Value = player.Health;
		player.Healed += (health) => healthBar.Value = player.Health;

		player.ImmunityShieldActivated += () => immunityShield.Visible = true;
		player.ImmunityShieldDeactivated += () => immunityShield.Visible = false;

		player.SetRemoteTarget(this);
	}

	#endregion
}
