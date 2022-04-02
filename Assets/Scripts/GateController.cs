using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public List<GameObject> outboundTravellerQueue =
        new List<GameObject>();

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        ProcessOutboundTraveller(); 
    }

    private void OnMouseEnter()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 1f);
    }

    private void ProcessOutboundTraveller()
    {
        // send payment fee to the GameController

        // and tell gamecontroller to remove from its list

        var processedTraveller = outboundTravellerQueue[0];

        outboundTravellerQueue.RemoveAt(0);

        Destroy(processedTraveller);

        // shift the remaining ones
        foreach (var traveller in outboundTravellerQueue)
        {
            traveller.transform.position =
                new Vector2(traveller.transform.position.x - 0.1f,
                traveller.transform.position.y);
        }
    }
}
