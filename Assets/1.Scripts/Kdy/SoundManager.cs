using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Inst = null;
    [SerializeField]
    AudioMixer m_audioMixer;
    [SerializeField]
    public AudioSource m_sfxAudioSource;
    [SerializeField]
    AudioSource m_bgmAudioSource;
    [SerializeField]
    AudioClip[] m_sfxClips;
    [SerializeField]
    Slider m_masterSlider;
    [SerializeField]
    Slider m_gameSlider;
    [SerializeField]
    Slider m_bgmSlider;
    // Start is called before the first frame update
    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlaySfx(string name)
    {
        for(int i = 0; i < m_sfxClips.Length; i++)
        {
            if (m_sfxClips[i].name == name)
            {
                m_sfxAudioSource.clip = m_sfxClips[i];
                m_sfxAudioSource.Play();
                //m_sfxAudioSource.PlayOneShot(m_sfxAudioSource.clip);
            }
        }
    }

    public void PlayBgm(string name)
    {
        for (int i = 0; i < m_sfxClips.Length; i++)
        {
            if (m_sfxClips[i].name == name)
            {
                m_bgmAudioSource.clip = m_sfxClips[i];
                m_bgmAudioSource.Play();
            }
        }
    }

    public void SetMasterVolume()
    {
        float sound = m_masterSlider.value;
        if (sound == -40.0f) m_audioMixer.SetFloat("MASTER", -80);
        else m_audioMixer.SetFloat("MASTER", sound);
    }

    public void SetBgmVolume()
    {
        float sound = m_bgmSlider.value;
        if (sound == -40.0f) m_audioMixer.SetFloat("BGM", -80);
        else m_audioMixer.SetFloat("BGM", sound);
    }

    public void SetSfxVolume()
    {
        float sound = m_gameSlider.value;
        if (sound == -40.0f) m_audioMixer.SetFloat("GAME", -80);
        else m_audioMixer.SetFloat("GAME", sound);
    }

    public void StopSfxSound()
    {
        m_sfxAudioSource.Stop();
    }
}
