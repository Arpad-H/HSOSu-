using UnityEngine;

public class PerfectCircleCurve : ICurve
{
    private readonly Vector3[] _controlPoints;
    private readonly float _arcLength; // Die Länge des Kreisbogens in Pixeln
    private const int k_NumCirclePoints = 100;
    private const float k_FullCircleDegrees = 360f;

    public PerfectCircleCurve(Vector3[] controlPoints, float arcLength)
    {
        _controlPoints = controlPoints;
        _arcLength = arcLength;
    }

    public void Draw(LineRenderer lineRenderer)
    {
        if (_controlPoints.Length < 3) return;

        // Mittelpunkt und Radius des Kreises berechnen
        var center = (_controlPoints[0] + _controlPoints[1] + _controlPoints[2]) / 3f;
        var radius = Vector3.Distance(_controlPoints[0], center);

        // Berechnung des Winkels in Grad, der der gegebenen Bogenlänge entspricht
        // Bogenlänge = Radius * Winkel (in Radians), also Winkel = Bogenlänge / Radius
        float arcAngleRadians = _arcLength / radius;
        float arcAngleDegrees = arcAngleRadians * Mathf.Rad2Deg;

        // Anzahl der Punkte entlang des Bogens berechnen
        int pointCount = Mathf.CeilToInt(k_NumCirclePoints * (arcAngleDegrees / k_FullCircleDegrees));
        pointCount = Mathf.Max(2, pointCount); // mindestens zwei Punkte

        Vector3[] positions = new Vector3[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            float angle = (i / (float)(pointCount - 1)) * arcAngleRadians; // Winkel in Radians
            positions[i] = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y + Mathf.Sin(angle) * radius,
                center.z // Optional für z-Koordinate, falls 2D
            );
        }

        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(positions);
    }

    public Vector3 GetPointAtTime(float t)
    {
        if (_controlPoints.Length < 3) return _controlPoints[0]; // Invalid circle

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

    Vector3 CalculateCircleCenter(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Vektorunterschiede
        var midAB = (p1 + p2) / 2f;
        var midBC = (p2 + p3) / 2f;

        // Berechnung der Richtungsvektoren
        var dirAB = new Vector3(-(p2.z - p1.z), 0, p2.x - p1.x);
        var dirBC = new Vector3(-(p3.z - p2.z), 0, p3.x - p2.x);

        // Kontrolliere für Parallelität
        if (Vector3.Cross(p2 - p1, p3 - p2).magnitude < Mathf.Epsilon)
        {
            throw new System.ArgumentException("Die gegebenen Punkte liegen auf einer Linie.");
        }

        // Berechnung der Kreuzungspunkte (Orthonormalprojektion auf die Richtungsvektoren)
        Vector3 a = midAB + dirAB.normalized;
        Vector3 b = midBC + dirBC.normalized;
        Vector3 abDirection = b - a;
        Vector3 center;

        float t = Vector3.Dot(midBC - midAB, abDirection) / abDirection.magnitude;
        center = midAB + dirAB.normalized * t;

        return center;
    }
}