using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EnemyBaseData", menuName = "Scriptable Objects/EnemyBaseData")]
public class EnemyBaseData : ScriptableObject
{
    //Info
    [BoxGroup("Info"), HorizontalGroup("Info/Split", 160)]
    [PreviewField(150, ObjectFieldAlignment.Left), HideLabel]
    [SerializeField] private Sprite icon;

    [VerticalGroup("Info/Split/Details"), LabelWidth(90)]
    [SerializeField] private string enemyName;

    [VerticalGroup("Info/Split/Details"), LabelWidth(90)]
    [SerializeField] private string enemyID;

    [VerticalGroup("Info/Split/Details"), LabelWidth(90)]
    [SerializeField] private EnemyType enemyType;

    [VerticalGroup("Info/Split/Details"), LabelWidth(90)]
    [SerializeField] private WeaknessType weakness;

    //stats
    [BoxGroup("Stats")]
    [ProgressBar(0, 100, ColorGetter = "HealthColor")]
    [SerializeField] private float health;

    [BoxGroup("Stats"), LabelWidth(90)]
    [SerializeField] private float speed;

    [BoxGroup("Stats"), LabelWidth(90)]
    [SerializeField] private float damage;

    [BoxGroup("Stats"), LabelWidth(90)]
    [SerializeField] private float detectionRange;

    //description
    [BoxGroup("Description"), HideLabel, MultiLineProperty(4)]
    [SerializeField] private string description;

    // Color del health bar, cambia según el valor de health
    private Color HealthColor => health > 60 ? Color.green : health > 30 ? Color.yellow : Color.red;

    #region Getters
    public Sprite Icon => icon;
    public string EnemyName => enemyName;
    public string EnemyID => enemyID;
    public EnemyType EnemyType => enemyType;
    public WeaknessType Weakness => weakness;
    public float Health => health;
    public float Speed => speed;
    public float Damage => damage;
    public float DetectionRange => detectionRange;
    public string Description => description;
    #endregion
}
