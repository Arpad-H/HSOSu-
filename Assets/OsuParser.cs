using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsuParser : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the song's audio source //TODO decide based on menu selection and from parser
    public GameObject dotPrefab; // Prefab for the dot
    public GameObject sliderPrefab; // Prefab for the slider
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
        bool inHitObjects = false;

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

            int x = int.Parse(data[0]);
            int y = int.Parse(data[1]);
            float time = float.Parse(data[2])/1000;

            int type = int.Parse(data[3]);
            if ((type & 1) > 0) // Check if it's a hit circle
            {
                hitObjects.Add(new Circle(OsuToUnityCoordinates(x,y), time));
            }
            else if ((type & 2) > 0) // Check if it's a slider
            {
                string sliderData = data[5]; // L|291:77, P|...
                hitObjects.Add(new Slider(OsuToUnityCoordinates(x,y), time, sliderData));
            }
        }
    }
    Vector3 OsuToUnityCoordinates(int x_osu, int y_osu)
    {
        // Step 1: Normalize the osu coordinates (0 to 1 range)
        float x_norm = x_osu / 512f;
        float y_norm = y_osu / 384f;

        // Step 2: Calculate Unity's camera dimensions
        float cameraHeight = 2f * gameCamera.orthographicSize;
        float cameraWidth = cameraHeight * gameCamera.aspect;

        // Step 3: Map normalized osu coordinates to Unity's world coordinates
        float x_unity = (x_norm - 0.5f) * cameraWidth;
        float y_unity = (y_norm - 0.5f) * cameraHeight;

        return new Vector3(x_unity, y_unity, 0f); // Z is zero for 2D
    }
    IEnumerator SpawnObjects()
    {
        foreach (var hitObject in hitObjects)
        {
            yield return new WaitForSeconds(hitObject.time - audioSource.time);
            if (hitObject is Circle hitCircle)
            {
                SpawnDot(hitCircle.position);
            }
            else if (hitObject is Slider slider)
            {
                SpawnSlider(slider.position, slider.pathData);
            }
        }
    }

    void SpawnDot(Vector3 position)
    {
        Instantiate(dotPrefab, position, Quaternion.identity);
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
    public Vector3 position;
    public float time;

    protected HitObject(Vector3 position, float time)
    {
        this.position = position;
        this.time = time;
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
    public string pathData;

    public Slider(Vector3 position, float time, string pathData) : base(position, time)
    {
        this.pathData = pathData;
    }
}