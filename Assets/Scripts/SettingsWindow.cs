using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SettingsWindow : MonoBehaviour
{

    public Slider Music_Slider;
    public Text Music_Percentage_text;
    public Slider Sound_Slider;
    public Text Sound_Percentage_text;

    public Slider volumeSlider;
    public Text messageText;
    [SerializeField] private AudioMixer mixer = null;
    public GameObject controlsObject = null;
    private void Start()
    {
        //if (PlayerPrefs.HasKey ("MASTER_VOLUME")) {
        //    volumeSlider.value = PlayerPrefs.GetInt ("MASTER_VOLUME");
        //}
        if (PlayerPrefs.GetFloat("SoundSFXValue") <= -45)
        {
            mixer.SetFloat("SoundSFXVolume", -80.0f);
        }
        else
        {
            mixer.SetFloat("SoundSFXVolume", PlayerPrefs.GetFloat("SoundSFXValue"));
        }

        if (PlayerPrefs.GetFloat("MusicSFXValue") <= -45)
        {
            mixer.SetFloat("MusicSFXVolume", -80.0f);
        }
        else
        {
            mixer.SetFloat("MusicSFXVolume", PlayerPrefs.GetFloat("MusicSFXValue"));
        }

    }
    private void OnEnable()
    {
        //mixer.SetFloat("MusicSFXVolume", MusicValue);
        //mixer.SetFloat("SoundSFXVolume", SoundValue);
        SetMusicSprits();
        SetSoundSprits();
        SetKnob();
        controlsObject.SetActive(!Constants.IsWindowEditor());
        //Debug.Log("level is: " + QualitySettings.GetQualityLevel());
    }
    public Image[] knobImages = null;
    public Sprite checkSprite = null, uncheckSprite = null;

    void SetKnob()
    {
        knobImages[0].sprite = QualitySettings.GetQualityLevel() == 0 ? checkSprite : uncheckSprite;
        knobImages[1].sprite = QualitySettings.GetQualityLevel() == 2 ? checkSprite : uncheckSprite;
        knobImages[2].sprite = QualitySettings.GetQualityLevel() == 5 ? checkSprite : uncheckSprite;
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
        //    PlayerPrefs.SetFloat("SoundSFXValue", slider.value);
        //    PlayerPrefs.Save();
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

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        SetKnob();
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
        //Save();
    }

    public Image musicImage = null;
    public Sprite musicOn = null, musicOff = null;
    public Text musicStatus = null;

    void SetMusicSprits()
    {
        //musicImage.sprite = MusicValue == 0 ? musicOn : musicOff;
        //musicStatus.text = MusicValue == 0 ? "ON" : "OFF";
        Music_Slider.value = MusicValue;

        musicImage.sprite = Music_Slider.value > -50f ? musicOn : musicOff; //
        musicStatus.text = Music_Slider.value > -50f ? "ON" : "OFF"; //
    }

    public Image soundImage = null;
    public Sprite soundOn = null, soundff = null;
    public Text soundStatus = null;

    void SetSoundSprits()
    {
        ////soundImage.sprite = SoundValue == 0 ? soundOn : soundff;
        ////soundStatus.text = SoundValue == 0 ? "ON" : "OFF";
        Sound_Slider.value = SoundValue;

        soundImage.sprite = Sound_Slider.value > -50f ? soundOn : soundff; //
        soundStatus.text = Sound_Slider.value > -50f ? "ON" : "OFF"; //
    }
    public void SetPlayerControls(int controlsType)
    {
        if (controlsType == 0)
            RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.TouchScreen;
        else if (controlsType == 1)
            RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.SteeringWheel;
        else if (controlsType == 2)
            RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.Gyro;
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