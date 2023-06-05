using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInformation : MonoBehaviour
{
    [SerializeField]
    private int TickRate;
    public static int SecondsPerTick;
    public static int TimeTicks;

    void Start()
    {
        SecondsPerTick = TickRate;
        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(SecondsPerTick);
            TimeTicks++;
            //Debug.Log("Tick: " + TimeTicks);
        }
    }

    public void Reset()
    {
        TimeTicks = 0;
    }
}
