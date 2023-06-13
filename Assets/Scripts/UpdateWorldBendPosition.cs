using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWorldBendPosition : MonoBehaviour
{
    [SerializeField]
    float bendAmount = 0.03f;

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = transform.position - Camera.main.transform.position;
        float moveInY = (-bendAmount) * Mathf.Pow(diff.z, 2);
        transform.position = new Vector3(transform.position.x, moveInY, transform.position.z);
    }
}
