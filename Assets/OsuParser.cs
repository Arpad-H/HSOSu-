using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsuParser : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the song's audio source //TODO decide based on menu selection and from parser
    public GameObject dotPrefab; // Prefab for the dot
    public GameObject sliderPrefab; // Prefab for the slider
    public GameObject spinnerPrefab; // Prefab for the spinner
    private List<HitObject> hitObjects = new List<HitObject>();
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
                hitObjects.Add(new Circle(OsuToUnityCoordinates(x,y), time));
            }
            else if ((type & 2) > 0) // Check if it's a slider
            {
                var sliderData = data[5]; // L|291:77, P|...
                hitObjects.Add(new Slider(OsuToUnityCoordinates(x,y), time, sliderData));
            }
            else if ((type & 4) > 0) // Check if it's a spinner
            {
                hitObjects.Add(new Spinner(OsuToUnityCoordinates(x,y), time));
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
        foreach (var hitObject in hitObjects)
        {
            yield return new WaitForSeconds(hitObject.Time - audioSource.time);
            switch (hitObject)
            {
                case Circle circle:
                    SpawnDot(circle.Position);
                    // SpawnSpinner(circle.Position);
                    break;
                case Slider slider:
                    SpawnSlider(slider.Position, slider.PathData);
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
   

    void SpawnSlider(Vector3 position, string pathData)
    {
        //TODO
        // Handle slider spawning and movement along pathData
        var sliderObject = Instantiate(sliderPrefab, position, Quaternion.identity);
        // Additional logic to animate slider along the path
    }
}

public abstract class HitObject
{
    public Vector3 Position;
    public float Time;

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
    public string PathData;

    public Slider(Vector3 position, float time, string pathData) : base(position, time)
    {
        this.PathData = pathData;
    }
}

public class Spinner : HitObject
{
    public Spinner(Vector3 position, float time) : base(position, time)
    {
        
    }
}