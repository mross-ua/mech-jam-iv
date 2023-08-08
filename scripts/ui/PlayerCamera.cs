using Godot;
using System;
using MechJamIV;

public partial class PlayerCamera : Camera2D
{

    #region Node references

    private Player player;

    private GpuParticles2D immunityShield;
    private ProgressBar healthBar;
    private ProgressBar overHealthBar;

    private TextureRect primaryTextureRect;
    private Label primaryAmmoLabel;

    private TextureRect secondaryTextureRect;
    private Label secondaryAmmoLabel;

    #endregion

    public override void _Ready()
    {
        immunityShield = GetNode<GpuParticles2D>("UI/Control/TextureRect/ImmunityShield");
        healthBar = GetNode<ProgressBar>("UI/Control/HealthBar");
        overHealthBar = GetNode<ProgressBar>("UI/Control/OverHealthBar");

        primaryTextureRect = GetNode<TextureRect>("UI/Control2/PrimaryTextureRect");
        primaryAmmoLabel = GetNode<Label>("UI/Control2/PrimaryAmmoLabel");

        secondaryTextureRect = GetNode<TextureRect>("UI/Control3/SecondaryTextureRect");
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

        player.WeaponManager.WeaponUpdated += (weapon) => UpdateUI();

        player.SetRemoteTarget(this);

        UpdateUI();
    }

    private void UpdateUI()
    {
        healthBar.MaxValue = player.MaxHealth;
        // we want the overhealth bar to have the same scale
        overHealthBar.MaxValue = player.MaxHealth;

        healthBar.Value = Math.Max(player.MaxHealth, player.Health);
        overHealthBar.Value = player.Health - player.MaxHealth;

        primaryTextureRect.Texture = player.WeaponManager.PrimaryWeapon?.UISprite;
        secondaryTextureRect.Texture = player.WeaponManager.SecondaryWeapon?.UISprite;

        if (player.WeaponManager.PrimaryWeapon?.Ammo < 0)
        {
            primaryAmmoLabel.Text = "∞";
        }
        else
        {
            primaryAmmoLabel.Text = player.WeaponManager.PrimaryWeapon?.Ammo.ToString();
        }

        if (player.WeaponManager.SecondaryWeapon?.Ammo < 0)
        {
            secondaryAmmoLabel.Text = "∞";
        }
        else
        {
            secondaryAmmoLabel.Text = player.WeaponManager.SecondaryWeapon?.Ammo.ToString();
        }
    }

}
