using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour
{
    private float _ttl = 1f;
    private float maxTtl;// Total time for the sphere to travel
    private ICurve _curve;
    private ICurve infill_curve;
    public LineRenderer _lineRenderer;
    public LineRenderer infill;
    public GameObject sphere;

    private float _arcLength;
    private float _elapsedTime;

    void Start()
    {
        maxTtl = _ttl;
        SetLineRenderer();
        SetupSphere();
        _curve.Draw(_lineRenderer);
        infill_curve.Draw(infill);

        // Calculate arc length of the curve for smooth movement
        _arcLength = CalculateArcLength(_curve);
    }

    private void Update()
    {
        _ttl -= Time.deltaTime;
        _elapsedTime += Time.deltaTime;

        if (_ttl <= 0f)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // Move the sphere along the curve based on the time elapsed
            MoveSphereAlongCurve(_elapsedTime / maxTtl);  // 5f is the initial _ttl
        }
    }

    public void SetCurveType(CurveType curveType, List<Vector3> controlPoints, float unityLength)
    {
        _arcLength = unityLength;  // Set arc length from passed value
        switch (curveType)
        {
            case CurveType.Bezier:
                _curve = new BezierCurve(controlPoints.ToArray(), unityLength);
                infill_curve = new BezierCurve(controlPoints.ToArray(), unityLength);
                break;
            case CurveType.CatmullRom:
                _curve = new CatmullRomCurve(controlPoints.ToArray(), unityLength);
                infill_curve = new CatmullRomCurve(controlPoints.ToArray(), unityLength);
                break;
            case CurveType.Linear:
                _curve = new LinearCurve(controlPoints.ToArray(), unityLength);
                infill_curve = new LinearCurve(controlPoints.ToArray(), unityLength);
                break;
            case CurveType.PerfectCircle:
                _curve = new PerfectCircleCurve(controlPoints.ToArray(), unityLength);
                infill_curve = new PerfectCircleCurve(controlPoints.ToArray(), unityLength);
                break;
            // default:
            //     Debug.LogWarning("Unbekannter Kurventyp: " + curveType);
            //     break;
        }
    }

    private void SetLineRenderer()
    {
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startWidth = 0.6f;
        _lineRenderer.endWidth = 0.6f;

        infill.startWidth = 0.5f;
        infill.endWidth = 0.5f;
        infill.startColor = new Color(0f, 0f, 0f, 1f);
        infill.endColor = new Color(0f, 0f, 0f, 1f);
    }

    public void SetupSphere()
    {
        sphere.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
    }

    public void SetColor(Color c)
    {
        _lineRenderer.startColor = c;
        _lineRenderer.endColor = c;
    }

    private void MoveSphereAlongCurve(float t)
    {
        // Interpolates t based on total TTL (from 0 to 1) and moves the sphere along the curve
        Vector3 newPosition = _curve.GetPointAtTime(Mathf.Clamp01(t));
        sphere.transform.position = newPosition;
    }

    private float CalculateArcLength(ICurve curve)
    {
        // This is a simple method to estimate the arc length of a curve by sampling points
        float length = 0f;
        Vector3 previousPoint = curve.GetPointAtTime(0f);
        int numSamples = 100;  // Number of segments for sampling the curve
        
        for (int i = 1; i <= numSamples; i++)
        {
            float t = i / (float)numSamples;
            Vector3 currentPoint = curve.GetPointAtTime(t);
            length += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return length;
    }
}
