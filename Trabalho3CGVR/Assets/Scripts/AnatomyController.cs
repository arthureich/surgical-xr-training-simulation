using UnityEngine;

/// <summary>
/// Controlador para ajustar a transparência do shader de holograma de anatomia
/// com suporte para alternar entre modo normal e holográfico
/// </summary>
public class AnatomyController : MonoBehaviour
{
    [Header("Configurações do Objeto")]
    [Tooltip("Renderer do objeto de anatomia (ex: coração)")]
    public Renderer targetRenderer;

    [Header("Materiais")]
    [Tooltip("Material normal (textura realista)")]
    public Material normalMaterial;
    
    [Tooltip("Material holográfico (efeito sci-fi)")]
    public Material hologramMaterial;

    [Header("Configurações de Transparência")]
    [Range(0f, 1f)]
    [Tooltip("Valor inicial da transparência (0 = invisível, 1 = opaco)")]
    public float initialTransparency = 0.5f;

    [Header("Modo de Operação")]
    [Tooltip("Ativar modo holográfico automaticamente ao ajustar transparência")]
    public bool autoSwitchToHologram = true;

    // Nome da propriedade no shader
    private const string TRANSPARENCY_PROPERTY = "_Transparency";
    
    // Cache do material atual
    private Material currentMaterial;
    private bool isHologramMode = false;

    private void Start()
    {
        // Validação
        if (targetRenderer == null)
        {
            Debug.LogError("AnatomyController: Nenhum Renderer foi atribuído!");
            return;
        }

        // Começar no modo normal se tiver material normal
        if (normalMaterial != null)
        {
            SetNormalMode();
        }
        else
        {
            // Se não tiver material normal, usar o holográfico
            SetHologramMode();
            UpdateTransparency(initialTransparency);
        }
    }

    /// <summary>
    /// Atualiza a transparência do material do holograma
    /// </summary>
    /// <param name="value">Valor de transparência entre 0 (invisível) e 1 (opaco)</param>
    public void UpdateTransparency(float value)
    {
        // Validação do renderer
        if (targetRenderer == null)
        {
            Debug.LogWarning("AnatomyController: Renderer não está atribuído!");
            return;
        }

        // Garantir que o valor está no range correto
        value = Mathf.Clamp01(value);

        // Mudar para modo holográfico automaticamente se configurado
        if (autoSwitchToHologram && !isHologramMode && value < 1f)
        {
            SetHologramMode();
        }

        // Atualizar a propriedade do shader
        if (currentMaterial != null && currentMaterial.HasProperty(TRANSPARENCY_PROPERTY))
        {
            currentMaterial.SetFloat(TRANSPARENCY_PROPERTY, value);
        }

        // Voltar para modo normal se opacidade total
        if (autoSwitchToHologram && isHologramMode && value >= 0.99f)
        {
            SetNormalMode();
        }
    }

    /// <summary>
    /// Ativa o modo de visualização normal (material realista)
    /// </summary>
    public void SetNormalMode()
    {
        if (normalMaterial == null || targetRenderer == null) return;

        targetRenderer.material = normalMaterial;
        currentMaterial = targetRenderer.material;
        isHologramMode = false;
    }

    /// <summary>
    /// Ativa o modo holográfico (efeito sci-fi transparente)
    /// </summary>
    public void SetHologramMode()
    {
        if (hologramMaterial == null || targetRenderer == null) return;

        targetRenderer.material = hologramMaterial;
        currentMaterial = targetRenderer.material;
        isHologramMode = true;
    }

    /// <summary>
    /// Alterna entre modo normal e holográfico
    /// </summary>
    public void ToggleMode()
    {
        if (isHologramMode)
        {
            SetNormalMode();
        }
        else
        {
            SetHologramMode();
        }
    }

    /// <summary>
    /// Define a transparência com animação suave
    /// </summary>
    public void SetTransparencySmooth(float targetValue, float duration = 1f)
    {
        if (currentMaterial != null)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateTransparency(targetValue, duration));
        }
    }

    private System.Collections.IEnumerator AnimateTransparency(float targetValue, float duration)
    {
        if (!currentMaterial.HasProperty(TRANSPARENCY_PROPERTY)) yield break;

        float startValue = currentMaterial.GetFloat(TRANSPARENCY_PROPERTY);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentValue = Mathf.Lerp(startValue, targetValue, t);
            currentMaterial.SetFloat(TRANSPARENCY_PROPERTY, currentValue);
            yield return null;
        }

        currentMaterial.SetFloat(TRANSPARENCY_PROPERTY, targetValue);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying && currentMaterial != null)
        {
            UpdateTransparency(initialTransparency);
        }
    }
#endif
}