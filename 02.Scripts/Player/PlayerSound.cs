using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSound : NetworkBehaviour {
    public enum AudioClipNum { AR, SG, SR, WALK, RUN, FLY, JUMP, DAMAGE, GET_ITEM, ITEM_HP, ITEM_ENERGY, ITEM_UPGRADE}
    
    [SerializeField]
    private AudioClip[] audioClips;

    [SerializeField]
    private AudioSource audioSource;

    public AudioClip[] GetAudioClips { get; private set; }
    public AudioSource GetAudioSource { get; private set; }

    private void Start()
    {
        GetAudioClips = audioClips;
        GetAudioSource = audioSource;
        GetAudioSource.loop = true;
    }

    [Client]
    public void PlayOneShot(string playerName, int audioClipNum, float volume)
    {
        if (gameObject.name == GameDirector.Instance.LocalPlayerName)
        {
            GetComponent<PlayerSound>().CmdPlayOneShot(playerName, audioClipNum, volume);
        }
    }

    [Command]
    private void CmdPlayOneShot(string PlayerName, int audioClipNum, float volume)
    {
        GameDirector.Instance.players[PlayerName].GetComponent<PlayerSound>().RpcPlayOneShot(audioClipNum, volume);
    }

    [ClientRpc]
    private void RpcPlayOneShot(int audioClipNum, float volume)
    {
        audioSource.PlayOneShot(audioClips[audioClipNum], volume);
    }

    [Client]
    public void Play(string playerName, int audioClipNum, float volume)
    {
        if (gameObject.name == GameDirector.Instance.LocalPlayerName)
        {
            GetComponent<PlayerSound>().CmdPlay(playerName, audioClipNum, volume);
        }
    }

    [Command]
    private void CmdPlay(string PlayerName, int audioClipNum, float volume)
    {
        GameDirector.Instance.players[PlayerName].GetComponent<PlayerSound>().RpcPlay(audioClipNum, volume);
    }

    [ClientRpc]
    private void RpcPlay(int audioClipNum, float volume)
    {
        audioSource.loop = true;
        audioSource.clip = audioClips[audioClipNum];
        audioSource.Play();
    }

    [Client]
    public void Stop()
    {
        if (gameObject.name == GameDirector.Instance.LocalPlayerName)
        {
            CmdStop(gameObject.name);
        }
    }

    [Command]
    private void CmdStop(string PlayerName)
    {
        GameDirector.Instance.players[PlayerName].GetComponent<PlayerSound>().RpcStop();
    }

    [ClientRpc]
    private void RpcStop()
    {
        audioSource.loop = false;
    }
}
