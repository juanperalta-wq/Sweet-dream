using Sirenix.OdinInspector;
using System;
using System.Collections;
using TMPro.Examples;
using Unity.Cinemachine;
using Unity.VisualScripting;
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

    [BoxGroup("Referencias")]
    public CinemachineCamera CameraIU;

    [BoxGroup("Referencias")]
    public GameObject Canvas;

    [BoxGroup("Referencias")]
    public GameObject CanvasOpciones;

    [BoxGroup("Eventos")]
    public Action<GameObject> onSelect;

    [FoldoutGroup("Debug"), ReadOnly, ShowInInspector]
    private Node<GameObject> current;

    private MyStack<GameObject> Windows;
    private CircularDoubleLinkedList<GameObject> menuItems = new();
    [SerializeField] private CorrutinaCamera CorrutinaCamera;

    public void OnEnable()
    {
        MenuInputs.OnNavigate += HandleNavigate;
        MenuInputs.OnSubmit += HandleSubmit;
        MenuInputs.OnCancel += Escape;
    }

    public void OnDisable()
    {
        MenuInputs.OnNavigate -= HandleNavigate;
        MenuInputs.OnSubmit -= HandleSubmit;
        MenuInputs.OnCancel -= Escape;
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

    [GUIColor(0.5f, 1f, 0.5f)]
    [Button("Play", ButtonSizes.Large), ButtonGroup("Escenas")]
    public void PlayGame()
    {
        CameraIU.Priority = 20;
        gameObject.SetActive(false);

        Debug.Log("Iniciando corrutina");
        CorrutinaCamera.InitiationCorrutine();
        Debug.Log("Corrutina iniciada");
    }

    [GUIColor(0.6f, 0.8f, 1f)]
    [Button("Opciones", ButtonSizes.Large), ButtonGroup("Escenas")]
    public void OpenOptions()
    {
        if (Canvas == true)
        {
            CanvasOpciones.SetActive(true);

        }
    }

    [GUIColor(1f, 0.4f, 0.4f)]
    [Button("Salir", ButtonSizes.Large), ButtonGroup("Escenas")]
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Escape()
    {
        
    }
}