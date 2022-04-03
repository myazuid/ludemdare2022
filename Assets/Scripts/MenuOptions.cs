using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{
    public GameObject container;
    public Slider gameSoundSlider;
    public Slider musicSoundSlider;

    public Button closeButton;

    public AudioMixerGroup gameSoundAudioMixerGroup;
    // Start is called before the first frame update
    void Start()
    {

        gameSoundSlider.onValueChanged.AddListener(GameSoundChanged);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    private void OnCloseClicked()
    {
        container.SetActive(!container.activeSelf);
    }

    private void GameSoundChanged(float arg0)
    {
        gameSoundAudioMixerGroup.audioMixer.SetFloat("Volume", arg0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            container.SetActive(!container.activeSelf);
        }
    }
}
