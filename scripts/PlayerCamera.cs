using Godot;
using System;

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

	public void TrackPlayer(Player player)
	{
		if (this.player != null)
		{
			throw new NotSupportedException("A player instance is already being tracked.");
		}

		this.player = player;

		healthBar.Value = player.Health;

		player.Injured += (damage) => healthBar.Value = player.Health;
		player.Healed += (amount) => healthBar.Value = player.Health;
		player.ImmunityShieldActivated += () => immunityShield.Visible = true;
		player.ImmunityShieldDeactivated += () => immunityShield.Visible = false;

		player.RemoteTransform.RemotePath = GetPath();
	}

}
