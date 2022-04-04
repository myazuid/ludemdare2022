using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    private float ver, hor;
    [SerializeField] private float moveKeySpeed;
    [SerializeField] private float zoomScrollSpeed;

    private float maxZoom = 2, minZoom = 25;

    [SerializeField] private CinemachineVirtualCamera virtualCam;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Key input to scroll
        ver = Input.GetAxisRaw("Vertical");
        hor = Input.GetAxisRaw("Horizontal");
        var move = new Vector2(hor, ver).normalized;
        transform.Translate(move * moveKeySpeed * Time.deltaTime);

        // Zooming
        var scroll = Input.mouseScrollDelta.y;
        var currentSize = virtualCam.m_Lens.OrthographicSize;
        var clampedSize = Mathf.Clamp(currentSize - scroll * zoomScrollSpeed, maxZoom, minZoom);
        virtualCam.m_Lens.OrthographicSize = clampedSize;
    }
}
