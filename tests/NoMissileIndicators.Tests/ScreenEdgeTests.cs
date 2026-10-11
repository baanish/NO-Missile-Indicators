using System.Numerics;
using NoMissileIndicators.Missiles;
using static NoMissileIndicators.Tests.Program;

namespace NoMissileIndicators.Tests;

/// <summary>Where a missile's indicator goes on a 1920x1080 screen with a 60 degree vertical field of view.</summary>
internal static class ScreenEdgeTests
{
    private const float Ring = 380f;
    private const float Focal = 935.3f; // 540 / tan(30 deg)

    public static readonly (string Name, Action Test)[] All =
    {
        ("a missile on screen is on screen", MissileOnScreenIsOnScreen),
        ("a missile behind is never on screen", MissileBehindIsNotOnScreen),
        ("a missile off the right gets the right of the ring", MissileOffRightGetsRightOfRing),
        ("a missile behind on the left points left", MissileBehindLeftPointsLeft),
        ("a missile behind and above points up", MissileBehindAbovePointsUp),
        ("a missile dead astern points down", MissileDeadAsternPointsDown),
        ("a ring larger than the screen stops at the edge", LargeRingStopsAtEdge),
    };

    private static void MissileOnScreenIsOnScreen()
    {
        Expect(ScreenEdge.OnScreen(new Vector3(100, 0, 1000), new Vector2(960, 540), Focal), "expected a missile inside the view on screen");
        Expect(!ScreenEdge.OnScreen(new Vector3(2000, 0, 1000), new Vector2(960, 540), Focal), "expected a missile off the right off screen");
    }

    /// <summary>Camera.WorldToScreenPoint mirrors a point behind the camera into the view.</summary>
    private static void MissileBehindIsNotOnScreen() =>
        Expect(!ScreenEdge.OnScreen(new Vector3(10, 0, -1000), new Vector2(960, 540), Focal), "expected a missile behind off screen");

    private static void MissileOffRightGetsRightOfRing() =>
        ExpectPin(Pin(new Vector3(2000, 0, 1000)), new Vector2(Ring, 0));

    /// <summary>Camera.WorldToScreenPoint mirrors a point behind the camera, which would put this arrow on the right.</summary>
    private static void MissileBehindLeftPointsLeft() =>
        ExpectPin(Pin(new Vector3(-500, 0, -1000)), new Vector2(-Ring, 0));

    private static void MissileBehindAbovePointsUp() =>
        ExpectPin(Pin(new Vector3(0, 300, -1000)), new Vector2(0, Ring));

    private static void MissileDeadAsternPointsDown() =>
        ExpectPin(Pin(new Vector3(0, 0, -1000)), new Vector2(0, -Ring));

    private static void LargeRingStopsAtEdge()
    {
        ExpectPin(ScreenEdge.Pin(new Vector3(0, 300, -1000), new Vector2(960, 540), 2000f), new Vector2(0, 540));
        ExpectPin(ScreenEdge.Pin(new Vector3(2000, 0, 1000), new Vector2(960, 540), 2000f), new Vector2(960, 0));
    }

    private static Vector2 Pin(Vector3 cameraSpace) => ScreenEdge.Pin(cameraSpace, new Vector2(960, 540), Ring);

    private static void ExpectPin(Vector2 actual, Vector2 expected) =>
        Expect(Vector2.Distance(actual, expected) < 0.01f, $"expected an indicator at {expected}, got {actual}");
}
