using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NumericsVector2 = System.Numerics.Vector2;

namespace NoMissileIndicators.Missiles;

/// <summary>
/// Draws every Project Wingman plate (<see cref="WingmanSphere"/>) as one mesh on the HUD canvas, far faces first so
/// near ones cover them, each darkened by its shade. Styled after Project Wingman's: a plate's big triangle glows softly
/// around its edge, is filled a little denser toward its rim, and carries a thin light outline; edge walls and corner bars
/// are solid, told apart by their shade. Widths are in pixels at 1080p and grow as a plate comes nearer. Each plate is
/// about 60 vertices, so six missiles cost well under a tenth of a millisecond to mesh.
/// </summary>
internal sealed class WingmanSphereGraphic : MaskableGraphic
{
    private const float GlowWidth = 4f;
    private const float GlowAlpha = 0.35f;
    private const float FillCentreAlpha = 0.75f;
    private const float FillRimAlpha = 0.92f;
    private const float EdgeWidth = 1.2f;
    private const float SolidAlpha = 0.95f;

    /// <summary>How far toward white the outline is.</summary>
    private const float Light = 0.55f;

    private readonly List<(WingmanShape Shape, float Alpha)> _shapes = new();
    private readonly Vector2[] _points = new Vector2[4];

    /// <summary>Read once per mesh: each Unity property or transform call from a plugin costs more than the math around it.</summary>
    private Matrix4x4 _toLocal;
    private Vector2 _screenCentre;

    public Color Red { get; set; } = Color.red;
    public Color Yellow { get; set; } = Color.yellow;

    /// <summary>Screen height over 1080, so widths match at any resolution.</summary>
    public float PixelScale { get; set; } = 1f;

    public void Clear() => _shapes.Clear();

    /// <summary>One missile's plate; <paramref name="alpha"/> fades it as the game fades an outdated marker.</summary>
    public void Add(List<WingmanShape> shapes, float alpha)
    {
        foreach (var shape in shapes)
        {
            _shapes.Add((shape, alpha));
        }
    }

    public void Redraw()
    {
        _shapes.Sort((a, b) => b.Shape.Depth.CompareTo(a.Shape.Depth));
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        _toLocal = rectTransform.worldToLocalMatrix;
        _screenCentre = new Vector2(UnityEngine.Screen.width * 0.5f, UnityEngine.Screen.height * 0.5f);
        foreach (var (shape, alpha) in _shapes)
        {
            var count = shape.Corners;
            _points[0] = Screen(shape.A);
            _points[1] = Screen(shape.B);
            _points[2] = Screen(shape.C);
            _points[3] = Screen(shape.D);
            var paint = shape.Paint == WingmanPaint.Red ? Red : Yellow;
            paint = new Color(paint.r * shape.Shade, paint.g * shape.Shade, paint.b * shape.Shade, paint.a * alpha);
            var px = PixelScale / shape.Depth;
            var light = Color.Lerp(paint, new Color(1f, 1f, 1f, paint.a), Light);

            if (shape.Part is WingmanPart.Side or WingmanPart.Bar)
            {
                Polygon(vh, count, Alpha(paint, SolidAlpha));
            }
            else
            {
                Ring(vh, count, 0f, GlowWidth * px, Alpha(paint, GlowAlpha), Alpha(paint, 0f));
                Fan(vh, count, Alpha(paint, FillCentreAlpha), Alpha(paint, FillRimAlpha));
                Ring(vh, count, -EdgeWidth * 0.5f * px, EdgeWidth * 0.5f * px, light, light);
            }
        }
    }

    private static Color Alpha(Color color, float factor)
    {
        color.a *= factor;
        return color;
    }

    private Vector2 Screen(NumericsVector2 offset) => new(_screenCentre.x + offset.X, _screenCentre.y + offset.Y);

    private Vector2 Centroid(int count)
    {
        var sum = Vector2.zero;
        for (var i = 0; i < count; i++)
        {
            sum += _points[i];
        }

        return sum / count;
    }

    private void Polygon(VertexHelper vh, int count, Color color)
    {
        var start = vh.currentVertCount;
        for (var i = 0; i < count; i++)
        {
            vh.AddVert(Local(_points[i]), color, Vector2.zero);
        }

        for (var i = 1; i < count - 1; i++)
        {
            vh.AddTriangle(start, start + i, start + i + 1);
        }
    }

    /// <summary>Filled from the centroid out, so the rim can be denser than the middle.</summary>
    private void Fan(VertexHelper vh, int count, Color centre, Color rim)
    {
        var start = vh.currentVertCount;
        vh.AddVert(Local(Centroid(count)), centre, Vector2.zero);
        for (var i = 0; i < count; i++)
        {
            vh.AddVert(Local(_points[i]), rim, Vector2.zero);
        }

        for (var i = 0; i < count; i++)
        {
            vh.AddTriangle(start, start + 1 + i, start + 1 + (i + 1) % count);
        }
    }

    /// <summary>
    /// A band along the outline from <paramref name="inner"/> to <paramref name="outer"/> pixels outside it (negative is
    /// inside), with mitred corners, coloured from <paramref name="innerColor"/> to <paramref name="outerColor"/>.
    /// </summary>
    private void Ring(VertexHelper vh, int count, float inner, float outer, Color innerColor, Color outerColor)
    {
        var area = 0f;
        for (var i = 0; i < count; i++)
        {
            var next = _points[(i + 1) % count];
            area += _points[i].x * next.y - next.x * _points[i].y;
        }

        var winding = area >= 0f ? 1f : -1f;
        var start = vh.currentVertCount;
        for (var i = 0; i < count; i++)
        {
            var miter = Miter(_points[(i + count - 1) % count], _points[i], _points[(i + 1) % count], winding);
            vh.AddVert(Local(_points[i] + miter * inner), innerColor, Vector2.zero);
            vh.AddVert(Local(_points[i] + miter * outer), outerColor, Vector2.zero);
        }

        for (var i = 0; i < count; i++)
        {
            var a = start + i * 2;
            var b = start + (i + 1) % count * 2;
            vh.AddTriangle(a, a + 1, b + 1);
            vh.AddTriangle(a, b + 1, b);
        }
    }

    /// <summary>The outward offset at a corner for a band one pixel wide, its length capped so sharp corners stay tidy.</summary>
    private static Vector2 Miter(Vector2 previous, Vector2 corner, Vector2 next, float winding)
    {
        var before = Outward(previous, corner, winding);
        var after = Outward(corner, next, winding);
        var bisector = (before + after).normalized;
        var cos = Vector2.Dot(bisector, after);
        return bisector.sqrMagnitude < 1e-6f ? after : bisector / Mathf.Max(cos, 0.35f);
    }

    private static Vector2 Outward(Vector2 from, Vector2 to, float winding)
    {
        var along = (to - from).normalized;
        return new Vector2(along.y, -along.x) * winding;
    }

    /// <summary>The HUD canvas is a screen-space overlay, where world positions are screen pixels.</summary>
    private Vector3 Local(Vector2 screen) => _toLocal.MultiplyPoint3x4(screen);
}
