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

		player.Injured += (damage) => UpdateUI();
		player.Healed += (health) => UpdateUI();

		player.ImmunityShieldActivated += () => immunityShield.Visible = true;
		player.ImmunityShieldDeactivated += () => immunityShield.Visible = false;

		player.WeaponManager.WeaponFired += (fireMode, weapon) => UpdateUI();

		player.SetRemoteTarget(this);

		UpdateUI();
	}

	private void UpdateUI()
	{
		healthBar.Value = player.Health;

		if (player.WeaponManager.PrimaryWeapon.Ammo < 0)
		{
			primaryAmmoLabel.Text = "∞";
		}
		else
		{
			primaryAmmoLabel.Text = player.WeaponManager.PrimaryWeapon.Ammo.ToString();
		}

		if (player.WeaponManager.SecondaryWeapon.Ammo < 0)
		{
			secondaryAmmoLabel.Text = "∞";
		}
		else
		{
			secondaryAmmoLabel.Text = player.WeaponManager.SecondaryWeapon.Ammo.ToString();
		}
	}

}
