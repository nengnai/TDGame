using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Music : MonoBehaviour
{

    public AudioSource musicA;
    public AudioSource musicB;
    public float 渐变时间;
    public MusicSetting musicSetting;

    
    private bool isFirsttime = false;
    private bool isPlayinA = false;
    private bool 正在渐变 = false;


    public void StartMusic()
    {
        if(isFirsttime == true) return;

        isFirsttime = true;

        musicA.Play();
        musicB.Play();

        musicA.volume = musicSetting.maxVolume;
        musicB.volume = 0f;

        isPlayinA = true;

    }

    public void ChangeMusic()
    {
        if(!isFirsttime) return;
        if(正在渐变 == true) return;

        StartCoroutine(changeAB());
    }

    IEnumerator changeAB()
    {
        正在渐变 = true;

        float timer = 0;

        while (timer < 渐变时间)
        {
            timer += Time.deltaTime;

            float t = timer / 渐变时间;

            // 让渐变更自然
            t = Mathf.SmoothStep(0f, 1f, t);

            if (isPlayinA)
            {
                musicA.volume = musicSetting.maxVolume * (1f - t);
                musicB.volume = musicSetting.maxVolume * t;
                Debug.Log("切换B面");
            }
            else
            {
                musicA.volume = musicSetting.maxVolume * t;
                musicB.volume = musicSetting.maxVolume * (1f - t);
                Debug.Log("切换A面");
            }

            yield return null;
        }

        isPlayinA = !isPlayinA;
        正在渐变 = false;
    }

}
