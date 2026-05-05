using UnityEngine;

public class ClinicPortal : MonoBehaviour
{
    [Header("Destino")]
    public Transform clinicSpawnPoint; // Crie um objeto vazio lá na clínica para marcar onde o jogador cai
    public GameObject playerXROrigin;  // Arraste o seu XR Origin aqui

    [Header("Troca de Ambiente")]
    public GameObject lobbyGeo;   // O objeto pai com toda a sala Demo
    public GameObject clinicGeo;  // O objeto pai com a sala da clínica
    
    // Referência para o script que controla o Passthrough (opcional, se tiver)
    // public RoomToggle roomToggleScript; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportToClinic();
        }
    }

    public void TeleportToClinic()
    {
        // 1. Mover o Jogador
        // Desabilitar o CharacterController momentaneamente evita bugs de colisão no teleporte
        CharacterController charController = playerXROrigin.GetComponent<CharacterController>();
        if (charController) charController.enabled = false;

        playerXROrigin.transform.position = clinicSpawnPoint.position;
        playerXROrigin.transform.rotation = clinicSpawnPoint.rotation;

        if (charController) charController.enabled = true;

        // 2. Trocar o Visual (Otimização)
        if (lobbyGeo != null) lobbyGeo.SetActive(false); // Desliga o Lobby para economizar GPU
        if (clinicGeo != null) clinicGeo.SetActive(true); // Liga a Clínica
        
        // Se a clínica for Passthrough puro, garanta que o fundo da câmera fique preto (transparente) aqui
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.clear; // Alpha 0
    }
}