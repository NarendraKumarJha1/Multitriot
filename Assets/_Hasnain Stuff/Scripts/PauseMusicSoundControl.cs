using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMusicSoundControl : MonoBehaviour
{
    public Slider Music_Slider;
    public Text Music_Percentage_text;
    public Slider Sound_Slider;
    public Text Sound_Percentage_text;

    public Image musicImage = null;
    public Sprite musicOn = null, musicOff = null;
    public Text musicStatus = null;

    public Image soundImage = null;
    public Sprite soundOn = null, soundff = null;
    public Text soundStatus = null;

    [SerializeField] private AudioMixer mixer = null;

    private void OnEnable()
    {
        SetMusicSprits();
        SetSoundSprits();
    }

    public void MusicSliderValueChnaged(Slider slider)
    {
        MusicValue = slider.value;
        musicImage.sprite = slider.value > -50f ? musicOn : musicOff; //
        musicStatus.text = slider.value > -50f ? "ON" : "OFF"; //
        float a = Mathf.Abs(slider.value);
        float per = a / 50f; //
        per -= 1;
        if (per < 0)
            Music_Percentage_text.text = Mathf.Abs((per * 100)).ToString("##") + "%";
        else
            Music_Percentage_text.text = "0 %";

        if (slider.value <= -45)
        {
            mixer.SetFloat("MusicSFXVolume", -80.0f);
        }
        else
        {
            mixer.SetFloat("MusicSFXVolume", slider.value);
        }
    }

    public void SoundSliderValueChnaged(Slider slider)
    {
        SoundValue = slider.value;
        soundImage.sprite = slider.value > -50f ? soundOn : soundff; //
        soundStatus.text = slider.value > -50f ? "ON" : "OFF"; //

        float a = Mathf.Abs(slider.value);
        float per = a / 50f; //
        per -= 1;
        if (per < 0)
            Sound_Percentage_text.text = Mathf.Abs((per * 100)).ToString("##") + "%";
        else
            Sound_Percentage_text.text = "0 %";

        if (slider.value <= -45)
        {
            mixer.SetFloat("SoundSFXVolume", -80.0f);
        }
        else
        {
            mixer.SetFloat("SoundSFXVolume", slider.value);
        }
    }

    public void SoundPress()
    {
        SetSoundValue();
        SetSoundSprits();
    }
    public void MusicPress()
    {
        SetMusicValue();
        SetMusicSprits();
    }

    void SetMusicSprits()
    {
        Music_Slider.value = MusicValue;

        musicImage.sprite = Music_Slider.value > -50f ? musicOn : musicOff; //
        musicStatus.text = Music_Slider.value > -50f ? "ON" : "OFF"; //
    }

    void SetSoundSprits()
    {
        Sound_Slider.value = SoundValue;

        soundImage.sprite = Sound_Slider.value > -50f ? soundOn : soundff; //
        soundStatus.text = Sound_Slider.value > -50f ? "ON" : "OFF"; //
    }

    public void SetMusicValue()
    {
        if (MusicValue > -50f) //
        {
            MusicValue = -50f; //
        }
        else
        {
            MusicValue = 0;
        }
        mixer.SetFloat("MusicSFXVolume", MusicValue);
        Music_Slider.value = MusicValue;
        PlayerPrefs.SetFloat("MusicSFXValue", MusicValue);
    }

    public void SetSoundValue()
    {
        if (SoundValue > -50f) //
        {
            SoundValue = -50f; //
        }
        else
        {
            SoundValue = 0;
        }
        mixer.SetFloat("SoundSFXVolume", SoundValue);
        Sound_Slider.value = SoundValue;
        PlayerPrefs.SetFloat("SoundSFXValue", SoundValue);
    }

    float MusicValue
    {
        get
        {
            return PlayerPrefs.GetFloat("MusicSFXValue", 0);
        }
        set
        {
            PlayerPrefs.SetFloat("MusicSFXValue", value);
            PlayerPrefs.Save();
        }
    }
    float SoundValue
    {
        get
        {
            return PlayerPrefs.GetFloat("SoundSFXValue", 0);
        }
        set
        {
            PlayerPrefs.SetFloat("SoundSFXValue", value);
            PlayerPrefs.Save();
        }
    }

}
