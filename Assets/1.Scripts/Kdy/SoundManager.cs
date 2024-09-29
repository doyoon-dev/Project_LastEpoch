using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Inst = null;
    [SerializeField]
    AudioMixer m_audioMixer;
    [SerializeField]
    AudioSource m_audioSource;
    [SerializeField]
    AudioClip[] m_sfxClips;
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
                m_audioSource.clip = m_sfxClips[i];
                m_audioSource.Play();
            }
        }
    }
}
