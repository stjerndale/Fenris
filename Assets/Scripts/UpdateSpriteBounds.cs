using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSpriteBounds : MonoBehaviour
{
    [SerializeField]
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetComponent<SpriteRenderer>().bounds = new Bounds(player.position, new Vector3(1,1,1) * 10f);
    }
}
