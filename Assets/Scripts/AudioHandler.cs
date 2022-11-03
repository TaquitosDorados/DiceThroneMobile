using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioHandler : MonoBehaviour
{
    private AudioSource music;
    public Slider volSlider;
    public Toggle muteToggle;
    // Start is called before the first frame update
    void Start()
    {
        volSlider.value = PlayerPrefs.GetFloat("Volume");
         

        music = GetComponent<AudioSource>();
        music.volume = PlayerPrefs.GetFloat("Volume");
        if (PlayerPrefs.GetInt("IsMuted") == 1)
        {
            music.mute = true;
            muteToggle.isOn = false;
        }
        else
        {
            music.mute = false;
            muteToggle.isOn = true;
        }
    }

    public void ChangeVolume(float _value)
    {
        music = GetComponent<AudioSource>();
        music.volume = _value;
        PlayerPrefs.SetFloat("Volume", _value);
    }

    public void Mute(bool muted)
    {
        music.mute = !muted;
        if (muted)
        {
            PlayerPrefs.SetInt("IsMuted", 0);
        } else
        {
            PlayerPrefs.SetInt("IsMuted", 1);
        }
    }
}
