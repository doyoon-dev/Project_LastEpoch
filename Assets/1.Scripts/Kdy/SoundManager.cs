using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    const int MaxSfxPlayCount = 3;

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
    Dictionary<string, int> m_sfxPlayList = new Dictionary<string, int>();

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
    
    //public void PlaySfx(string name)
    //{
    //    for(int i = 0; i < m_sfxClips.Length; i++)
    //    {
    //        if (m_sfxClips[i].name == name)
    //        {
    //            m_sfxAudioSource.clip = m_sfxClips[i];
    //            //m_sfxAudioSource.Play();
    //            m_sfxAudioSource.PlayOneShot(m_sfxAudioSource.clip);
    //        }
    //    }
    //}

    public void PlaySfx(string name)
    {
        // 동일한 사운드 출력 될 때
        if (m_sfxPlayList.ContainsKey(name))
        {
            // 같은 사운드 최대 플레이 개수(3개)보다 현재 플레이 중인 리스트 개수가 많으면 사운드 추가 안함
            if (m_sfxPlayList[name] >= MaxSfxPlayCount)
            {
                return;
            }
            // 아니면 리스트에 사운드 증가
            else
            {
                m_sfxPlayList[name]++;
            }
        }
        // 다른 사운드 출력 될 때
        else
        {
            m_sfxPlayList.Add(name, 1);      // sfx : 현재 사운드, 1 : 플레이 중인 사운드 개수
        }
        for (int i = 0; i < m_sfxClips.Length; i++)
        {
            if (m_sfxClips[i].name == name)
            {
                m_sfxAudioSource.clip = m_sfxClips[i];
                m_sfxAudioSource.PlayOneShot(m_sfxAudioSource.clip);
                StartCoroutine(RemoveSfxPlayList(name, m_sfxClips[i].length));
            }
        }
        
    }

    IEnumerator RemoveSfxPlayList(string name, float length)
    {
        // 실행 중인 사운드(1개)가 끝날때까지 대기
        yield return new WaitForSeconds(length);
        // 사운드가 리스트에 남아 있으면 개수를 줄이고 아니면 제거
        if (m_sfxPlayList[name] > 1)
        {
            m_sfxPlayList[name]--;
        }
        else
        {
            m_sfxPlayList.Remove(name);
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

    public void StopSfxSound(string name)
    {
        for (int i = 0; i < m_sfxClips.Length; i++)
        {
            if (m_sfxClips[i].name == name)
            {
                m_bgmAudioSource.clip = m_sfxClips[i];
                m_bgmAudioSource.Stop();
            }
        }
    }
}
