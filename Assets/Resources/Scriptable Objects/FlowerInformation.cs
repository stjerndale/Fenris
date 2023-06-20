using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FlowerInformation", order = 1)]
public class FlowerInformation : ScriptableObject
{

    public enum Requirements
    {
        NearWater,
        Shadows,
    }

    [Header("Seed Information")]
    [SerializeField]
    public int id;

    [SerializeField]
    public Color UIColor;

    [Header("Appearance")]
    [SerializeField]
    public GameObject prefab;

    [SerializeField]
    public Sprite seed;

    [SerializeField]
    public Sprite leaf;

    [SerializeField]
    public Sprite bloom; 
    
    [SerializeField]
    public Texture mask;

    [Header("Spread")]
    [Tooltip("How many seconds it takes for the plant to have a chance to spread")]
    [SerializeField]
    public float spreadRate;

    [Tooltip("The chance for the plant to spread")]
    [SerializeField]
    public float spreadChance;

    [Header("Growth & Requirements")]
    [SerializeField]
    public float growthSpeed;

    [SerializeField]
    public Requirements[] growthRequirements;
}