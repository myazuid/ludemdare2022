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

    private SpriteShapeController pathSprite;
    private SpriteShapeRenderer spriteShapeRenderer;
    [SerializeField] List<SpriteShape> pathSprites = new List<SpriteShape>();


    private void Awake()
    {
        pathSprite = GetComponent<SpriteShapeController>();
        spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
    }

    private void OnMouseDown()
    {
        UpgradePath();
    }

    private void OnMouseEnter()
    {
        spriteShapeRenderer.color = new Color(spriteShapeRenderer.color.r,
            spriteShapeRenderer.color.g, spriteShapeRenderer.color.b, 0.5f);
    }

    private void OnMouseExit()
    {
        spriteShapeRenderer.color = new Color(spriteShapeRenderer.color.r,
            spriteShapeRenderer.color.g, spriteShapeRenderer.color.b, 1f);
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
    }

    public void UpgradePath()
    {
        if (pathLevel == pathSprites.Count - 1)
        {
            print("Max level already");
            return;
        }

        pathLevel++;

        pathSprite.spriteShape = pathSprites[pathLevel];

        // to upgrade pathLevel
        PathManager.OnPathUpgraded?.Invoke(this);

        GameController.instance.SpendFromBalance(
            GameController.instance.pathUpgradeCosts[pathLevel]);
    }
}
