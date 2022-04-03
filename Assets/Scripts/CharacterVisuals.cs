using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterVisuals : MonoBehaviour
{
    public Transform headContainer;
    public SpriteRenderer head;
    public SpriteRenderer eyeRender;
    public SpriteRenderer mouthRender;
    public SpriteRenderer craft;


    public Sprite[] heads;
    public Color[] headColors;

    public Sprite[] eyes;
    public Sprite[] mouths;


    public Sprite[] crafts;
    public Color[] craftColors;

    private bool craftBobDown;
    private float craftBobSpeed;
    private float craftMaxBobAmount = .2f;
    private float craftHalfMaxBobAmount;
    
    private bool headBobDown;
    private float headBobSpeed;
    private float maxBobAmount = .1f;
    private float halfMaxBobAmount;
    private Vector2 lastPosition;
    private Vector3 parentLastPosition;
    private Vector3 startingScale;

    private Transform myParent;

    private void Awake()
    {
        eyeRender.sprite = eyes[Random.Range(0, eyes.Length)];
        mouthRender.sprite = mouths[Random.Range(0, mouths.Length)];
        head.sprite = heads[Random.Range(0, heads.Length)];
        head.color = headColors[Random.Range(0, headColors.Length)];
        craft.sprite = crafts[Random.Range(0, crafts.Length)];
        craft.color = craftColors[Random.Range(0, craftColors.Length)];
        headBobSpeed = Random.Range(.05f, .1f);
        halfMaxBobAmount = maxBobAmount / 2f;
        craftBobSpeed = Random.Range(.03f, .07f);
        craftHalfMaxBobAmount = craftMaxBobAmount / 2f;
        startingScale = transform.localScale;
        myParent = transform.parent;
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        headContainer.transform.position += (headBobDown ? Vector3.down : Vector3.up) * Time.deltaTime * headBobSpeed;
        if ((headBobDown && headContainer.transform.localPosition.y <= -halfMaxBobAmount) || (!headBobDown && headContainer.transform.localPosition.y >= 0))
            headBobDown = !headBobDown;

        if (Vector2.Distance(myParent.transform.position, parentLastPosition) < .005f * Time.deltaTime)
        {
            transform.position += (craftBobDown ? Vector3.down : Vector3.up) * Time.deltaTime * craftBobSpeed;
            if ((craftBobDown && transform.localPosition.y <= -craftHalfMaxBobAmount) || (!craftBobDown && transform.localPosition.y >= 0))
                craftBobDown = !craftBobDown;
        }

        if (lastPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(-startingScale.x, startingScale.y, startingScale.z);
        }

        lastPosition = transform.position;
        parentLastPosition = myParent.transform.position;
    }
}