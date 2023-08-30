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
    

    // Start is called before the first frame update
    void Start()
    {
        sunLight = sun.GetComponent<Light>();
        divider = 360f / secondsPerDygn;
    }

    // Update is called once per frame
    void Update()
    {
        // dt * 360/x -> x seconds per day-night
        // dt * 360/x * 1/secondsPerTick -> x ticks per day-night
        sun.transform.Rotate(0, Time.deltaTime * divider, 0, Space.World);
        // zenith at 0d

        //sun.transform.position = new Vector3(sun.transform.position.x, Mathf.Cos(yrot * Mathf.PI) * 4, sun.transform.position.z);
        Debug.Log(sun.transform.eulerAngles.y / 360);

        float t = Mathf.Abs( sun.transform.eulerAngles.y);

        if (sun.transform.eulerAngles.y > 0 && sun.transform.eulerAngles.y < 90)
        {
            sunLight.color = Color.Lerp(dusk, night, t / 90);
        }
        else if (sun.transform.eulerAngles.y > 90 && sun.transform.eulerAngles.y < 180)
        {
            sunLight.color = Color.Lerp(night, dawn, (t - 90) / 90);
        }
        else if (sun.transform.eulerAngles.y > 180 && sun.transform.eulerAngles.y < 270)
        {
            sunLight.color = Color.Lerp(dawn, day, (t - 180) / 90);
        }
        else if (sun.transform.eulerAngles.y > 270)
        {
            sunLight.color = Color.Lerp(day, dusk, (t - 270) / 90);
        }
    }
}
