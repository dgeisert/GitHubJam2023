using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : Menu
{
    private float previousTimescale;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider audioSlider;

    private void Start()
    {
        // Load saved volume values or set default values
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedOverallVolume = PlayerPrefs.GetFloat("OverallVolume", 0.5f);
        musicSlider.value = savedMusicVolume;
        audioSlider.value = savedOverallVolume;
        SetMusicVolume(savedMusicVolume);
        SetOverallVolume(savedOverallVolume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetOverallVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("OverallVolume", volume);
    }

    public void Close()
    {
        manager.RemoveOverlay();
        manager.ReEnableBaseMenu();
    }

    public override void Show()
    {
        base.Show();
        previousTimescale = Time.timeScale;
        Time.timeScale = 0;
    }
    public override void Hide()
    {
        base.Hide();
        Time.timeScale = previousTimescale;
    }
}
