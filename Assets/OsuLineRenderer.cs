using UnityEngine;

public class OsuLineRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {
        // Erstelle ein neues GameObject
        GameObject lineObject = new GameObject("Line");
        
        // Hinzufügen des LineRenderers
        lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Ein einfaches Beispielmaterial hinzufügen
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        // Breite einstellen
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        
        // Anzahl der Segmente einstellen
        lineRenderer.positionCount = 2;
        
        // Start- und Endposition der Linie einstellen
        lineRenderer.SetPosition(0, new Vector3(0, 0, 0)); // Startpunkt
        lineRenderer.SetPosition(1, new Vector3(1, 1, 0)); // Endpunkt
    }
}