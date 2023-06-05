using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField]
    public Transform icon;

    [SerializeField]
    Image img;

    [SerializeField]
    private Color selectedColour;
    private Color originalColour;

    public FlowerInformation flowerInfo;

    // Start is called before the first frame update
    void Start()
    {
        img = transform.GetComponent<Image>();
        originalColour = img.color;
    }

    public void SetIcon(FlowerInformation flowerInfo)
    {
        this.flowerInfo = flowerInfo;
        Image iconImg = icon.GetComponent<Image>();
        iconImg.color = flowerInfo.UIColor;
    }

    public void Select()
    {
        img.color = selectedColour;
    }

    public void Deselect()
    {
        img.color = originalColour;
    }
}
