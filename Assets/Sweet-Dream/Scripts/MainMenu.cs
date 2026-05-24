using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using System;

public class MainMenu : MonoBehaviour
{
    [Title("Botones del Menú")]
    [Required] public GameObject botonPlay;
    [Required] public GameObject botonQuit;
    [Required] public GameObject botonOption;

    [Title("Input")]
    public InputAction navigateAction;
    public InputAction selectAction;

    [Title("Eventos")]
    public Action<GameObject> onHighlight;
    public Action<GameObject> onSelect;

    [FoldoutGroup("Debug")]
    [ReadOnly, ShowInInspector] private Node<GameObject> current;

    private CircularDoubleLinkedList<GameObject> menuItems = new();

    void Start()
    {
        menuItems.Add(botonPlay);
        menuItems.Add(botonOption);
        menuItems.Add(botonQuit);

        current = menuItems.head;
        onHighlight?.Invoke(current.Value);
    }

    void OnEnable()
    {
        navigateAction.Enable();
        selectAction.Enable();

        navigateAction.performed += OnNavigate;
        selectAction.performed += OnSelect;
    }

    void OnDisable()
    {
        navigateAction.performed -= OnNavigate;
        selectAction.performed -= OnSelect;

        navigateAction.Disable();
        selectAction.Disable();
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector3 input = ctx.ReadValue<Vector3>();

        if (input.y < 0)
        {
            current = current.Next;
            onHighlight?.Invoke(current.Value);
            Debug.Log("Destacando:" + current.Value.name);
        }
        else if (input.y > 0)
        {
            current = current.Prev;
            onHighlight?.Invoke(current.Value);
            Debug.Log("Destacando:" +current.Value.name);
        }
    }

    private void OnSelect(InputAction.CallbackContext ctx)
    {
        onSelect?.Invoke(current.Value);

        if (current.Value == botonPlay)
            PlayGame();
        else if (current.Value == botonOption)
            OpenOptions();
        else if (current.Value == botonQuit)
            QuitGame();
    }
    public void OpenOptions()
    {
        Debug.Log("Options");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Scene_Home");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}