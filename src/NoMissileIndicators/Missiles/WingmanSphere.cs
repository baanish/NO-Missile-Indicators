using System;
using System.Collections.Generic;
using System.Numerics;

namespace NoMissileIndicators.Missiles;

/// <summary>Which part of a Project Wingman plate a face belongs to.</summary>
public enum WingmanPart
{
    /// <summary>The plate's triangle toward the missile.</summary>
    Front,

    /// <summary>The plate's triangle away from the missile.</summary>
    Back,

    /// <summary>One of the plate's three edge walls.</summary>
    Side,

    /// <summary>A face of a corner bar.</summary>
    Bar,
}

/// <summary>The two colours of a plate: the red of the side toward the missile, the yellow of its back and corner bars.</summary>
public enum WingmanPaint
{
    Red,
    Yellow,
}

/// <summary>
/// One face of a plate the camera sees, in pixels from the screen centre (Y up): a triangle (A, B, C) or a quad (A, B, C,
/// D). Shade is how lit it is, from about 0.5 to 1. Depth is its distance ahead of the camera, for drawing far faces first
/// and for scaling line widths.
/// </summary>
public readonly struct WingmanShape
{
    public WingmanShape(WingmanPart part, WingmanPaint paint, float shade, float depth, Vector2 a, Vector2 b, Vector2 c, Vector2? d = null)
    {
        Part = part;
        Paint = paint;
        Shade = shade;
        Depth = depth;
        A = a;
        B = b;
        C = c;
        D = d ?? c;
        Corners = d is null ? 3 : 4;
    }

    public WingmanPart Part { get; }
    public WingmanPaint Paint { get; }
    public float Shade { get; }
    public float Depth { get; }
    public Vector2 A { get; }
    public Vector2 B { get; }
    public Vector2 C { get; }
    public Vector2 D { get; }
    public int Corners { get; }
}

/// <summary>
/// Project Wingman's missile indicator: a solid triangular plate on a sphere around you, at the point toward the missile
/// and turned to face it, red on the side facing the missile and yellow behind, with a yellow bar standing out of the back
/// at each corner, square to the plate, into the sphere. The sphere sits just ahead of the camera and is seen from a little
/// above, as Project Wingman's chase camera sees its sphere around the aircraft, so its perspective reads from the cockpit
/// too: a missile dead astern shows a large red triangle below the middle, one dead ahead a small yellow one above it, and one to
/// the side a plate seen at a slant at that side. Only faces turned toward the camera are kept, each lit from above and in
/// front. Units are the distance from the camera to the sphere's centre.
/// </summary>
public static class WingmanSphere
{
    /// <summary>The sphere's radius. Smaller flattens the perspective; larger exaggerates it.</summary>
    public const float Radius = 0.25f;

    /// <summary>
    /// How far above the sphere the view sits, as Project Wingman's chase camera looks down on its sphere: a missile dead
    /// astern shows below the middle, one dead ahead above it, and plates facing you are seen at enough of a slant to show
    /// their bars.
    /// </summary>
    private const float ViewPitchDegrees = 20f;

    private const float PlateRadius = 0.025f;

    /// <summary>The plate's thickness, and a corner bar's length, width and depth, as fractions of the plate's radius.</summary>
    private const float PlateThickness = 0.22f;
    private const float BarLength = 0.34f;
    private const float BarWidth = 0.13f;
    private const float BarDepth = 0.13f;

    /// <summary>
    /// How far each plate turns its back toward the camera. Seen from the camera, a plate beside you would face it edge on;
    /// this turns it enough to read, as Project Wingman's chase camera sees its plates from above and behind.
    /// </summary>
    private const float Tilt = 0.5f;

    /// <summary>Faces turned away from the light keep this much brightness; faces turned to it get the rest.</summary>
    private const float Ambient = 0.55f;

    private static readonly Matrix4x4 ViewPitch = Matrix4x4.CreateRotationX(-ViewPitchDegrees * (MathF.PI / 180f));

    /// <summary>From above, a little left, and toward the camera.</summary>
    private static readonly Vector3 Light = Vector3.Normalize(new Vector3(-0.35f, 0.8f, -0.5f));

