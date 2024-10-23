using UnityEngine;

public class CatmullRomCurve : ICurve
{
    private readonly Vector3[] _controlPoints;
    private const int k_NumPoints = 50;
    private readonly float _unityLength;

    public CatmullRomCurve(Vector3[] controlPoints, float unityLength)
    {
        _controlPoints = controlPoints;
        _unityLength = unityLength;
    }

    public void Draw(LineRenderer lineRenderer)
    {
        if (_controlPoints.Length < 2) return;

        Vector3[] positions = new Vector3[k_NumPoints];
        float currentLength = 0.0f;
        Vector3 previousPoint = CalculateCatmullRomPoint(0);
        positions[0] = previousPoint;
        int numValidPoints = 1;

        for (int i = 1; i < k_NumPoints; i++)
        {
            float t = i / (float)(k_NumPoints - 1);
            Vector3 point = CalculateCatmullRomPoint(t);
            float segmentLength = Vector3.Distance(previousPoint, point);

            if (currentLength + segmentLength > _unityLength)
            {
                float remainingLength = _unityLength - currentLength;
                Vector3 direction = (point - previousPoint).normalized;
                positions[numValidPoints] = previousPoint + direction * remainingLength;
                numValidPoints++;
                break;
            }

            positions[numValidPoints] = point;
            numValidPoints++;
            currentLength += segmentLength;
            previousPoint = point;
        }

        lineRenderer.positionCount = numValidPoints;
        lineRenderer.SetPositions(positions);
    }

    private Vector3 CalculateCatmullRomPoint(float t)
    {
        int numSections = _controlPoints.Length - 1;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
        float u = t * numSections - currPt;

        Vector3 a = currPt == 0 ? _controlPoints[0] : _controlPoints[currPt - 1];
        Vector3 b = _controlPoints[currPt];
        Vector3 c = _controlPoints[currPt + 1];
        Vector3 d = currPt + 2 < _controlPoints.Length ? _controlPoints[currPt + 2] : _controlPoints[currPt + 1];

        return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) +
                       (2f * a - 5f * b + 4f * c - d) * (u * u) +
                       (-a + c) * u + 2f * b);
    }
    public Vector3 GetPointAtTime(float t)
    {
        // Catmull-Rom requires at least 4 points
        if (_controlPoints.Length < 4) return _controlPoints[0];

        // Figure out which segment we're in (based on total t)
        int numSegments = _controlPoints.Length - 3; // Num segments = num points - 3 (catmull-rom uses four points per segment)
        float segmentT = t * numSegments;
        int segmentIndex = Mathf.FloorToInt(segmentT);
        segmentT -= segmentIndex;

        // Ensure index stays within bounds
        if (segmentIndex >= _controlPoints.Length - 3)
        {
            segmentIndex = _controlPoints.Length - 4;
        }

        Vector3 p0 = _controlPoints[segmentIndex];
        Vector3 p1 = _controlPoints[segmentIndex + 1];
        Vector3 p2 = _controlPoints[segmentIndex + 2];
        Vector3 p3 = _controlPoints[segmentIndex + 3];

        // Catmull-Rom interpolation formula
        float t2 = segmentT * segmentT;
        float t3 = t2 * segmentT;

        return 0.5f * ((2.0f * p1) +
                       (-p0 + p2) * segmentT +
                       (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
                       (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3);
    }
}
