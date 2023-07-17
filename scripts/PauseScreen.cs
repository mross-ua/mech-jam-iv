using Godot;
using System;

public partial class PauseScreen : CanvasLayer
{

	[Signal]
	public delegate void ContinueClickedEventHandler();
	[Signal]
	public delegate void RestartClickedEventHandler();
	[Signal]
	public delegate void QuitClickedEventHandler();

	#region Node references

	private Button continueButton;
	private Button restartButton;
	private Button quitButton;

	#endregion

	public override void _Ready()
	{
		continueButton = GetNode<Button>("VBoxContainer/ContinueButton");
		continueButton.Pressed += () => EmitSignal(SignalName.ContinueClicked);

		restartButton = GetNode<Button>("VBoxContainer/RestartButton");
		restartButton.Pressed += () => EmitSignal(SignalName.RestartClicked);

		quitButton = GetNode<Button>("VBoxContainer/QuitButton");
		quitButton.Pressed += () => EmitSignal(SignalName.QuitClicked);
	}

}
