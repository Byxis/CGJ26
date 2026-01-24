using UnityEngine;

[CreateAssetMenu(fileName = "NewTipData", menuName = "Game/Tip Data")]
public class TipData : ScriptableObject
{
    [HideInInspector]
    [SerializeField]
    private string id;
    public string ID => id;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }
    }

    [Tooltip("Le texte à afficher dans l'interface")]
    [TextArea(3, 10)]
    public string message;

    [Header("Visual Guide")]
    public bool showHighlight;
    [Tooltip("Position X/Y sur l'écran (par rapport au centre ou selon l'ancrage du cercle dans le Canvas)")]
    public Vector2 highlightPosition;

    [Tooltip("Si coché, l'astuce s'affichera à chaque fois, même si déjà vue.")]
    public bool isPersistent;
}
