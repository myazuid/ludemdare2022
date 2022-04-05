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
    public AudioMixerGroup musicSoundAudioMixerGroup;
    // Start is called before the first frame update
    void Start()
    {

        gameSoundSlider.onValueChanged.AddListener(GameSoundChanged);
        musicSoundSlider.onValueChanged.AddListener(MusicSoundChanged);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    private void OnCloseClicked()
    {
        container.SetActive(!container.activeSelf);
    }
    
    private void MusicSoundChanged(float arg0)
    {
        musicSoundAudioMixerGroup.audioMixer.SetFloat("VolumeMusic", arg0);
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
