using System;
using System.Numerics;

namespace NoMissileIndicators.Missiles;

/// <summary>
/// Places an indicator on a ring around the screen centre toward a missile. Works in camera space (X right, Y up,
/// Z forward) rather than from Camera.WorldToScreenPoint, which mirrors points behind the camera: an arrow built from
/// it, like the game's own target arrow, points away from a missile closing from behind.
/// </summary>
public static class ScreenEdge
{
    /// <summary>True when the point is in front of the camera and projects inside the screen.</summary>
    /// <param name="focalPixels">Half the screen height over the tangent of half the vertical field of view.</param>
    public static bool OnScreen(Vector3 cameraSpace, Vector2 halfScreen, float focalPixels)
    {
        if (cameraSpace.Z <= 0f)
        {
            return false;
        }

        var projected = new Vector2(cameraSpace.X, cameraSpace.Y) * (focalPixels / cameraSpace.Z);
        return Math.Abs(projected.X) <= halfScreen.X && Math.Abs(projected.Y) <= halfScreen.Y;
    }

    /// <summary>
    /// The indicator's offset from the screen centre in pixels: on the ring of <paramref name="ringRadius"/>, in the
    /// direction to turn toward the point, and never past the screen edge. A point dead ahead or astern gets the bottom.
    /// </summary>
    public static Vector2 Pin(Vector3 cameraSpace, Vector2 halfScreen, float ringRadius)
    {
        var across = new Vector2(cameraSpace.X, cameraSpace.Y);
        var direction = across.LengthSquared() < 1e-6f ? -Vector2.UnitY : Vector2.Normalize(across);
        var toSide = direction.X == 0f ? float.MaxValue : halfScreen.X / Math.Abs(direction.X);
        var toTopOrBottom = direction.Y == 0f ? float.MaxValue : halfScreen.Y / Math.Abs(direction.Y);
        return direction * Math.Min(ringRadius, Math.Min(toSide, toTopOrBottom));
    }
}
