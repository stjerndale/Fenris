using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateColliderMeshWithObject : MonoBehaviour
{
    [SerializeField]
    float bendAmount = 0.02f;
    private BoxCollider coll;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = transform.position - Camera.main.transform.position;
        float moveInY = (- bendAmount) * Mathf.Pow(diff.z, 2);
        coll.center = new Vector3(0, moveInY, 0);
        //transform.position = new Vector3(transform.position.x, moveInY, transform.position.z);
    }
}
