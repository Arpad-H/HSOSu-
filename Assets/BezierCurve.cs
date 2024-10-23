using UnityEngine;

public class BezierCurve : ICurve
{
    private Vector3[] controlPoints;
    private int numPoints = 50;  // Anzahl der zu zeichnenden Punkte auf der Kurve

    public BezierCurve(Vector3[] controlPoints)
    {
        this.controlPoints = controlPoints;
    }

    public void Draw(LineRenderer lineRenderer)
    {
        Vector3[] positions = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            positions[i] = CalculateBezierPoint(t);
        }
        lineRenderer.positionCount = numPoints;
        lineRenderer.SetPositions(positions);
    }

    private Vector3 CalculateBezierPoint(float t)
    {
        Vector3 point = Vector3.zero;
        int n = controlPoints.Length - 1;
        
        for (int i = 0; i <= n; i++)
        {
            float binomialCoeff = BinomialCoefficient(n, i);
            float term = binomialCoeff * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - i);
            point += term * controlPoints[i];
        }

        return point;
    }

    private int BinomialCoefficient(int n, int k)
    {
        if (k > n)
            return 0;
        if (k == 0 || k == n)
            return 1;
        
        int c = 1;
        for (int i = 0; i < k; i++)
        {
            c = c * (n - i) / (i + 1);
        }
        return c;
    }
    public Vector3 GetPointAtTime(float t)
    {
        return CalculateBezierPoint(t);
    }
}