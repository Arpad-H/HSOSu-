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
}