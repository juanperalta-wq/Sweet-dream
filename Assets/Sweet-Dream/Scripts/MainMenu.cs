using Sirenix.OdinInspector;
using System;
using UnityEngine;
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

    private CircularDoubleLinkedList<GameObject> menuItems = new();

    public void OnEnable()
    {
        MenuInputs.OnNavigate += HandleNavigate;
        MenuInputs.OnSubmit += HandleSubmit;
    }

    public void OnDisable()
    {
        MenuInputs.OnNavigate -= HandleNavigate;
        MenuInputs.OnSubmit -= HandleSubmit;
    }

    public void Start()
    {
        menuItems.Add(botonPlay);
        menuItems.Add(botonOption);
        menuItems.Add(botonQuit);

        current = menuItems.head;
        onHighlight?.Invoke(current.Value);
    }

    private void HandleNavigate(Vector2 input)
    {
        if (input.y < 0)
        {
            current = current.Next;
            Debug.Log(current.Value.name);
            onHighlight?.Invoke(current.Value);
        }
        else if (input.y > 0)
        {
            current = current.Prev;
            Debug.Log(current.Value.name);
            onHighlight?.Invoke(current.Value);
        }
    }

    private void HandleSubmit()
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