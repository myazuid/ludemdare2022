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

    private bool headBobDown;

    private float headBobSpeed;
    private float maxBobAmount = .1f;
    private float halfMaxBobAmount;

    private void Awake()
    {
        eyeRender.sprite = eyes[Random.Range(0, eyes.Length)];
        mouthRender.sprite = mouths[Random.Range(0, mouths.Length)];
        head.sprite = heads[Random.Range(0, heads.Length)];
        head.color = headColors[Random.Range(0, headColors.Length)];
        craft.sprite = crafts[Random.Range(0, crafts.Length)];
        craft.color = craftColors[Random.Range(0, craftColors.Length)];
        headBobSpeed = Random.Range(.01f, .1f);
        halfMaxBobAmount = maxBobAmount / 2f;
    }

    private void Update()
    {
        headContainer.transform.position += (headBobDown ? Vector3.down : Vector3.up) * Time.deltaTime * headBobSpeed;
        if ((headBobDown && headContainer.transform.localPosition.y <= -halfMaxBobAmount) || (!headBobDown && headContainer.transform.localPosition.y >= 0))
            headBobDown = !headBobDown;
    }
}