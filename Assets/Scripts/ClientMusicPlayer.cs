using UnityEngine;

[RequireComponent(typeof(AudioClip))]
public class ClientMusicPlayer : Singleton<ClientMusicPlayer>
{
    [SerializeField] private AudioClip _eatAudioClip;

    private AudioSource _audioSource;

    public override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayerEatAudioClip()
    {
        _audioSource.clip = _eatAudioClip;
        _audioSource.Play();
    }
}