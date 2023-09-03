using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private GameObject sun;
    private Light sunLight;
    [SerializeField] private int secondsPerDygn;
    private float divider;

    [SerializeField] private Color day;
    [SerializeField] private Color dawn;
    [SerializeField] private Color dusk;
    [SerializeField] private Color night;
    private Gradient gradient;


    // Start is called before the first frame update
    void Start()
    {
        sunLight = sun.GetComponent<Light>();
        divider = 360f / secondsPerDygn;

        // prepare the color gradient
        gradient = new Gradient();
        var colors = new GradientColorKey[5];
        colors[0] = new GradientColorKey(day, 0.0f);
        colors[1] = new GradientColorKey(dusk, 0.20f);
        colors[2] = new GradientColorKey(night, 0.60f);
        colors[3] = new GradientColorKey(dawn, 0.8f);
        colors[4] = new GradientColorKey(day, 1.0f);

        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1f);

        gradient.SetKeys(colors, alphas);
    }

    // Update is called once per frame
    void Update()
    {
        // dt * 360/x -> x seconds per day-night
        // dt * 360/x * 1/secondsPerTick -> x ticks per day-night
        sun.transform.Rotate(0, Time.deltaTime * divider, 0, Space.World);
        // zenith at 0d

        //sun.transform.position = new Vector3(sun.transform.position.x, Mathf.Cos(yrot * Mathf.PI) * 4, sun.transform.position.z);

        float t = sun.transform.eulerAngles.y / 360f;

        sunLight.color = gradient.Evaluate(t);
    }
}
