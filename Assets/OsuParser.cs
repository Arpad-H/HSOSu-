using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsuParser : MonoBehaviour
{
    private const int k_OsuWidth = 512;
    private const int k_OsuHeight = 384;
    public AudioSource audioSource; // Reference to the song's audio source //TODO decide based on menu selection and from parser
    public GameObject dotPrefab; // Prefab for the dot
    public GameObject sliderPrefab; // Prefab for the slider
    public GameObject spinnerPrefab; // Prefab for the spinner
    private readonly List<HitObject> _hitObjects = new List<HitObject>();
    List<TimingPoint> timingPoints = new List<TimingPoint>();
    Dictionary<int, Color> comboColors = new Dictionary<int, Color>();
    public Camera gameCamera;

    void Start()
    {
        ParseOsuFile("Assets/songfile.osu"); //TODO decide based on menu selection
        StartCoroutine(SpawnObjects());
    }

    void ParseOsuFile(string path)
    {
        string[] lines = System.IO.File.ReadAllLines(path);
        bool inHitObjects = false;
        bool inTimingPoints = false;
        bool inColours = false;
        int comboIndex = 0;  // To track the current combo index

    
        foreach (var line in lines)
        {
            // Check if we've reached the [Colours] section
            if (line.StartsWith("[Colours]"))
            {
                inColours = true;
                inHitObjects = false;
                continue;
            }

            
            // Check if we've reached the [TimingPoints] section
            if (line.StartsWith("[TimingPoints]"))
            {
                inColours = false;
                inTimingPoints = true;
                inHitObjects = false;
                continue;
            }

            // Check if we've reached the [HitObjects] section
            if (line.StartsWith("[HitObjects]"))
            {
                inHitObjects = true;
                continue;
            }
            // Parse the [Colours] section
            if (inColours && line.StartsWith("Combo"))
            {
                if (line.Length == 0)
                {
                    inColours = false;
                    continue;
                }
                
                var split = line.Split(':');
                var comboNumber = int.Parse(split[0].Substring(5));  // Get the combo number (e.g., Combo1 -> 1)
                var colorValues = split[1].Trim().Split(',');
                var color = new Color(
                    float.Parse(colorValues[0]) / 255f,
                    float.Parse(colorValues[1]) / 255f,
                    float.Parse(colorValues[2]) / 255f
                );
                comboColors[comboNumber] = color;  // Store combo color
            }

            if (inTimingPoints)
            {
                if (line.Length == 0)
                {
                    inTimingPoints = false;
                    continue;
                }
                string[] data = line.Split(',');

                float time = int.Parse(data[0]) / 1000f; // Time in seconds
                float beatLength = float.Parse(data[1]); // Positive for uninherited, negative for inherited
                int meter = int.Parse(data[2]); // Time signature (optional, often ignored)
                // float sliderMultiplier = data.Length > 7 ? float.Parse(data[7]) : 1f;

                bool inherited = beatLength < 0; // Inherited timing points have negative beatLength

                timingPoints.Add(new TimingPoint(time, beatLength,meter, inherited));
            }


            if (inHitObjects)
            {

                var data = line.Split(',');

                var x = int.Parse(data[0]);
                var y = int.Parse(data[1]);
                var time = float.Parse(data[2]) / 1000;

                var type = int.Parse(data[3]);

                // Check if this hit object starts a new combo
                // Apply the correct color based on comboIndex
                if ((type & 3) > 0) comboIndex++;
                var colorIndex = (comboIndex % comboColors.Count) + 1;  // Loop through combo colors
                var color = comboColors.ContainsKey(colorIndex) ? comboColors[colorIndex] : Color.white;

                if ((type & 1) > 0) // Check if it's a hit circle
                {
                    _hitObjects.Add(new Circle(OsuToUnityCoordinates(x,y), time, color));
                }
                else if ((type & 2) > 0) // Check if it's a slider
                {
                    var sliderData = data[5]; // L|291:77, P|...
                    var pixelLength = float.Parse(data[7]);
                    _hitObjects.Add(new Slider(OsuToUnityCoordinates(x,y), time, sliderData, OsuToUnityLength(pixelLength), color));
                }
                else if ((type & 4) > 0) // Check if it's a spinner
                {
                    _hitObjects.Add(new Spinner(OsuToUnityCoordinates(k_OsuWidth / 2, k_OsuHeight / 2), time, color));
                }
            }
        }
    }

    float OsuToUnityLength(float osuPixelLength)
    {
        // Berechnung der Höhe und Breite der Kamera in Unity-Einheiten
        var cameraHeight = 2f * gameCamera.orthographicSize;  // Ortographic Size * 2 für die gesamte Höhe
        var cameraWidth = cameraHeight * gameCamera.aspect;   // Breite = Höhe * Seitenverhältnis

        // Berechnung der Skalierung von Pixeln zur Weltgröße
        var pixelSizeX = cameraWidth / k_OsuWidth;
        var pixelSizeY = cameraHeight / k_OsuHeight;

        // Berechnung des Verhältnisses für osu! Pixel zu Unity-Einheiten
        // Hier nimmst du den Mittelwert, falls die Pixel nicht quadratisch skaliert
        var osuToUnityScale = (pixelSizeX + pixelSizeY) / 2f;

        // Umrechnung der osu! Pixel Länge auf Unity Länge
        var unityLength = osuPixelLength * osuToUnityScale;

        // Debug.Log("UnityLength: " + unityLength);
        return unityLength;
        
    }
    
    Vector3 OsuToUnityCoordinates(int xOsu, int yOsu)
    {
        // Step 1: Normalize the osu coordinates (0 to 1 range)
        var xNorm = xOsu * 1f / k_OsuWidth;
        var yNorm = yOsu * 1f / k_OsuHeight;

        // Step 2: Calculate Unity's camera dimensions
        var cameraHeight = 2f * gameCamera.orthographicSize;
        var cameraWidth = cameraHeight * gameCamera.aspect;
        
        // Debug.Log("AspectRatio: " + gameCamera.aspect);
        // 1.7777778 = 16:9
        
        // Step 3: Map normalized osu coordinates to Unity's world coordinates
        var xUnity = (xNorm - 0.5f) * cameraWidth;
        var yUnity = (yNorm - 0.5f) * cameraHeight;

        return new Vector3(xUnity, yUnity, 0f); // Z is zero for 2D
    }

    IEnumerator SpawnObjects()
    {
        TimingPoint currentTimingPoint = timingPoints[0]; // Start with the first timing point

        foreach (var hitObject in _hitObjects)
        {
            // Get the correct timing point for the current hit object's time
            currentTimingPoint = GetTimingPointForTime(timingPoints, hitObject.time);

            // Calculate time to wait, adjusting for BPM (based on beat length)
            float waitTime = hitObject.time - audioSource.time;
            if (currentTimingPoint != null && currentTimingPoint.inherited)
            {
                // If it's an uninherited point, use the beat length for accurate timing
                waitTime = hitObject.time - (audioSource.time + currentTimingPoint.beatLength / 1000f);
            }

            yield return new WaitForSeconds(waitTime);
            switch (hitObject)
            {
                case Circle circle:
                    SpawnDot(circle.position, circle.color);
                    break;
                case Slider slider:
                    // Adjust slider speed and length according to timing point's slider multiplier
                    float adjustedArcLength = slider.ArcLength;
                    SpawnSlider(slider.position, slider.PathData, adjustedArcLength, slider.color);
                    break;
                case Spinner spinner:
                    SpawnSpinner(spinner.position);
                    break;
            }
        }
    }

    void SpawnDot(Vector3 position,Color color)
    {
        var dot = Instantiate(dotPrefab, position, Quaternion.identity);
        dot.GetComponent<CircleScript>().SetColor(color);
    }

    void SpawnSpinner(Vector3 position)
    {
        Instantiate(spinnerPrefab, position, Quaternion.identity);
    }


    void SpawnSlider(Vector3 position, string pathData, float arcLength,Color color)
    {
        var sliderObject = Instantiate(sliderPrefab, position, Quaternion.identity);

        var points = ParsePathData(pathData, out var curveType);

        points.Insert(0, new Vector3(position.x, position.y, 0));

        var sliderComponent = sliderObject.GetComponent<SliderScript>();
        sliderComponent.SetCurveType(curveType, points, arcLength);
        sliderComponent.SetColor(color);

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
            // default:
            //     Debug.LogWarning("Unbekannter Kurventyp: " + segments[0]);
            //     break;
        }

        // Darauffolgende Teile sind die Kontrollpunkte
        for (var i = 1; i < segments.Length; i++)
        {
            var coords = segments[i].Split(':');
            if (coords.Length != 2) continue;
            var xOsu = int.Parse(coords[0]);
            var yOsu = int.Parse(coords[1]);
            var point = OsuToUnityCoordinates(xOsu, yOsu);
            points.Add(point);
        }

        return points;
    }

    TimingPoint GetTimingPointForTime(List<TimingPoint> timingPoints, float time)
    {
        TimingPoint current = timingPoints[0];

        foreach (var point in timingPoints)
        {
            if (point.time > time)
                break;

            current = point;
        }

        return current;
    }

    public class TimingPoint
    {

        public string[] data;
        public float time; // Time in seconds
        public float beatLength; // Positive for uninherited, negative for inherited
        public int meter; // Time signature (optional, often ignored)
        public float sliderMultiplier;
        public bool inherited;

        public TimingPoint(float time, float beatLength,  int meter, bool inherited)
        {
            this.time = time;
            this.beatLength = beatLength;
            this.meter = meter;
            // this.sliderMultiplier = sliderMultiplier;
            this.inherited = inherited;
        }
    }


    public abstract class HitObject
    {
        public Vector3 position;
        public readonly float time;
        public Color color;

        protected HitObject(Vector3 position, float time,Color color)
        {
            this.position = position;
            this.time = time;
            this.color = color;
        }
    }

    public class Circle : HitObject //TODO put in circleScript
    {
        public Circle(Vector3 position, float time, Color color) : base(position, time, color)
        {
        }
    }

    public class Slider : HitObject //TODO put in sliderScript
    {
        public readonly string PathData;
        public readonly float ArcLength;

        public Slider(Vector3 position, float time, string pathData, float arcLength,Color color) : base(position, time,color)
        {
            PathData = pathData;
            ArcLength = arcLength;
        }
    }

    public class Spinner : HitObject
    {
        public Spinner(Vector3 position, float time,Color color) : base(position, time,color)
        {

        }
    }
}