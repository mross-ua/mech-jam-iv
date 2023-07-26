
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MechJamIV {
    public static class TrackerHelper
    {

        public static Vector2 GetDirectionToTarget(this ITracker tracker)
        {
            Debug.Assert(tracker.Target != null, "A target is not currently being tracked.");

            return tracker.GlobalTransform.Origin.DirectionTo(tracker.Target.GlobalTransform.Origin);
        }

        public static bool IsTargetInFieldOfView(this ITracker tracker, Vector2 faceDirection, float fieldOfView)
        {
            return Mathf.RadToDeg(faceDirection.AngleTo(tracker.GetDirectionToTarget())) < fieldOfView;
        }

        public static bool IsTargetInLineOfSight(this ITracker tracker)
        {
            Debug.Assert(tracker.Target != null, "A target is not currently being tracked.");

            Vector2 from = tracker.GlobalTransform.Origin;
            Vector2 to = from + GetDirectionToTarget(tracker) * tracker.LineOfSightDistance;

            Godot.Collections.Dictionary collision = tracker.GetWorld2D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
            {
                From = from,
                To = to,
                Exclude = null,
                CollideWithBodies = true,
                CollideWithAreas = true,
                CollisionMask = (uint)tracker.LineOfSightMask
            });

            return collision.ContainsKey("collider") && collision["collider"].Obj == tracker.Target;
        }

    }
}
