using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refill : MonoBehaviour
{
    [SerializeField]
    int refillAmount;
    [SerializeField]
    FlowerInformation flowerInfo;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerInventory>() != null)
        {
            other.gameObject.GetComponent<PlayerInventory>().IncreaseStackBy(flowerInfo, refillAmount);
        }
    }
}
