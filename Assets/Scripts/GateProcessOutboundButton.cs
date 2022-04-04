using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateProcessOutboundButton : MonoBehaviour
{
    private GateController gateController;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        gateController = transform.parent.GetComponent<GateController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (gateController.outboundTravellerQueue.Count > 0)
        {
            gateController.ProcessOutboundTraveller();
        }
    }

    private void OnMouseEnter()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);

        UIController.instance.showTooltip(this);
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        UIController.instance.hideTooltip();
    }
}
