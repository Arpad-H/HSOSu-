using UnityEngine;

public class LinearCurve : ICurve
{
    private Vector3[] controlPoints;
    private float maxLength;

    public LinearCurve(Vector3[] controlPoints, float maxLength)
    {
        if (controlPoints.Length != 2)
        {
            throw new System.ArgumentException("LinearCurve benötigt genau zwei Kontrollpunkte.");
        }

        this.controlPoints = controlPoints;
        this.maxLength = maxLength;
    }

    public void Draw(LineRenderer lineRenderer)
    {
        // Berechne die Länge der Linie
        float totalLength = Vector3.Distance(controlPoints[0], controlPoints[1]);

        // Bereite den neuen Endpunkt vor, falls die Länge MAXLänge überschreitet
        Vector3 endpoint = controlPoints[1];

        if (totalLength > maxLength)
        {
            // Skaliere die Linie zur Maxlänge, indem ein entsprechend weit liegender Punkt errechnet wird
            Vector3 direction = (controlPoints[1] - controlPoints[0]).normalized;
            endpoint = controlPoints[0] + direction * maxLength;
        }

        // Setze die Endpunkte der Linie
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, controlPoints[0]);
        lineRenderer.SetPosition(1, endpoint);
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