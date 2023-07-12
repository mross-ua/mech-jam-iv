using Godot;
using System;
using System.Collections.Generic;

namespace MechJamIV {
    public static class NodeHelper
    {

        public static async void TimedFree(this Node node, double timeSec, bool processAlways = true, bool processInPhysics = false, bool ignoreTimeScale = false)
        {
			await node.ToSignal(node.GetTree().CreateTimer(timeSec, processAlways, processInPhysics, ignoreTimeScale), SceneTreeTimer.SignalName.Timeout);

			node.QueueFree();
        }

    }
}