    /// <summary>
    /// Adds the faces the camera sees of the plate toward a missile at <paramref name="cameraSpace"/> (X right, Y up,
    /// Z forward), scaled so a plate beside you sits <paramref name="ringPixels"/> from the screen centre.
    /// </summary>
    public static void Build(Vector3 cameraSpace, float ringPixels, List<WingmanShape> into)
    {
        if (cameraSpace.LengthSquared() < 1e-6f)
        {
            return;
        }

        var toward = Vector3.Transform(Vector3.Normalize(cameraSpace), ViewPitch);
        var centre = Vector3.UnitZ + toward * Radius;
        var normal = Vector3.Normalize(toward + Vector3.Normalize(centre) * Tilt);

        // The triangle's point is up, except on a plate straight above or below, where up lies along its normal.
        var up = Vector3.UnitY - normal * normal.Y;
        if (up.LengthSquared() < 1e-4f)
        {
            up = Vector3.UnitZ - normal * normal.Z;
        }

        up = Vector3.Normalize(up) * PlateRadius;
        var side = Vector3.Cross(normal, up) * 0.8660254f;
        var corners = new[] { up, side - up * 0.5f, -side - up * 0.5f };
        var scale = ringPixels / Radius;

        var halfThickness = normal * (PlateThickness * PlateRadius * 0.5f);
        var front = new Vector3[3];
        var back = new Vector3[3];
        for (var i = 0; i < 3; i++)
        {
            front[i] = centre + corners[i] + halfThickness;
            back[i] = centre + corners[i] - halfThickness;
        }

        Face(into, WingmanPart.Front, WingmanPaint.Red, normal, scale, front[0], front[1], front[2]);
        Face(into, WingmanPart.Back, WingmanPaint.Yellow, -normal, scale, back[0], back[1], back[2]);
        for (var i = 0; i < 3; i++)
        {
            var next = (i + 1) % 3;
            var outward = Vector3.Normalize(corners[i] + corners[next]);
            Face(into, WingmanPart.Side, WingmanPaint.Red, outward, scale, front[i], front[next], back[next], back[i]);
        }

        foreach (var corner in corners)
        {
            var outward = corner / PlateRadius;
            var across = Vector3.Cross(normal, outward);
            var top = centre + corner - halfThickness;
            Cuboid(into, top, top - normal * (BarLength * PlateRadius), across * (BarWidth * PlateRadius * 0.5f),
                outward * (BarDepth * PlateRadius * 0.5f), scale);
        }
    }

    /// <summary>A bar from <paramref name="top"/> to <paramref name="bottom"/>, its cross-section set by two half-extents.</summary>
    private static void Cuboid(List<WingmanShape> into, Vector3 top, Vector3 bottom, Vector3 halfAcross, Vector3 halfOut, float scale)
    {
        var length = Vector3.Normalize(top - bottom);
        var across = Vector3.Normalize(halfAcross);
        var outward = Vector3.Normalize(halfOut);
        Vector3 At(Vector3 end, float sa, float so) => end + halfAcross * sa + halfOut * so;

        Face(into, WingmanPart.Bar, WingmanPaint.Yellow, across, scale, At(top, 1, -1), At(top, 1, 1), At(bottom, 1, 1), At(bottom, 1, -1));
        Face(into, WingmanPart.Bar, WingmanPaint.Yellow, -across, scale, At(top, -1, 1), At(top, -1, -1), At(bottom, -1, -1), At(bottom, -1, 1));
        Face(into, WingmanPart.Bar, WingmanPaint.Yellow, outward, scale, At(top, 1, 1), At(top, -1, 1), At(bottom, -1, 1), At(bottom, 1, 1));
        Face(into, WingmanPart.Bar, WingmanPaint.Yellow, -outward, scale, At(top, -1, -1), At(top, 1, -1), At(bottom, 1, -1), At(bottom, -1, -1));
        Face(into, WingmanPart.Bar, WingmanPaint.Yellow, length, scale, At(top, 1, 1), At(top, 1, -1), At(top, -1, -1), At(top, -1, 1));
        Face(into, WingmanPart.Bar, WingmanPaint.Yellow, -length, scale, At(bottom, 1, -1), At(bottom, 1, 1), At(bottom, -1, 1), At(bottom, -1, -1));
    }

    /// <summary>Adds a face if it's turned toward the camera, which sits at the origin.</summary>
    private static void Face(List<WingmanShape> into, WingmanPart part, WingmanPaint paint, Vector3 normal, float scale,
        Vector3 a, Vector3 b, Vector3 c, Vector3? d = null)
    {
        var middle = d is { } last ? (a + b + c + last) / 4f : (a + b + c) / 3f;
        if (Vector3.Dot(normal, middle) >= 0f)
        {
            return;
        }

        var shade = Ambient + (1f - Ambient) * MathF.Max(0f, Vector3.Dot(normal, Light));
        into.Add(new WingmanShape(part, paint, shade, middle.Z, Project(a, scale), Project(b, scale), Project(c, scale),
            d is { } corner ? Project(corner, scale) : null));
    }

    /// <summary>The sphere spans 0.75 to 1.25 ahead of the camera, so Z is never near zero.</summary>
    private static Vector2 Project(Vector3 point, float scale) => new Vector2(point.X, point.Y) * (scale / point.Z);
}
