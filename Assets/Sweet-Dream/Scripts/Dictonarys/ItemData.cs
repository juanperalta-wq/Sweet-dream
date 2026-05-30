using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/BaseItem")]
public class ItemData : ScriptableObject
{
    [BoxGroup("Info"), HorizontalGroup("Info/Split", 160), PreviewField(150, ObjectFieldAlignment.Left)]
    [HideLabel]
    [SerializeField] private Sprite icon;

    [VerticalGroup("Info/Split/Details"), LabelWidth(80)]
    [SerializeField] private int id;

    [VerticalGroup("Info/Split/Details"), LabelWidth(80)]
    [SerializeField] private string itemName;

    [VerticalGroup("Info/Split/Details"), LabelWidth(80)]
    [SerializeField] private ItemType type;

    [VerticalGroup("Info/Split/Details"), LabelWidth(80)]
    [SerializeField] private float sanity;

    [BoxGroup("Description"), HideLabel, MultiLineProperty(4)]
    [SerializeField] private string description;

    #region Getters
    public Sprite Icon => icon;
    public int ID => id;
    public string ItemName => itemName;
    public ItemType Type => type;
    public string Description => description;
    public float Sanity => sanity;
    #endregion
}