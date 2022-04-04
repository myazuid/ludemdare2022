using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateUpgradeButton : MonoBehaviour
{
    /*
     * 
     *  THIS NO LONGER DOES UPGRADES.
     *  It does the processing of travellers instead.
     * 
     */

    private GateController gateController;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        gateController = transform.parent.GetComponent<GateController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        gateController.ProcessOutboundTraveller();
    }

    private void OnMouseEnter()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);

        UIController.instance.showTooltip(gateController);
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        UIController.instance.hideTooltip();
    }
}
