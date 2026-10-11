using System.Numerics;
using NoMissileIndicators.Missiles;
using static NoMissileIndicators.Tests.Program;

namespace NoMissileIndicators.Tests;

/// <summary>What a Project Wingman plate looks like from the cockpit, with the sphere 300 px across on screen.</summary>
internal static class WingmanSphereTests
{
    private const float Ring = 300f;

    public static readonly (string Name, Action Test)[] All =
    {
        ("a missile behind shows its red face, large, below the middle", MissileBehindShowsRedFaceLarge),
        ("a missile ahead shows its yellow back, small, above the middle", MissileAheadShowsYellowBackSmall),
        ("a missile to the right is a slanted plate on the right", MissileRightIsThinPlateOnRight),
        ("a missile to the left is a slanted plate on the left", MissileLeftIsThinPlateOnLeft),
        ("a missile to the right has yellow corner bars running in", MissileRightHasYellowBarsPointingIn),
        ("a missile straight above still gets a plate at the top", MissileAboveGetsPlateAtTop),
    };

    private static void MissileBehindShowsRedFaceLarge()
    {
        var behind = Build(new Vector3(0, 0, -1000));
        var ahead = Build(new Vector3(0, 0, 1000));
        Expect(behind.Any(s => s.Part == WingmanPart.Front) && behind.All(s => s.Part != WingmanPart.Back), "expected the red front seen and the yellow back hidden");
        var centre = Centroid(Part(behind, WingmanPart.Front));
        Expect(MathF.Abs(centre.X) < 1f && centre.Y < -0.2f * Ring, $"expected the plate below the middle, got {centre}");
        Expect(Width(Part(behind, WingmanPart.Front)) > 1.5f * Width(Plate(ahead)), "expected a plate behind to look much larger than one ahead");
    }

    private static void MissileAheadShowsYellowBackSmall()
    {
        var ahead = Build(new Vector3(0, 0, 1000));
        Expect(ahead.Any(s => s.Part == WingmanPart.Back) && ahead.All(s => s.Part != WingmanPart.Front), "expected the yellow back seen and the red front hidden");
        var centre = Centroid(Part(ahead, WingmanPart.Back));
        Expect(MathF.Abs(centre.X) < 1f && centre.Y > 0.2f * Ring, $"expected the plate above the middle, got {centre}");
    }

    private static void MissileRightIsThinPlateOnRight()
    {
        var face = Plate(Build(new Vector3(1000, 0, 0)));
        var centre = Centroid(face);
        Expect(MathF.Abs(centre.X - Ring) < 0.1f * Ring && MathF.Abs(centre.Y) < 1f, $"expected the plate at the right of the ring, got {centre}");
        Expect(Width(face) < 0.9f * Height(face), $"expected a plate seen at a slant (face on is 1.15 wide per tall), got {Width(face)} wide by {Height(face)} tall");
    }

    private static void MissileLeftIsThinPlateOnLeft()
    {
        var face = Plate(Build(new Vector3(-1000, 0, 0)));
        Expect(Centroid(face).X < -0.9f * Ring, $"expected the plate at the left of the ring, got {Centroid(face)}");
        Expect(Width(face) < 0.9f * Height(face), "expected a plate seen at a slant");
    }

    private static void MissileRightHasYellowBarsPointingIn()
    {
        var shapes = Build(new Vector3(1000, 0, 0));
        var bars = shapes.Where(s => s.Part == WingmanPart.Bar).ToList();
        Expect(bars.Count >= 3 && bars.All(b => b.Paint == WingmanPaint.Yellow), "expected the yellow faces of three corner bars");
        var barsX = bars.Average(b => (b.A + b.B + b.C + b.D).X / 4f);
        Expect(barsX < Centroid(Plate(shapes)).X, "expected the bars of a plate on the right to run in toward the screen centre");
    }

    private static void MissileAboveGetsPlateAtTop()
    {
        var face = Plate(Build(new Vector3(0, 1000, 0)));
        Expect(Centroid(face).Y > 0.5f * Ring && !float.IsNaN(face.A.X), $"expected a plate at the top, got {Centroid(face)}");
    }

    private static List<WingmanShape> Build(Vector3 cameraSpace)
    {
        var shapes = new List<WingmanShape>();
        WingmanSphere.Build(cameraSpace, Ring, shapes);
        return shapes;
    }

    private static WingmanShape Part(List<WingmanShape> shapes, WingmanPart part) => shapes.First(s => s.Part == part);
    /// <summary>Whichever of the plate's two triangles the camera sees.</summary>
    private static WingmanShape Plate(List<WingmanShape> shapes) => shapes.First(s => s.Part is WingmanPart.Front or WingmanPart.Back);
    private static Vector2 Centroid(WingmanShape t) => (t.A + t.B + t.C) / 3f;
    private static float Width(WingmanShape t) => new[] { t.A.X, t.B.X, t.C.X }.Max() - new[] { t.A.X, t.B.X, t.C.X }.Min();
    private static float Height(WingmanShape t) => new[] { t.A.Y, t.B.Y, t.C.Y }.Max() - new[] { t.A.Y, t.B.Y, t.C.Y }.Min();
}
