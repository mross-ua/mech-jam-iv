using Godot;
using System;
using System.Collections.Generic;
using static Godot.Node;

namespace MechJamIV {
    public static class NodeHelper
    {

        public static async void TimedFree(this Node node, double timeSec, bool processAlways = false, bool processInPhysics = true)
        {
			await node.ToSignal(node.GetTree().CreateTimer(timeSec, processAlways, processInPhysics), SceneTreeTimer.SignalName.Timeout);

			node.QueueFree();
        }

        public static SignalAwaiter AddChildDeferred(this Node parent, Node node, bool forceReadableName = false, InternalMode @internal = InternalMode.Disabled)
        {
            // NOTE: This is a possible future Godot feature.
            //       See https://github.com/godotengine/godot-proposals/issues/3935

            parent.CallDeferred(Node.MethodName.AddChild, node, forceReadableName, (long)@internal);

            return node.ToSignal(node, Node.SignalName.Ready);
        }

    }
}