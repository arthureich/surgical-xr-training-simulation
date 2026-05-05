using UnityEngine;
using System.Collections;

/// <summary>
/// Sistema de incisão realista para aplicação médica VR
/// Detecta contato com bisturi e faz fade out da pele
/// </summary>
public class ScalpelIncision : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Referência ao script AnatomyController que controla a transparência")]
    public AnatomyController anatomyController;

    [Tooltip("GameObject da pele que será desativado após o corte")]
    public GameObject skinObject;

    [Header("Configurações do Corte")]
    [Tooltip("Velocidade do fade out (segundos para chegar a 0)")]
    [Range(0.5f, 5f)]
    public float fadeOutDuration = 2f;

    [Tooltip("Tag da ferramenta que pode fazer o corte (ex: 'Tool')")]
    public string toolTag = "Tool";

    [Tooltip("Desativar objeto da pele quando transparência chegar a 0")]
    public bool deactivateSkinWhenInvisible = true;

    [Tooltip("Apenas desabilitar collider ao invés de desativar objeto")]
    public bool onlyDisableCollider = false;

    [Header("Feedback Visual/Sonoro")]
    [Tooltip("Partículas de sangue/corte (opcional)")]
    public ParticleSystem incisionParticles;

    [Tooltip("Som de corte (opcional)")]
    public AudioSource incisionSound;

    [Header("Debug")]
    [Tooltip("Mostrar logs no console")]
    public bool showDebugLogs = false;

    // Estado interno
    private bool isIncisionInProgress = false;
    private bool isIncisionComplete = false;
    private Collider skinCollider;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // Validações
        if (anatomyController == null)
        {
            Debug.LogError("ScalpelIncision: AnatomyController não foi atribuído!");
        }

        if (skinObject == null)
        {
            skinObject = gameObject;
            if (showDebugLogs)
                Debug.LogWarning("ScalpelIncision: skinObject não atribuído. Usando GameObject atual.");
        }

        // Cache do collider
        skinCollider = skinObject.GetComponent<Collider>();
        if (skinCollider == null)
        {
            Debug.LogError("ScalpelIncision: Nenhum Collider encontrado no skinObject!");
        }
    }

    /// <summary>
    /// Detecta quando o bisturi toca na pele
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Verificar se já foi cortado
        if (isIncisionComplete)
        {
            if (showDebugLogs)
                Debug.Log("ScalpelIncision: Incisão já foi completada anteriormente.");
            return;
        }

        // Verificar se é a ferramenta correta
        if (!other.CompareTag(toolTag))
        {
            if (showDebugLogs)
                Debug.Log($"ScalpelIncision: Objeto tocado não é Tool. Tag: {other.tag}");
            return;
        }

        // Iniciar corte se ainda não estiver em progresso
        if (!isIncisionInProgress)
        {
            if (showDebugLogs)
                Debug.Log("ScalpelIncision: Bisturi detectado! Iniciando incisão...");

            StartIncision(other.transform.position);
        }
    }

    /// <summary>
    /// Inicia o processo de incisão
    /// </summary>
    private void StartIncision(Vector3 contactPoint)
    {
        isIncisionInProgress = true;

        // Efeitos visuais
        if (incisionParticles != null)
        {
            incisionParticles.transform.position = contactPoint;
            incisionParticles.Play();
        }

        // Efeito sonoro
        if (incisionSound != null && !incisionSound.isPlaying)
        {
            incisionSound.Play();
        }

        // Iniciar fade out
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutSkin());
    }

    /// <summary>
    /// Coroutine que faz o fade out gradual da pele
    /// </summary>
    private IEnumerator FadeOutSkin()
    {
        if (anatomyController == null)
        {
            Debug.LogError("ScalpelIncision: AnatomyController não disponível!");
            yield break;
        }

        float elapsedTime = 0f;
        float startTransparency = 1f; // Assumindo que começa opaca

        // Obter transparência atual se possível
        if (anatomyController.targetRenderer != null && 
            anatomyController.targetRenderer.material.HasProperty("_Transparency"))
        {
            startTransparency = anatomyController.targetRenderer.material.GetFloat("_Transparency");
        }

        if (showDebugLogs)
            Debug.Log($"ScalpelIncision: Iniciando fade de {startTransparency} para 0 em {fadeOutDuration}s");

        // Fade gradual
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeOutDuration;

            // Curva de fade (pode ser linear ou com easing)
            float currentTransparency = Mathf.Lerp(startTransparency, 0f, t);

            // Atualizar transparência via AnatomyController
            anatomyController.UpdateTransparency(currentTransparency);

            yield return null;
        }

        // Garantir transparência final
        anatomyController.UpdateTransparency(0f);

        if (showDebugLogs)
            Debug.Log("ScalpelIncision: Fade completo. Pele invisível.");

        // Desativar pele/collider
        CompletIncision();
    }

    /// <summary>
    /// Finaliza a incisão desativando a pele
    /// </summary>
    private void CompletIncision()
    {
        isIncisionComplete = true;

        if (deactivateSkinWhenInvisible)
        {
            if (onlyDisableCollider && skinCollider != null)
            {
                // Apenas desabilitar collider (objeto permanece visível mas não colide)
                skinCollider.enabled = false;
                if (showDebugLogs)
                    Debug.Log("ScalpelIncision: Collider da pele desabilitado.");
            }
            else
            {
                // Desativar objeto completamente
                skinObject.SetActive(false);
                if (showDebugLogs)
                    Debug.Log("ScalpelIncision: Objeto da pele desativado.");
            }
        }

        // Parar efeitos
        if (incisionParticles != null && incisionParticles.isPlaying)
        {
            incisionParticles.Stop();
        }
    }

    /// <summary>
    /// Reseta o corpo para o estado inicial (para testes)
    /// </summary>
    public void ResetBody()
    {
        if (showDebugLogs)
            Debug.Log("ScalpelIncision: Resetando corpo...");

        // Parar coroutines ativas
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // Reativar objeto
        if (!skinObject.activeSelf)
        {
            skinObject.SetActive(true);
        }

        // Reativar collider
        if (skinCollider != null)
        {
            skinCollider.enabled = true;
        }

        // Restaurar transparência
        if (anatomyController != null)
        {
            anatomyController.UpdateTransparency(1f); // Totalmente opaco
        }

        // Resetar estados
        isIncisionInProgress = false;
        isIncisionComplete = false;

        // Parar efeitos
        if (incisionParticles != null && incisionParticles.isPlaying)
        {
            incisionParticles.Stop();
        }

        if (incisionSound != null && incisionSound.isPlaying)
        {
            incisionSound.Stop();
        }

        if (showDebugLogs)
            Debug.Log("ScalpelIncision: Reset completo!");
    }

    /// <summary>
    /// Verifica se a incisão foi completada
    /// </summary>
    public bool IsIncisionComplete()
    {
        return isIncisionComplete;
    }

    /// <summary>
    /// Permite ajustar a duração do fade em runtime
    /// </summary>
    public void SetFadeOutDuration(float duration)
    {
        fadeOutDuration = Mathf.Clamp(duration, 0.5f, 5f);
    }

#if UNITY_EDITOR
    // Visualização no Editor
    private void OnDrawGizmosSelected()
    {
        if (skinCollider != null)
        {
            Gizmos.color = isIncisionComplete ? Color.red : Color.green;
            Gizmos.DrawWireCube(skinCollider.bounds.center, skinCollider.bounds.size);
        }
    }
#endif
}