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
}