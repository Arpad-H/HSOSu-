using UnityEngine;

public class PerfectCircleCurve : ICurve
{
    private readonly Vector3[] _controlPoints;
    private float _arcLength;
    private const int NumCirclePoints = 100;
    private const float FullCircleDegrees = 360f;

    public PerfectCircleCurve(Vector3[] controlPoints, float arcLength)
    {
        _controlPoints = controlPoints;
        _arcLength = arcLength;
    }

    public void Draw(LineRenderer lineRenderer)
    {
        if (_controlPoints.Length < 3) return;

        var center = (_controlPoints[0] + _controlPoints[1] + _controlPoints[2]) / 3f;
        var radius = Vector3.Distance(_controlPoints[0], center);
        
        int pointCount = Mathf.CeilToInt(NumCirclePoints * (_arcLength / FullCircleDegrees));
        pointCount = Mathf.Max(2, pointCount); // mindestens zwei Punkte

        Vector3[] positions = new Vector3[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * (_arcLength * Mathf.Deg2Rad) / (pointCount - 1);
            positions[i] = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y + Mathf.Sin(angle) * radius,
                center.z); // Für X-Y-Plane in 2D
        }

        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(positions);
    }

    public Vector3 GetPointAtTime(float t)
    {
        if (_controlPoints.Length < 3) return _controlPoints[0];  // Invalid circle

        // Calculate the center of the circle and the radius
        var center = CalculateCircleCenter(_controlPoints[0], _controlPoints[1], _controlPoints[2]);
        var radius = Vector3.Distance(_controlPoints[0], center);

        // Calculate the total angle for the arc (in degrees)
        float totalArcAngle = _arcLength;

        // Calculate the angle at the given time t (0 <= t <= 1)
        float angleAtTime = Mathf.Lerp(0, totalArcAngle, t);
        float angleInRadians = angleAtTime * Mathf.Deg2Rad;

        // Calculate the point on the circle at this angle (assuming XY plane)
        return new Vector3(
            center.x + Mathf.Cos(angleInRadians) * radius,
            center.y + Mathf.Sin(angleInRadians) * radius,
            center.z); // For the X-Y plane, Z remains constant
    }

    private Vector3 CalculateCircleCenter(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Berechne den Mittelpunkt des Kreises durch die drei Punkte mittels Umkreismethode
        Vector3 mid1 = (p1 + p2) / 2;
        Vector3 mid2 = (p2 + p3) / 2;

        Vector3 dir1 = Vector3.Cross(p2 - p1, Vector3.forward).normalized;
        Vector3 dir2 = Vector3.Cross(p3 - p2, Vector3.forward).normalized;

        float a1 = mid1.y - mid2.y;
        float b1 = mid2.x - mid1.x;
        float c1 = a1 * mid1.x + b1 * mid1.y;

        float a2 = dir2.y;
        float b2 = -dir2.x;
        float c2 = a2 * mid2.x + b2 * mid2.y;

        float det = a1 * b2 - a2 * b1;

        if (Mathf.Abs(det) < Mathf.Epsilon)
        {
            return Vector3.zero; // Wenn Punkte kollinear sind, gib ein Nullvektor zurück
        }

        float x = (b2 * c1 - b1 * c2) / det;
        float y = (a1 * c2 - a2 * c1) / det;

        return new Vector3(x, y, 0);
    }
}