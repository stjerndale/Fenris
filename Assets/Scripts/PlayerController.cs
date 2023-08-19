using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed = 1.0f;
    [SerializeField]
    float jumpAmount;

    [SerializeField]
    Rigidbody rb;
    SpriteRenderer sprite;

    private float hori;
    private float verti;
    private Vector3 direction;
    private bool immobile;

    private GameObject groundTile; // the tile currently below the player
    private GameObject targetTile;

    [SerializeField]
    FlowerInformation flowerInfo;

    private PlayerInventory inventory;

    private void Start()
    {
        immobile = false;
        SetGroundTile();
        targetTile = groundTile;
        inventory = transform.GetComponent<PlayerInventory>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

        GameEvents.current.onImmobilizePlayer += SetImmobile;
    }

    void Update()
    {
        if(immobile)
        {
            return;
        }

        hori = Input.GetAxisRaw("Horizontal");
        verti = Input.GetAxisRaw("Vertical");
        direction = new Vector3(hori, 0f, verti);

        /* if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpAmount, ForceMode.Impulse);
        }*/

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlantSeed();
        }

        if (Input.GetKeyDown(KeyCode.V)){
            WaterPlant();
        }

        if(Input.GetMouseButtonDown(0))
        {
            DigGround();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetTargetTile();
        if(direction.magnitude > 0.01f && GetNextTilePassable())
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            
            if(direction.x < 0f)
            {
                sprite.flipX = true;
            }
            else if(direction.x > 0f)
            {
                sprite.flipX = false;
            }

            SetGroundTile(); // update groundTile when moving
        }
    }

    private void PlantSeed()
    {
        if(targetTile == null) { return; }

        BoxLogic boxC = targetTile.GetComponent<BoxLogic>();
        if (inventory.GetStackSize() > inventory.selectedSeed)
        {
            bool success = boxC.PlantFlower(inventory.GetSelectedStack().flowerInformation);
            if (success)
            {
                // trigger flower planted event
                GameEvents.current.FlowerPlanted(inventory.GetSelectedStack().flowerInformation);

                inventory.DecreaseSeedStack(inventory.selectedSeed);
            }
        }
    }

    private void WaterPlant()
    {
        BoxLogic boxC = targetTile?.GetComponent<BoxLogic>();
        boxC?.GoWet();
    }

    private void DigGround()
    {
        BoxLogic boxC = targetTile?.GetComponent<BoxLogic>();
        boxC?.GetDug(transform);
    }

    private void SetGroundTile()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(new Vector3(transform.position.x, 1f, transform.position.z), Vector3.down, out hit, 4f, mask))
        {
            //groundTile?.GetComponent<SelectionHighlight>().DeSelect(); // deselect the old one
            groundTile = hit.collider.gameObject;
            //groundTile.GetComponent<SelectionHighlight>().Select(); // select the new one
        }
    }

    private bool GetNextTilePassable()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(transform.position, direction + Vector3.down, out hit, 4f, mask))
        {
            return hit.collider.gameObject.GetComponent<BoxLogic>().passable;
        }
        return false;
    }

    private void SetTargetTileF()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Ground");
        Vector3 center = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseDirection = new Vector3(Input.mousePosition.x - center.x, transform.position.y, Input.mousePosition.y - center.y).normalized;
        Debug.Log(mouseDirection);
        if (Physics.Raycast(transform.position, mouseDirection + Vector3.down * 0.4f, out hit, 4f, mask))
        {
            targetTile?.GetComponent<SelectionHighlight>().DeSelect(); // deselect the old one
            targetTile = hit.collider.gameObject;
            targetTile.GetComponent<SelectionHighlight>().Select(); // select the new one
        }
    }

    private void SetTargetTile()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Ground");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            if((hit.collider.transform.position - transform.position).magnitude < 1f)
            {
                targetTile?.GetComponent<SelectionHighlight>().DeSelect(); // deselect the old one
                targetTile = hit.collider.gameObject;
                targetTile.GetComponent<SelectionHighlight>().Select(); // select the new one
            }
            else
            {
                targetTile?.GetComponent<SelectionHighlight>().DeSelect(); // deselect the old one
                targetTile = groundTile;
                targetTile?.GetComponent<SelectionHighlight>().Select(); // select the new one
            }
        }
        if(targetTile == null)
        {
            targetTile = groundTile;
        }
    }

    private void SetImmobile(bool immobility)
    {
        immobile = immobility;
        direction = Vector3.zero;
    }

    private void OnDisable()
    {
        GameEvents.current.onImmobilizePlayer -= SetImmobile;
    }

}
