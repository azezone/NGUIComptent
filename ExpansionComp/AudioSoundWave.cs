﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioSoundWave : MonoBehaviour
{
    private Image[] barsSprites;
    [Range(0, 10)]
    public float colorMultiplyer = 1;
    [Range(0, 1)]
    public float s = 1;
    [Range(0, 1)]
    public float v = 1;
    private int index = 0;
    private float musicLength;

    private AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        barsSprites = transform.GetComponentsInChildren<Image>();

        StartCoroutine(ShowAudioWave(audio));
    }

    private IEnumerator ShowAudioWave(AudioSource source)
    {
        while (source.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
            Visulization(source);
        }
    }

    void Visulization(AudioSource source)
    {
        if (barsSprites != null && barsSprites.Length > 0 && barsSprites.Length <= 64)
        {
            float[] musicData = new float[64];
            source.GetSpectrumData(musicData, 0, FFTWindow.Triangle);
            int i = 0;
            int len = barsSprites.Length;
            while (i < len)
            {
                barsSprites[i].transform.localScale = new Vector3(0.8f, 10 * musicData[i], 1);
                barsSprites[i].color = HSVtoRGB(musicData[i] * colorMultiplyer, s, v, 1);
                i++;
            }
        }
    }

    #region Static
    public static Color HSVtoRGB(float hue, float saturation, float value, float alpha)
    {
        while (hue > 1f)
        {
            hue -= 1f;
        }
        while (hue < 0f)
        {
            hue += 1f;
        }
        while (saturation > 1f)
        {
            saturation -= 1f;
        }
        while (saturation < 0f)
        {
            saturation += 1f;
        }
        while (value > 1f)
        {
            value -= 1f;
        }
        while (value < 0f)
        {
            value += 1f;
        }
        if (hue > 0.999f)
        {
            hue = 0.999f;
        }
        if (hue < 0.001f)
        {
            hue = 0.001f;
        }
        if (saturation > 0.999f)
        {
            saturation = 0.999f;
        }
        if (saturation < 0.001f)
        {
            return new Color(value * 255f, value * 255f, value * 255f);

        }
        if (value > 0.999f)
        {
            value = 0.999f;
        }
        if (value < 0.001f)
        {
            value = 0.001f;
        }

        float h6 = hue * 6f;
        if (h6 == 6f)
        {
            h6 = 0f;
        }
        int ihue = (int)(h6);
        float p = value * (1f - saturation);
        float q = value * (1f - (saturation * (h6 - (float)ihue)));
        float t = value * (1f - (saturation * (1f - (h6 - (float)ihue))));
        switch (ihue)
        {
            case 0:
                return new Color(value, t, p, alpha);
            case 1:
                return new Color(q, value, p, alpha);
            case 2:
                return new Color(p, value, t, alpha);
            case 3:
                return new Color(p, q, value, alpha);
            case 4:
                return new Color(t, p, value, alpha);
            default:
                return new Color(value, p, q, alpha);
        }
    }
    #endregion
}