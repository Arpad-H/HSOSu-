using UnityEngine;

public class BezierCurve : ICurve
{
    private readonly Vector3[] _controlPoints;
    private const int k_NumPoints = 50;
    private readonly float _unityLength;

    public BezierCurve(Vector3[] controlPoints, float unityLength)
    {
        _controlPoints = controlPoints;
        _unityLength = unityLength;
    }

    public void Draw(LineRenderer lineRenderer)
    {
        Vector3[] positions = new Vector3[k_NumPoints];
        float currentLength = 0.0f;
        Vector3 previousPoint = CalculateBezierPoint(0);
        positions[0] = previousPoint;
        int numValidPoints = 1;

        for (int i = 1; i < k_NumPoints; i++)
        {
            float t = i / (float)(k_NumPoints - 1);
            Vector3 point = CalculateBezierPoint(t);
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

    private Vector3 CalculateBezierPoint(float t)
    {
        Vector3 point = Vector3.zero;
        int n = _controlPoints.Length - 1;

        for (int i = 0; i <= n; i++)
        {
            float binomialCoeff = BinomialCoefficient(n, i);
            float term = binomialCoeff * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - i);
            point += term * _controlPoints[i];
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