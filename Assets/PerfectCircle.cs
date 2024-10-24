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
        CalculateCircle(_controlPoints[0], _controlPoints[1], _controlPoints[2], out var center, out var radius);

        float startAngle = Mathf.Atan2(_controlPoints[0].y - center.y, _controlPoints[0].x - center.x);
        float endAngle = Mathf.Atan2(_controlPoints[2].y - center.y, _controlPoints[2].x - center.x);

        // Winkel zwischen Start- und Endpunkt
        float angleBetween = Mathf.Abs(endAngle - startAngle);
        if (angleBetween > Mathf.PI) // Wenn der Winkel mehr als π ist, in den anderen Richtungen umkreisen
        {
            angleBetween = 2 * Mathf.PI - angleBetween;
        }

        // Maximale Winkelverschiebung aufgrund von arcLength
        float maxArcAngle = _arcLength / radius; // θ = s / r

        // Limitiere tatsächlich verwendeten Winkelbase
        float actualAngle = Mathf.Min(angleBetween, maxArcAngle);

        int segmentCount = 100; // Anzahl der Segmente für den Kreisbogen
        lineRenderer.positionCount = segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            float angle = startAngle + t * actualAngle * Mathf.Sign(endAngle - startAngle);

            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
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
    
    private void CalculateCircle(Vector3 p1, Vector3 p2, Vector3 p3, out Vector3 center, out float radius)
    {
        // Berechnung des Umkreises (Umkreis der Dreiecks-Punkte) basierend auf den drei Punkten
        Vector3 mid1 = (p1 + p2) / 2;
        Vector3 mid2 = (p2 + p3) / 2;

        Vector3 dir1 = new Vector3(-(p2 - p1).y, (p2 - p1).x, 0);
        Vector3 dir2 = new Vector3(-(p3 - p2).y, (p3 - p2).x, 0);

        float t = ((mid2.y - mid1.y) * dir2.x - (mid2.x - mid1.x) * dir2.y) / (dir1.x * dir2.y - dir1.y * dir2.x);

        center = mid1 + dir1 * t;
        radius = Vector3.Distance(center, p1);
    }
}