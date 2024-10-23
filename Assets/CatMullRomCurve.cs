using UnityEngine;

public class CatmullRomCurve : ICurve
{
    private Vector3[] controlPoints;
    private int numPoints = 50;

    public CatmullRomCurve(Vector3[] controlPoints)
    {
        this.controlPoints = controlPoints;
    }

    public void Draw(LineRenderer lineRenderer)
    {
        if (controlPoints.Length < 2) return;

        Vector3[] positions = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            positions[i] = CalculateCatmullRomPoint(t);
        }

        lineRenderer.positionCount = numPoints;
        lineRenderer.SetPositions(positions);
    }

    private Vector3 CalculateCatmullRomPoint(float t)
    {
        int numSections = controlPoints.Length - 1;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
        float u = t * numSections - currPt;

        Vector3 a = currPt == 0 ? controlPoints[0] : controlPoints[currPt - 1];
        Vector3 b = controlPoints[currPt];
        Vector3 c = controlPoints[currPt + 1];
        Vector3 d = currPt + 2 < controlPoints.Length ? controlPoints[currPt + 2] : controlPoints[currPt + 1];

        return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) +
                       (2f * a - 5f * b + 4f * c - d) * (u * u) + 
                       (-a + c) * u + 2f * b);
    }
    public Vector3 GetPointAtTime(float t)
    {
        // Catmull-Rom requires at least 4 points
        if (controlPoints.Length < 4) return controlPoints[0];

        // Figure out which segment we're in (based on total t)
        int numSegments = controlPoints.Length - 3; // Num segments = num points - 3 (catmull-rom uses four points per segment)
        float segmentT = t * numSegments;
        int segmentIndex = Mathf.FloorToInt(segmentT);
        segmentT -= segmentIndex;

        // Ensure index stays within bounds
        if (segmentIndex >= controlPoints.Length - 3)
        {
            segmentIndex = controlPoints.Length - 4;
        }

        Vector3 p0 = controlPoints[segmentIndex];
        Vector3 p1 = controlPoints[segmentIndex + 1];
        Vector3 p2 = controlPoints[segmentIndex + 2];
        Vector3 p3 = controlPoints[segmentIndex + 3];

        // Catmull-Rom interpolation formula
        float t2 = segmentT * segmentT;
        float t3 = t2 * segmentT;

        return 0.5f * ((2.0f * p1) +
                       (-p0 + p2) * segmentT +
                       (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
                       (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3);
    }
}