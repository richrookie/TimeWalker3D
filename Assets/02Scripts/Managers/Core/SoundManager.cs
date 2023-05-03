using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private AudioSource[] _audioSources = new AudioSource[(int)Define.eSound.MaxCount];
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject root = new GameObject { name = "@Sound" };
        Object.DontDestroyOnLoad(root);

        string[] soundNames = System.Enum.GetNames(typeof(Define.eSound));
        for (int i = 0; i < soundNames.Length - 1; i++)
        {
            GameObject go = new GameObject { name = soundNames[i] };
            _audioSources[i] = go.AddComponent<AudioSource>();
            go.transform.parent = root.transform;
        }
        _audioSources[(int)Define.eSound.Bgm].loop = true;
    }

    public void Clear()
    {
        // foreach (AudioSource audioSource in _audioSources)
        // {
        //     audioSource.clip = null;
        //     audioSource.Stop();
        // }
        // _audioClips.Clear();
    }

    public void Play(string path, Define.eSound type, float volume = 1.0f)
    {
        if (!Managers.Data.UseSound) return;

        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, volume);
    }

    private void Play(AudioClip audioClip, Define.eSound type, float volume = 1.0f)
    {
        if (!Managers.Data.UseSound) return;

        if (audioClip == null)
            return;

        if (type == Define.eSound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.eSound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.volume = volume;
            audioSource.clip = audioClip;
            audioSource.Play();

            oriBgmVol = volume;
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)type];
            audioSource.volume = volume;
            audioSource.PlayOneShot(audioClip);
        }
    }

    private float oriBgmVol;
    public void BgmOnOff(bool isOn)
    {
        _audioSources[(int)Define.eSound.Bgm].volume = isOn ? oriBgmVol : 0;
    }

    private AudioClip GetOrAddAudioClip(string path, Define.eSound type)
    {
        AudioClip audioClip = null;

        if (type == Define.eSound.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                UnityEngine.Debug.Log(audioClip);
                _audioClips.TryAdd(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }
}
