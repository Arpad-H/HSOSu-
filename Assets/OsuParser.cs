using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsuParser : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the song's audio source //TODO decide based on menu selection and from parser
    public GameObject dotPrefab; // Prefab for the dot
    public GameObject sliderPrefab; // Prefab for the slider
    public GameObject spinnerPrefab; // Prefab for the spinner
    private readonly List<HitObject> _hitObjects = new List<HitObject>();
    public Camera gameCamera;
    void Start()
    {
        ParseOsuFile("Assets/songfile.osu"); //TODO decide based on menu selection
        StartCoroutine(SpawnObjects());
    }

    void ParseOsuFile(string path)
    {
        string[] lines = System.IO.File.ReadAllLines(path);
        var inHitObjects = false;

        foreach (var line in lines)
        {
            // Check if we've reached the [HitObjects] section
            if (line.StartsWith("[HitObjects]"))
            {
                inHitObjects = true;
                continue;
            }

            if (!inHitObjects) continue;

            var data = line.Split(',');

            var x = int.Parse(data[0]);
            var y = int.Parse(data[1]);
            var time = float.Parse(data[2])/1000;

            var type = int.Parse(data[3]);
            if ((type & 1) > 0) // Check if it's a hit circle
            {
                _hitObjects.Add(new Circle(OsuToUnityCoordinates(x,y), time));
            }
            else if ((type & 2) > 0) // Check if it's a slider
            {
                var sliderData = data[5]; // L|291:77, P|...
                var arcLength = float.Parse(data[7]);
                _hitObjects.Add(new Slider(OsuToUnityCoordinates(x,y), time, sliderData, arcLength));
            }
            else if ((type & 4) > 0) // Check if it's a spinner
            {
                _hitObjects.Add(new Spinner(OsuToUnityCoordinates(x,y), time));
            }
        }
    }
    Vector3 OsuToUnityCoordinates(int xOsu, int yOsu)
    {
        // Step 1: Normalize the osu coordinates (0 to 1 range)
        var xNorm = xOsu / 512f;
        var yNorm = yOsu / 384f;

        // Step 2: Calculate Unity's camera dimensions
        var cameraHeight = 2f * gameCamera.orthographicSize;
        var cameraWidth = cameraHeight * gameCamera.aspect;

        // Step 3: Map normalized osu coordinates to Unity's world coordinates
        var xUnity = (xNorm - 0.5f) * cameraWidth;
        var yUnity = (yNorm - 0.5f) * cameraHeight;

        return new Vector3(xUnity, yUnity, 0f); // Z is zero for 2D
    }
    IEnumerator SpawnObjects()
    {
        foreach (var hitObject in _hitObjects)
        {
            yield return new WaitForSeconds(hitObject.Time - audioSource.time);
            switch (hitObject)
            {
                case Circle circle:
                    SpawnDot(circle.Position);
                    break;
                case Slider slider:
                    SpawnSlider(slider.Position, slider.PathData, slider.ArcLength);
                    break;
                case Spinner spinner:
                    SpawnSpinner(spinner.Position);
                    break;
            }
        }
    }

    void SpawnDot(Vector3 position)
    {
        Instantiate(dotPrefab, position, Quaternion.identity);
    }

    void SpawnSpinner(Vector3 position)
    {
        Instantiate(spinnerPrefab, position, Quaternion.identity);
    }
   

    void SpawnSlider(Vector3 position, string pathData, float arcLength)
    {
        var sliderObject = Instantiate(sliderPrefab, position, Quaternion.identity);

        var points = ParsePathData(pathData ,out var curveType);
        
        points.Insert(0,new Vector3(position.x,position.y,0));
        
        var sliderComponent = sliderObject.GetComponent<SliderScript>();
        sliderComponent.SetCurveType(curveType, points, arcLength);

    }

    List<Vector3> ParsePathData(string pathData, out CurveType curveType)
    {
        var points = new List<Vector3>();
        var segments = pathData.Split('|');
        
        curveType = CurveType.Linear;
        
        if (segments.Length < 2)
            return points;

        // Erster Teil ist der Kurventyp (B, C, L, P)
        switch (segments[0])
        {
            case "B":
                curveType = CurveType.Bezier;
                break;
            case "C":
                curveType = CurveType.CatmullRom;
                break;
            case "L":
                curveType = CurveType.Linear;
                break;
            case "P":
                curveType = CurveType.PerfectCircle;
                break;
            default:
                Debug.LogWarning("Unbekannter Kurventyp: " + segments[0]);
                break;
        }

        // Darauffolgende Teile sind die Kontrollpunkte
        for (var i = 1; i < segments.Length; i++)
        {
            var coords = segments[i].Split(':');
            if (coords.Length == 2)
            {
                var xOsu = int.Parse(coords[0]);
                var yOsu = int.Parse(coords[1]);
                var point = OsuToUnityCoordinates(xOsu, yOsu);
                points.Add(point);
            }
            else
            {
                
            }
        }
        return points;
    }
}

public abstract class HitObject
{
    public Vector3 Position;
    public readonly float Time;

    protected HitObject(Vector3 position, float time)
    {
        Position = position;
        Time = time;
    }
}

public class Circle : HitObject //TODO put in circleScript
{
    public Circle(Vector3 position, float time) : base( position, time)
    {
    }
}

public class Slider : HitObject //TODO put in sliderScript
{
    public readonly string PathData;
    public readonly float ArcLength;

    public Slider(Vector3 position, float time, string pathData, float arcLength) : base(position, time)
    {
        PathData = pathData;
        ArcLength = arcLength;
    }
}

public class Spinner : HitObject
{
    public Spinner(Vector3 position, float time) : base(position, time)
    {
        
    }
}