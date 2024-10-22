using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SliderScript : MonoBehaviour
{
    private float _ttl = 5f;
    private ICurve _curve;
    private LineRenderer _lineRenderer;

    void Start()
    {
        SetLineRenderer();
        _curve.Draw(_lineRenderer);
    }

    private void Update()
    {
        _ttl -= Time.deltaTime;
        if (_ttl <= 0f) Destroy(this.gameObject);
    }

    public void SetCurveType(CurveType curveType, List<Vector3> controlPoints, float arcLength)
    {
        switch (curveType)
        {
            case CurveType.Bezier:
                _curve = new BezierCurve(controlPoints.ToArray());
                break;
            case CurveType.CatmullRom:
                _curve = new CatmullRomCurve(controlPoints.ToArray());
                break;
            case CurveType.Linear:
                _curve = new LinearCurve(controlPoints.ToArray());
                break;
            case CurveType.PerfectCircle:
                _curve = new PerfectCircleCurve(controlPoints.ToArray(), arcLength);
                break;
            default:
                Debug.LogWarning("Unbekannter Kurventyp: " + curveType);
                break;
        }
        
    }

    private void SetLineRenderer()
    {
        _lineRenderer = this.AddComponent<LineRenderer>();
        
        // Beispielhafte Materialkonfiguration
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;

    }
}