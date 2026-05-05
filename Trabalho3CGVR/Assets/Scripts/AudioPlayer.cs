using UnityEngine;

public class TocarAudioTrigger : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Pega o componente AudioSource anexado a este objeto
        audioSource = GetComponent<AudioSource>();

        // Verifica se o AudioSource foi encontrado
        if (audioSource == null)
        {
            Debug.LogError("AudioSource não encontrado no objeto! Certifique-se de que ele esteja anexado.");
        }
    }

    // Chamado quando outro collider entra no trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Opcional: Verifica se o objeto que entrou tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Toca o áudio uma vez
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    // Opcional: Use OnTriggerExit para parar o áudio quando o jogador sair da área
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         if (audioSource != null && audioSource.isPlaying)
    //         {
    //             audioSource.Stop();
    //         }
    //     }
    // }
}