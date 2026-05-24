using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class ExamplesOdinInspector : MonoBehaviour
{
    // ─────────────────────────────────────────
    // SECCIÓN 1: LABELS Y VISUAL
    // ─────────────────────────────────────────

    [Title("Visual y Labels", "Cómo mostrar campos")]
    [LabelText("Velocidad del jugador")]
    public float speed = 5f;

    [HideLabel]                          // Quita la etiqueta, útil para strings descriptivos
    public string description = "Escribe una descripción...";

    [Multiline(4)]                       // Campo de texto multilínea (Unity nativo)
    public string notes = "";

    [MultiLineProperty(4)]               // Igual pero funciona con [ShowInInspector]
    public string odinNotes = "";

    [DisplayAsString]                    // Muestra como texto no editable (útil para debug)
    public string readOnlyInfo = "Solo lectura visual";

    [PropertySpace(20)]                  // Espacio de 20px arriba del campo
    [Title("Con espacio arriba")]
    public int spacedField = 0;


    // ─────────────────────────────────────────
    // SECCIÓN 2: INFO Y MENSAJES
    // ─────────────────────────────────────────

    [Title("Info Boxes")]
    [InfoBox("Este es un mensaje informativo azul")]
    public int health = 100;

    [InfoBox("¡Cuidado con valores negativos!", InfoMessageType.Warning)]
    public float damage = 10f;

    [InfoBox("Campo obligatorio vacío", InfoMessageType.Error, VisibleIf = "ShowError")]
    public string requiredName = "";
    private bool ShowError => string.IsNullOrEmpty(requiredName);

    [DetailedInfoBox("Haz clic para ver más info", "Este campo controla la velocidad máxima. Valores recomendados entre 1 y 20 para un control fluido del personaje.")]
    public float maxSpeed = 15f;


    // ─────────────────────────────────────────
    // SECCIÓN 3: GRUPOS (BoxGroup, FoldoutGroup, Tabs)
    // ─────────────────────────────────────────

    [Title("Grupos")]

    [BoxGroup("Stats del Personaje")]
    public int strength = 10;
    [BoxGroup("Stats del Personaje")]
    public int agility = 8;
    [BoxGroup("Stats del Personaje")]
    public int intelligence = 12;

    [FoldoutGroup("Configuración Avanzada")]
    public bool enableAI = true;
    [FoldoutGroup("Configuración Avanzada")]
    public float detectionRadius = 5f;
    [FoldoutGroup("Configuración Avanzada")]
    public LayerMask detectionMask;

    [TabGroup("Audio")]
    public AudioClip jumpSound;
    [TabGroup("Audio")]
    public AudioClip attackSound;

    [TabGroup("Visual")]
    public GameObject vfxPrefab;
    [TabGroup("Visual")]
    public Color playerColor = Color.white;


    // ─────────────────────────────────────────
    // SECCIÓN 4: GRUPOS HORIZONTALES Y VERTICALES
    // ─────────────────────────────────────────

    [Title("Layout Horizontal")]
    [HorizontalGroup("Posición", Width = 60)]
    [HideLabel] public float posX = 0;
    [HorizontalGroup("Posición")]
    [HideLabel] public float posY = 0;
    [HorizontalGroup("Posición")]
    [HideLabel] public float posZ = 0;

    [HorizontalGroup("Split", 0.5f)]
    [BoxGroup("Split/Izquierda")]
    public string leftField = "Campo izquierdo";

    [BoxGroup("Split/Derecha")]
    public string rightField = "Campo derecho";


    // ─────────────────────────────────────────
    // SECCIÓN 5: MOSTRAR / OCULTAR CONDICIONAL
    // ─────────────────────────────────────────

    [Title("Mostrar/Ocultar Condicional")]
    public bool isEnemy = false;

    [ShowIf("isEnemy")]              // Solo aparece si isEnemy == true
    public float aggroRange = 10f;

    [HideIf("isEnemy")]              // Aparece si isEnemy == false
    public float friendlyRadius = 5f;

    [ShowIf("@health < 50")]         // Usando expresión directa con @
    [InfoBox("¡Vida baja!", InfoMessageType.Warning)]
    public bool showLowHealthWarning;

    public bool hasWeapon = false;

    [ShowIfGroup("hasWeapon")]
    [BoxGroup("hasWeapon/Arma")]
    public float weaponDamage = 25f;
    [BoxGroup("hasWeapon/Arma")]
    public float weaponRange = 3f;


    // ─────────────────────────────────────────
    // SECCIÓN 6: HABILITAR / DESHABILITAR
    // ─────────────────────────────────────────

    [Title("Habilitar/Deshabilitar")]
    public bool canJump = true;

    [EnableIf("canJump")]            // Solo editable si canJump == true
    public float jumpForce = 8f;

    [DisableIf("canJump")]           // Desactivado si canJump == true
    public float fallSpeed = 9.8f;

    [DisableInPlayMode]              // Solo editable fuera del Play Mode
    public int initialLives = 3;

    [DisableInEditorMode]            // Solo editable durante Play Mode
    public float currentScore = 0f;

    [ReadOnly]                       // Nunca editable en inspector
    public string guid = "ABC-123-XYZ";


    // ─────────────────────────────────────────
    // SECCIÓN 7: VALIDACIÓN Y RANGOS
    // ─────────────────────────────────────────

    [Title("Validación")]
    [Required]                       // Muestra error si es null
    public Transform spawnPoint;

    [Required("El prefab del jugador es obligatorio")]
    public GameObject playerPrefab;

    [MinValue(0)]                    // No permite valores menores a 0
    public float mana = 50f;

    [MaxValue(100)]                  // No permite valores mayores a 100
    public float stamina = 80f;

    [Range(1, 10)]                   // Slider entre 1 y 10 (Unity nativo)
    public int level = 1;

    [PropertyRange(0.1f, 5f)]        // Slider que funciona también en properties
    public float timeScale = 1f;

    [MinMaxSlider(0, 100, ShowFields = true)]  // Slider de rango mín/máx
    public Vector2 spawnAmountRange = new Vector2(2, 8);

    [ValidateInput("IsPositive", "Debe ser mayor a cero")]
    public float positiveOnly = 1f;
    private bool IsPositive(float val) => val > 0;


    // ─────────────────────────────────────────
    // SECCIÓN 8: BOTONES
    // ─────────────────────────────────────────

    [Title("Botones")]
    [Button("Curar al jugador")]
    private void HealPlayer()
    {
        health = 100;
        Debug.Log("¡Jugador curado!");
    }

    [Button("Matar al jugador", ButtonSizes.Large)]
    private void KillPlayer()
    {
        health = 0;
        Debug.Log("Jugador eliminado.");
    }

    [ButtonGroup("Acciones Rápidas")]
    [Button("Reset Stats")]
    private void ResetStats() { speed = 5f; health = 100; }

    [ButtonGroup("Acciones Rápidas")]
    [Button("Randomizar")]
    private void RandomizeStats()
    {
        speed = Random.Range(1f, 20f);
        health = Random.Range(1, 100);
    }


    // ─────────────────────────────────────────
    // SECCIÓN 9: ENUMS MEJORADOS
    // ─────────────────────────────────────────

    [Title("Enums")]
    [EnumToggleButtons]              // Muestra enum como botones toggle
    public DifficultyLevel difficulty = DifficultyLevel.Normal;

    [EnumPaging]                     // Navega el enum con botones anterior/siguiente
    public GameState gameState = GameState.MainMenu;


    // ─────────────────────────────────────────
    // SECCIÓN 10: LISTAS Y COLECCIONES
    // ─────────────────────────────────────────

    [Title("Listas Mejoradas")]
    [ListDrawerSettings(ShowPaging = true, NumberOfItemsPerPage = 4, AddCopiesLastElement = true)]
    public List<string> itemNames = new List<string>();

    [ListDrawerSettings(IsReadOnly = true)]
    public List<int> readOnlyList = new List<int> { 1, 2, 3 };

    [TableList]
    public List<EnemyData> enemyTable = new List<EnemyData>();

    [DictionaryDrawerSettings(KeyLabel = "Tecla", ValueLabel = "Acción")]
    public SerializableDictionary<string, string> keybindings = new SerializableDictionary<string, string>();


    // ─────────────────────────────────────────
    // SECCIÓN 11: ASSETS Y REFERENCIAS
    // ─────────────────────────────────────────

    [Title("Assets y Referencias")]
    [PreviewField(Height = 100, Alignment = ObjectFieldAlignment.Left)]
    public Sprite icon;

    [PreviewField(50)]
    public Texture2D texture;

    [AssetsOnly]                     // Solo permite arrastrar assets (no objetos de escena)
    public GameObject particlePrefab;

    [SceneObjectsOnly]               // Solo permite objetos de la escena activa
    public Transform sceneTarget;

    [ChildGameObjectsOnly]           // Solo hijos de este GameObject
    public Transform childBone;

    [InlineEditor]                   // Edita el ScriptableObject directamente aquí
    public ExampleSO embeddedConfig;

    [FilePath(Extensions = "json,txt")]  // Selector de archivo con filtro de extensión
    public string configFilePath;

    [FolderPath]                     // Selector de carpeta
    public string saveFolderPath;


    // ─────────────────────────────────────────
    // SECCIÓN 12: COLOR Y GUI
    // ─────────────────────────────────────────

    [Title("Colores y Estilo")]
    [GUIColor(0.5f, 1f, 0.5f)]       // Color verde para este campo
    public float greenField = 1f;

    [GUIColor(1f, 0.4f, 0.4f)]       // Color rojo
    public float redField = 1f;

    [GUIColor(1f, 0.85f, 0.3f)]      // Color dorado
    [Button("¡Botón dorado!")]
    private void GoldenButton() => Debug.Log("Dorado!");

    [ColorPalette("Default")]        // Selector con paleta de colores predefinida
    public Color paletteColor;

    [ColorPalette]                   // Selector de paleta sin nombre fijo
    public Color customColor = Color.cyan;


    // ─────────────────────────────────────────
    // SECCIÓN 13: MOSTRAR PROPIEDADES Y CALLBACKS
    // ─────────────────────────────────────────

    [Title("Propiedades y Callbacks")]
    [ShowInInspector]                // Muestra aunque no sea serializado
    public string RuntimeInfo => $"HP: {health} | Speed: {speed}";

    [ShowInInspector, ReadOnly]
    public int ItemCount => itemNames.Count;

    [OnValueChanged("OnSpeedChanged")]   // Llama al método cuando cambia el valor
    public float trackedSpeed = 5f;
    private void OnSpeedChanged() => Debug.Log($"Velocidad cambiada a: {trackedSpeed}");

    [OnValueChanged("OnColorChanged")]
    public Color trackedColor = Color.white;
    private void OnColorChanged() => Debug.Log($"Color cambiado a: {trackedColor}");

    [OnCollectionChanged("OnItemsChanged")]
    public List<string> trackedList = new List<string>();
    private void OnItemsChanged() => Debug.Log($"Lista modificada. Items: {trackedList.Count}");


    // ─────────────────────────────────────────
    // SECCIÓN 14: TOGGLE GROUP (habilitar/deshabilitar grupo)
    // ─────────────────────────────────────────

    [Title("Toggle Group")]
    [ToggleGroup("enableDash", "Habilitar Dash")]
    public bool enableDash = false;
    [ToggleGroup("enableDash")]
    public float dashSpeed = 20f;
    [ToggleGroup("enableDash")]
    public float dashCooldown = 1.5f;
    [ToggleGroup("enableDash")]
    public int dashCharges = 2;

    [ToggleGroup("enableDoubleJump", "Habilitar Doble Salto")]
    public bool enableDoubleJump = false;
    [ToggleGroup("enableDoubleJump")]
    public float doubleJumpForce = 6f;
    [ToggleGroup("enableDoubleJump")]
    public int maxJumps = 2;


    // ─────────────────────────────────────────
    // SECCIÓN 15: MENÚ CONTEXTUAL CUSTOM
    // ─────────────────────────────────────────

    [Title("Menú Contextual")]
    [CustomContextMenu("Poner en máximo", "SetMaxHealth")]
    [CustomContextMenu("Poner en mínimo", "SetMinHealth")]
    public int contextHealth = 50;
    private void SetMaxHealth() { contextHealth = 100; }
    private void SetMinHealth() { contextHealth = 1; }

    [PropertyOrder(-1)]              // Aparece al principio del inspector sin importar el orden
    [Title("⬆ Este campo siempre aparece primero", bold: false)]
    public string alwaysFirst = "Ordenado con PropertyOrder(-1)";
}

// ─────────────────────────────────────────
// CLASES DE APOYO
// ─────────────────────────────────────────

[System.Serializable]
public class EnemyData
{
    [TableColumnWidth(120)]
    public string enemyName;
    [TableColumnWidth(60)]
    public int hp;
    [TableColumnWidth(60)]
    public float speed;
    [TableColumnWidth(80)]
    public bool isBoss;
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>,
    ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();
    public void OnBeforeSerialize() { keys.Clear(); values.Clear(); foreach (var kv in this) { keys.Add(kv.Key); values.Add(kv.Value); } }
    public void OnAfterDeserialize() { Clear(); for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++) TryAdd(keys[i], values[i]); }
}

public enum DifficultyLevel { Easy, Normal, Hard, Extreme }
public enum GameState { MainMenu, Playing, Paused, GameOver }

// Crea este ScriptableObject en otro archivo:
// [CreateAssetMenu] public class ExampleSO : ScriptableObject { public string configName; public int value; }