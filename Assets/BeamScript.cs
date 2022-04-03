using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BeamScript : MonoBehaviour
{
    public float lifeTime = .3f;
    private float _lifeLeft;
    public Light2D light;
    private float lightIntensity;

    public AnimationCurve curve;
    // Start is called before the first frame update
    void Start()
    {
        lightIntensity = light.intensity;
        transform.Translate(0f,0f,5f);
    }

    // Update is called once per frame
    void Update()
    {
        _lifeLeft += Time.deltaTime;
        transform.localScale = new Vector3(curve.Evaluate(_lifeLeft / lifeTime), transform.localScale.y, transform.localScale.z);
        light.intensity = curve.Evaluate(_lifeLeft / lifeTime) * lightIntensity;

        if (_lifeLeft >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}