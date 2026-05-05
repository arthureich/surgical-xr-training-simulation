using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gerenciador global para controlar múltiplas incisões e UI
/// Útil para controlar várias partes do corpo ou resetar tudo
/// </summary>
public class IncisionManager : MonoBehaviour
{
    [Header("Referências das Incisões")]
    [Tooltip("Todos os scripts de incisão na cena")]
    public ScalpelIncision[] allIncisions;

    [Header("UI (Opcional)")]
    [Tooltip("Botão para resetar todas as incisões")]
    public Button resetButton;

    [Tooltip("Texto de feedback para o jogador")]
    public TMPro.TextMeshProUGUI feedbackText;

    [Header("Configurações")]
    [Tooltip("Resetar automaticamente ao recarregar a cena")]
    public bool autoResetOnSceneLoad = true;

    [Tooltip("Tecla de atalho para reset (apenas para debug em editor)")]
    public KeyCode resetHotkey = KeyCode.R;

    private void Start()
    {
        // Encontrar automaticamente todas as incisões se não foram atribuídas
        if (allIncisions == null || allIncisions.Length == 0)
        {
            allIncisions = FindObjectsOfType<ScalpelIncision>();
            Debug.Log($"IncisionManager: {allIncisions.Length} incisões encontradas automaticamente.");
        }

        // Configurar botão de reset
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetAllIncisions);
        }

        // Auto-reset se configurado
        if (autoResetOnSceneLoad)
        {
            ResetAllIncisions();
        }

        UpdateFeedback("Toque o bisturi na pele para iniciar a incisão.");
    }

    private void Update()
    {
        // Atalho de debug (apenas em editor)
#if UNITY_EDITOR
        if (Input.GetKeyDown(resetHotkey))
        {
            Debug.Log("IncisionManager: Atalho de reset pressionado!");
            ResetAllIncisions();
        }
#endif

        // Verificar progresso das incisões
        CheckIncisionProgress();
    }

    /// <summary>
    /// Reseta todas as incisões registradas
    /// </summary>
    public void ResetAllIncisions()
    {
        Debug.Log("IncisionManager: Resetando todas as incisões...");

        foreach (var incision in allIncisions)
        {
            if (incision != null)
            {
                incision.ResetBody();
            }
        }

        UpdateFeedback("Corpo resetado. Pronto para nova incisão.");
    }

    /// <summary>
    /// Verifica o progresso das incisões e atualiza UI
    /// </summary>
    private void CheckIncisionProgress()
    {
        if (allIncisions == null || allIncisions.Length == 0) return;

        int completedCount = 0;
        foreach (var incision in allIncisions)
        {
            if (incision != null && incision.IsIncisionComplete())
            {
                completedCount++;
            }
        }

        // Todas as incisões completas
        if (completedCount == allIncisions.Length && completedCount > 0)
        {
            UpdateFeedback($"Todas as incisões completas! ({completedCount}/{allIncisions.Length})");
        }
        else if (completedCount > 0)
        {
            UpdateFeedback($"Incisões: {completedCount}/{allIncisions.Length} completas");
        }
    }

    /// <summary>
    /// Atualiza o texto de feedback na UI
    /// </summary>
    private void UpdateFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }

    /// <summary>
    /// Reseta uma incisão específica por índice
    /// </summary>
    public void ResetIncisionByIndex(int index)
    {
        if (index >= 0 && index < allIncisions.Length && allIncisions[index] != null)
        {
            allIncisions[index].ResetBody();
            Debug.Log($"IncisionManager: Incisão {index} resetada.");
        }
    }

    /// <summary>
    /// Retorna quantas incisões foram completadas
    /// </summary>
    public int GetCompletedIncisionsCount()
    {
        int count = 0;
        foreach (var incision in allIncisions)
        {
            if (incision != null && incision.IsIncisionComplete())
            {
                count++;
            }
        }
        return count;
    }
}