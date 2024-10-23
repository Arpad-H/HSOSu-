using UnityEngine;

public class LinearCurve : ICurve
{
    private Vector3[] controlPoints;

    public LinearCurve(Vector3[] controlPoints)
    {
        this.controlPoints = controlPoints;
    }

    public void Draw(LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = controlPoints.Length;
        lineRenderer.SetPositions(controlPoints);
    }
    public Vector3 GetPointAtTime(float t)
    {
        // Linear interpolation between two points
        if (controlPoints.Length == 2)
        {
            return Vector3.Lerp(controlPoints[0], controlPoints[1], t);
        }

        // If there are more than two points, we could do segment-based linear interpolation
        int numSegments = controlPoints.Length - 1;
        float segmentT = t * numSegments;
        int segmentIndex = Mathf.FloorToInt(segmentT);
        segmentT -= segmentIndex;

        if (segmentIndex >= controlPoints.Length - 1)
        {
            return controlPoints[controlPoints.Length - 1]; // Return last point if at the end
        }

        return Vector3.Lerp(controlPoints[segmentIndex], controlPoints[segmentIndex + 1], segmentT);
    }
    
}