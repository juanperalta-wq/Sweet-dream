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

    [BoxGroup("Paneles"), Required]
    [SerializeField] private GameObject panelGeneral;
    [BoxGroup("Paneles"), Required]
    [SerializeField] private GameObject panelSonidos;
    [BoxGroup("Paneles"), Required]
    [SerializeField] private GameObject panelControles;

    [BoxGroup("Referencias"), Required]
    public CinemachineCamera CameraIU;
    [BoxGroup("Referencias"), Required]
    public GameObject Canvas;
    [BoxGroup("Referencias"), Required]
    public GameObject CanvasOpciones;
    [BoxGroup("Referencias"), Required]
    [SerializeField] private CorrutinaCamera CorrutinaCamera;

    public event Action<GameObject> onHighlight;
    public event Action<GameObject> onSelect;

    [FoldoutGroup("Debug"), ReadOnly, ShowInInspector]
    private Node<GameObject> current;

    private MyStack<GameObject> Windows = new();
    private CircularDoubleLinkedList<GameObject> menuItems = new();

    public void OnEnable()
    {
        MenuInputs.OnNavigate += HandleNavigate;
        MenuInputs.OnSubmit += HandleSubmit;
        MenuInputs.OnCancel += HandleCancel;
    }

    public void OnDisable()
    {
        MenuInputs.OnNavigate -= HandleNavigate;
        MenuInputs.OnSubmit -= HandleSubmit;
        MenuInputs.OnCancel -= HandleCancel;
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
        if (Windows.Count > 0) return;

        if (input.y < 0) current = current.Next;
        else if (input.y > 0) current = current.Prev;

        onHighlight?.Invoke(current.Value);
    }

    private void HandleSubmit()
    {
        if (Windows.Count > 0) return;

        onSelect?.Invoke(current.Value);

        if (current.Value == botonPlay) PlayGame();
        else if (current.Value == botonOption) OpenOptions();
        else if (current.Value == botonQuit) QuitGame();
    }

    [GUIColor(0.5f, 1f, 0.5f)]
    [Button("Play", ButtonSizes.Large), ButtonGroup("Acciones")]
    public void PlayGame()
    {
        CameraIU.Priority = 20;
        Canvas.SetActive(false);
        CorrutinaCamera.InitiationCorrutine();
    }

    [GUIColor(0.6f, 0.8f, 1f)]
    [Button("Opciones", ButtonSizes.Large), ButtonGroup("Acciones")]
    public void OpenOptions()
    {
        Canvas.SetActive(false);
        CanvasOpciones.SetActive(true);
    }

    [GUIColor(1f, 0.4f, 0.4f)]
    [Button("Salir", ButtonSizes.Large), ButtonGroup("Acciones")]
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenPanelGeneral() => OpenPanel(panelGeneral);
    public void OpenPanelSonidos() => OpenPanel(panelSonidos);
    public void OpenPanelControles() => OpenPanel(panelControles);
    private void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
        Windows.Push(panel);
    }

    [GUIColor(1f, 0.8f, 0.4f)]
    [Button("Escape", ButtonSizes.Large), ButtonGroup("Acciones")]
    public void HandleCancel()
    {
        if (CanvasOpciones != null && CanvasOpciones.activeSelf)
        {
            CanvasOpciones.SetActive(false);
            Canvas.SetActive(true);
        }
    }
}