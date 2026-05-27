using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [BoxGroup("Botones"), Required]
    [SerializeField] private GameObject botonPlay;

    [BoxGroup("Botones"), Required]
    [SerializeField] private GameObject botonOption;

    [BoxGroup("Botones"), Required]
    [SerializeField] private GameObject botonQuit;

    [BoxGroup("Eventos")]
    public Action<GameObject> onHighlight;

    [BoxGroup("Eventos")]
    public Action<GameObject> onSelect;

    [FoldoutGroup("Debug"), ReadOnly, ShowInInspector]
    private Node<GameObject> current;

    [FoldoutGroup("Debug"), ReadOnly, ShowInInspector]
    private string currentButtonName => current?.Value?.name ?? "Ninguno";

    private InputSystem_Actions inputs;
    private CircularDoubleLinkedList<GameObject> menuItems = new();

    public void Awake()
    {
        inputs = new InputSystem_Actions();
    }

    public void OnEnable()
    {
        inputs.Enable();
        inputs.UI.Navigate.performed += OnNavigate;
        inputs.UI.Submit.performed += OnSelect;
    }

    public void OnDisable()
    {
        inputs.UI.Navigate.performed -= OnNavigate;
        inputs.UI.Submit.performed -= OnSelect;
        inputs.Disable();
    }

    public void Start()
    {
        menuItems.Add(botonPlay);
        menuItems.Add(botonOption);
        menuItems.Add(botonQuit);

        current = menuItems.head;
        onHighlight?.Invoke(current.Value);
    }

    public void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        if (input.y < 0)
        {
            current = current.Next;
            onHighlight?.Invoke(current.Value);
            Debug.Log("Destacando: " + current.Value.name);
        }
        else if (input.y > 0)
        {
            current = current.Prev;
            onHighlight?.Invoke(current.Value);
            Debug.Log("Destacando: " + current.Value.name);
        }
    }

    public void OnSelect(InputAction.CallbackContext ctx)
    {
        onSelect?.Invoke(current.Value);

        if (current.Value == botonPlay) PlayGame();
        else if (current.Value == botonOption) OpenOptions();
        else if (current.Value == botonQuit) QuitGame();
    }
    [GUIColor(0.6f, 0.8f, 1f)]
    [Button("Opciones", ButtonSizes.Large), ButtonGroup("Escenas")]
    public void OpenOptions()
    {
        Debug.Log("Options");
    }
    [GUIColor(0.5f, 1f, 0.5f)]
    [Button("Play", ButtonSizes.Large), ButtonGroup("Escenas")]
    public void PlayGame()
    {
        SceneManager.LoadScene("Scene_Home");
    }
    [GUIColor(1f, 0.4f, 0.4f)]
    [Button("Salir", ButtonSizes.Large), ButtonGroup("Escenas")]
    public void QuitGame()
    {
        Application.Quit();
    }
}