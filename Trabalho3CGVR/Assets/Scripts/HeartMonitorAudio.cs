using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Necessário para detectar o Socket

public class HeartMonitorAudio : MonoBehaviour
{
    [Header("Configuração de Áudio")]
    public AudioSource audioSource;
    public AudioClip normalRhythmClip; // O som de beep beep
    public AudioClip flatlineClip;     // O som da parada

    [Header("Referência")]
    // Referência ao Socket do peito
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor chestSocket;

    private void Start()
    {
        // Se esqueceu de arrastar, tenta pegar do próprio objeto
        if (chestSocket == null)
            chestSocket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();

        // Inscrever nos eventos do Socket
        chestSocket.selectExited.AddListener(OnHeartRemoved); // Quando tira
        chestSocket.selectEntered.AddListener(OnHeartInserted); // Quando coloca

        // Checar estado inicial
        if (chestSocket.hasSelection)
        {
            PlayNormal();
        }
        else
        {
            PlayFlatline();
        }
    }

    // Chamado automaticamente quando tira o coração velho
    private void OnHeartRemoved(SelectExitEventArgs args)
    {
        PlayFlatline();
    }

    // Chamado automaticamente quando coloca qualquer coração (velho ou novo)
    private void OnHeartInserted(SelectEnterEventArgs args)
    {
        PlayNormal();
    }

    private void PlayNormal()
    {
        audioSource.clip = normalRhythmClip;
        audioSource.loop = true;
        audioSource.Play();
        Debug.Log("Monitor: Ritmo Normal");
    }

    private void PlayFlatline()
    {
        audioSource.clip = flatlineClip;
        audioSource.loop = true;
        audioSource.Play();
        Debug.Log("Monitor: PARADA CARDÍACA!");
    }
}