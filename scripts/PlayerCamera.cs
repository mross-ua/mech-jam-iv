using Godot;
using System;
using MechJamIV;

public partial class PlayerCamera : Camera2D
{

	#region Node references

	private Player player;

	private GpuParticles2D immunityShield;
	private ProgressBar healthBar;

	private Label primaryAmmoLabel;
	private Label secondaryAmmoLabel;

	#endregion

	public override void _Ready()
	{
		immunityShield = GetNode<GpuParticles2D>("UI/Control/TextureRect/ImmunityShield");
		healthBar = GetNode<ProgressBar>("UI/Control/HealthBar");

		primaryAmmoLabel = GetNode<Label>("UI/Control2/PrimaryAmmoLabel");
		secondaryAmmoLabel = GetNode<Label>("UI/Control3/SecondaryAmmoLabel");
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

		player.WeaponManager.WeaponFired += (fireMode, ammoRemaining) =>
		{
			switch (fireMode)
			{
				case FireMode.Primary:
				case FireMode.PrimarySustained:
					if (ammoRemaining == -1)
					{
						primaryAmmoLabel.Text = "∞";
					}
					else
					{
						primaryAmmoLabel.Text = ammoRemaining.ToString();
					}

					break;
				case FireMode.Secondary:
					if (ammoRemaining == -1)
					{
						secondaryAmmoLabel.Text = "∞";
					}
					else
					{
						secondaryAmmoLabel.Text = ammoRemaining.ToString();
					}

					break;
			}
		};

		player.SetRemoteTarget(this);
	}

}
