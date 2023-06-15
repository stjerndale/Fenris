using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatsUIHandler : MonoBehaviour
{
    [SerializeField] GridHandler gridHandler;
    [SerializeField] int barLength;
    [SerializeField] RectTransform[] bars;
    //[SerializeField] RectTransform groundBar;

    // Start is called before the first frame update
    void Start()
    {
        //groundBar = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < bars.Length; i++)
        {
            UpdateBar(i);
        }
    }

    private void UpdateBar(int index)
    {
        float percentage = gridHandler.GetPercentage(index);
        int newSize = Mathf.FloorToInt(percentage * barLength);
        bars[index].sizeDelta = new Vector2(newSize, bars[index].sizeDelta.y);
    }
}
