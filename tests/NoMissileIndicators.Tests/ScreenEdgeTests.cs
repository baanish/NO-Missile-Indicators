using System.Numerics;
using NoMissileIndicators.Missiles;
using static NoMissileIndicators.Tests.Program;

namespace NoMissileIndicators.Tests;

/// <summary>Where a missile's arrow goes on a 1920x1080 screen with a 60 degree vertical field of view.</summary>
internal static class ScreenEdgeTests
{
    public static readonly (string Name, Action Test)[] All =
    {
        ("a missile on screen gets no arrow", MissileOnScreenGetsNoArrow),
        ("a missile off the right gets the right edge", MissileOffRightGetsRightEdge),
        ("a missile behind on the left points left", MissileBehindLeftPointsLeft),
        ("a missile behind and above points up", MissileBehindAbovePointsUp),
        ("a missile dead astern points down", MissileDeadAsternPointsDown),
    };

    private static void MissileOnScreenGetsNoArrow() =>
        Expect(Pin(new Vector3(100, 0, 1000)) is null, "expected no arrow for a missile inside the view");

    private static void MissileOffRightGetsRightEdge() =>
        ExpectPin(Pin(new Vector3(2000, 0, 1000)), new Vector2(960, 0));

    /// <summary>Camera.WorldToScreenPoint mirrors a point behind the camera, which would put this arrow on the right.</summary>
    private static void MissileBehindLeftPointsLeft() =>
        ExpectPin(Pin(new Vector3(-500, 0, -1000)), new Vector2(-960, 0));

    private static void MissileBehindAbovePointsUp() =>
        ExpectPin(Pin(new Vector3(0, 300, -1000)), new Vector2(0, 540));

    private static void MissileDeadAsternPointsDown() =>
        ExpectPin(Pin(new Vector3(0, 0, -1000)), new Vector2(0, -540));

    private static Vector2? Pin(Vector3 cameraSpace) =>
        ScreenEdge.Pin(cameraSpace, new Vector2(960, 540), 540f / MathF.Tan(MathF.PI / 6f));

    private static void ExpectPin(Vector2? actual, Vector2 expected) =>
        Expect(actual is { } pin && Vector2.Distance(pin, expected) < 0.01f, $"expected an arrow at {expected}, got {actual?.ToString() ?? "none"}");
}
