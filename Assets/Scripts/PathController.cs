using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PathController : MonoBehaviour
{
    public GameObject gate1; // one of the gates
    public GameObject gate2; // the second gate
    public int pathLevel = 0; // the upgrade level (starts at 0)
    public float pathSpeed = 1; // between 0 and 1
    public int MaxPathLevel
    {
        get
        {
            return GameController.instance.pathLevels.Count - 1;
        }
    }

    private SpriteShapeController pathSprite;
    private SpriteShapeRenderer spriteShapeRenderer;


    private void Awake()
    {
        pathSprite = GetComponent<SpriteShapeController>();
        spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
    }

    private void Start()
    {
        SetSpriteShapePathPoints();
    }

    private void OnMouseDown()
    {
        UpgradePath();

        UIController.instance.showTooltip(this); // to refresh it
    }

    private void OnMouseEnter()
    {
        spriteShapeRenderer.color = new Color(spriteShapeRenderer.color.r,
            spriteShapeRenderer.color.g, spriteShapeRenderer.color.b, 0.5f);
        
        UIController.instance.showTooltip(this);

        if (!UIController.instance._firstPathTutorial)
        {
            if (UIController.instance._upgradedGatesOnce)
            {
                UIController.instance._firstPathTutorial = true;
                UIController.instance.ShowTutorial(this);
            }
        }
    }

    private void OnMouseExit()
    {
        spriteShapeRenderer.color = new Color(spriteShapeRenderer.color.r,
            spriteShapeRenderer.color.g, spriteShapeRenderer.color.b, 1f);
        
        UIController.instance.hideTooltip();
    }

    public void SetPathSpeed(float _speed)
    {
        var clampedSpeed = Mathf.Clamp(_speed, 0, 1);
        pathSpeed = clampedSpeed;
    }

    public void SetSpriteShapePathPoints()
    {
        if (gate1 == null || gate2 == null)
        {
            Debug.LogError("You haven't set the gates of this path yet");
            return;
        }

        int index = 0;
        pathSprite.spline.Clear();
        var pos1 = new Vector2(gate1.transform.position.x,
            gate1.transform.position.y);
        pathSprite.spline.InsertPointAt(index++, pos1);
        var pos2 = new Vector2(gate2.transform.position.x,
            gate2.transform.position.y);
        pathSprite.spline.InsertPointAt(index++, pos2);
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void UpgradePath()
    {
        if (pathLevel < MaxPathLevel)
        {
            var success = GameController.instance.SpendFromBalance(
            GameController.instance.pathLevels[pathLevel].upgradeCost);
            if (success)
            {
                pathLevel++;

                pathSprite.spriteShape =
                    GameController.instance.pathLevels[pathLevel].sprite;

                // to upgrade pathLevel
                PathManager.OnPathUpgraded?.Invoke(this);
                UIController.instance.showTooltip(this);
            }
        }   
    }
}
