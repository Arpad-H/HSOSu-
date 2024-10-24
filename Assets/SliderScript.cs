using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour
{
    private float _ttl = 5f;
    private float maxTtl;// Total time for the sphere to travel
    private ICurve _curve;
    private ICurve infill_curve;
    public LineRenderer _lineRenderer;
    public LineRenderer infill;
    public GameObject sphere;
    public GameObject sphereHolder;
    float circumference = 2 * Mathf.PI * 30;
    private float rotationAmount = 0;
    private float _arcLength;
    private float _elapsedTime;
    bool hovered = false;
    bool clicked = false;

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
                _curve = new BezierCurve(controlPoints.ToArray(), unityLength);
                infill_curve = new BezierCurve(controlPoints.ToArray(), unityLength);
                break;
                // _curve = new PerfectCircleCurve(controlPoints.ToArray(), unityLength);
                // infill_curve = new PerfectCircleCurve(controlPoints.ToArray(), unityLength);
                // break;
            // default:
            //     Debug.LogWarning("Unbekannter Kurventyp: " + curveType);
            //     break;
        }
    }

    private void SetLineRenderer()
    {
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startWidth = 50f * transform.localScale.x;
        _lineRenderer.endWidth = 50f* transform.localScale.x;

        infill.startWidth = 40f* transform.localScale.x;
        infill.endWidth = 40f* transform.localScale.x;
        _lineRenderer.startColor = new Color(1f, 01, 1f, 1f);
        _lineRenderer.endColor = new Color(1f, 1f, 1f, 1f);
    }

    public void SetupSphere()
    {
        sphere.transform.localScale = new Vector3(60f, 60f, 60f);
    }

    public void SetColor(Color c)
    {
        infill.startColor = c;
        infill.endColor = c;
        
        sphere.GetComponent<MeshRenderer>().material.color = c;
    }

    private void MoveSphereAlongCurve(float t)
    {
        Vector3 previousPosition = sphere.transform.position;
        // Interpolates t based on total TTL (from 0 to 1) and moves the sphere along the curve
        Vector3 newPosition = _curve.GetPointAtTime(Mathf.Clamp01(t));
        float distanceMoved = Vector3.Distance(previousPosition, newPosition);
        rotationAmount = (distanceMoved / circumference) * 360f;
        
        //sphere.transform.LookAt(newPosition);
        sphere.transform.Rotate(0, -rotationAmount, 0);
        sphereHolder.transform.LookAt(newPosition);
        sphereHolder.transform.position = newPosition;
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

    public float GetTTL()
    {
        return _ttl;
    }


    public float GetMaxTTL()
    {
        return maxTtl;
    }
}
