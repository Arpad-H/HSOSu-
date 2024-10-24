using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OsuParser : MonoBehaviour
{
    private float playfieldHeight; 
    private float playfieldWidth;
    private float osuScale; 
    private const int k_OsuWidth = 512;
    private const int k_OsuHeight = 384;
    public Canvas canvas;
    public AudioSource audioSource; // Reference to the song's audio source //TODO decide based on menu selection and from parser
    public GameObject dotPrefab; // Prefab for the dot
    public GameObject sliderPrefab; // Prefab for the slider
    public GameObject spinnerPrefab; // Prefab for the spinner
    private Image image;
    private readonly List<HitObject> _hitObjects = new List<HitObject>();
    List<TimingPoint> timingPoints = new List<TimingPoint>();
    Dictionary<int, Color> comboColors = new Dictionary<int, Color>();
    public Camera gameCamera;

    void Start()
    {
        image = FindObjectOfType<Image>();
        audioSource = FindObjectOfType<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Audio source not found");
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        var number = PlayerPrefs.HasKey("Song") ? PlayerPrefs.GetInt("Song") : 1;
        ParseOsuFile("Assets/song" + number + ".osu", out var audioFile, out var background);
        audioSource.clip = Resources.Load<AudioClip>(audioFile);
        Debug.Log(background);
        image.sprite = Resources.Load<Sprite>(background);
        if (audioSource.clip == null)
        {
            Debug.LogError("Audio clip not found.\t" + audioFile);
        }
        audioSource.clip.LoadAudioData();
        audioSource.Play();
        StartCoroutine(SpawnObjects());
    }

    void ParseOsuFile(string path, out string audioFile, out string background)
    {
        string[] lines = System.IO.File.ReadAllLines(path);
        bool inHitObjects = false;
        bool inTimingPoints = false;
        bool inColours = false;
        int comboIndex = 0;  // To track the current combo index

        audioFile = "Songs/audio";
        background = "Backgrounds/background";
        
        foreach (var line in lines)
        {
            if (line.StartsWith("0,0,\""))
            {
                background = line.Replace("0,0,\"", "Backgrounds/");
                var index = background.LastIndexOf('\"');
                background = background[..index];
                index = background.LastIndexOf('.');
                background = background[..index];
                continue;
            }
            if (line.StartsWith("AudioFilename: "))
            {
                audioFile = line.Replace("AudioFilename: ", "Songs/");
                var index = audioFile.LastIndexOf('.');
                audioFile = audioFile[..index];
                continue;
            }

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

    float OsuToUnityLength(float osuLen)
    {
        return osuLen;
    }
    
    Vector3 OsuToUnityCoordinates(int xOsu, int yOsu)
    {
        
        yOsu = k_OsuHeight / 2 - yOsu; // Flip Y axis
        xOsu -= k_OsuWidth / 2;
        return new Vector3(xOsu, yOsu, 0);
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


    void SpawnSlider(Vector3 position, string pathData, float length,Color color)
    {
        var sliderObject = Instantiate(sliderPrefab, position, Quaternion.identity);

        var points = ParsePathData(pathData, out var curveType);

        points.Insert(0, new Vector3(position.x, position.y, 0));

        var sliderComponent = sliderObject.GetComponent<SliderScript>();
        sliderComponent.SetCurveType(curveType, points, length);
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