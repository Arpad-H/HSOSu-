using UnityEngine;

public interface ICurve
{
    void Draw(LineRenderer lineRenderer);
    Vector3 GetPointAtTime(float t);

}