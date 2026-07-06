using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using Unity.Cinemachine;
using UnityEngine;

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

    [BoxGroup("Paneles"), Required]
    [SerializeField] private GameObject panelCreditos;

    [BoxGroup("Referencias"), Required]
    public CinemachineCamera CameraIU;

    [BoxGroup("Referencias"), Required]
    public GameObject Canvas;

    [BoxGroup("Referencias"), Required]
    public GameObject CanvasOpciones;

    [BoxGroup("Referencias"), Required]
    [SerializeField] private CorrutinaCamera CorrutinaCamera;

    [BoxGroup("Sound")]
    [Required]
    [SerializeField] private MMF_Player BTNFeedBack;
    [BoxGroup("Sound")]
    [Required]
    [SerializeField] private AudioClip menuMusic;

    public event Action<GameObject> onHighlight;
    public event Action<GameObject> onSelect;

    [FoldoutGroup("Debug"), ReadOnly, ShowInInspector]
    private Node<GameObject> current;

    private CircularDoubleLinkedList<GameObject> menuItems = new();

    private void OnEnable()
    {
        MenuInputs.OnNavigate += HandleNavigate;
        MenuInputs.OnSubmit += HandleSubmit;
        MenuInputs.OnCancel += HandleCancel;
    }

    private void OnDisable()
    {
        MenuInputs.OnNavigate -= HandleNavigate;
        MenuInputs.OnSubmit -= HandleSubmit;
        MenuInputs.OnCancel -= HandleCancel;
    }

    private void Start()
    {
        menuItems.Add(botonPlay);
        menuItems.Add(botonOption);
        menuItems.Add(botonQuit);

        current = menuItems.head;
        onHighlight?.Invoke(current.Value);

        CloseAllPanels();
        OpenPanelGeneral();
        MusicPlayer.Instance.Play(menuMusic);
    }

    private void HandleNavigate(Vector2 input)
    {
        if (input.y < 0)
            current = current.Next;
        else if (input.y > 0)
            current = current.Prev;

        onHighlight?.Invoke(current.Value);
    }

    private void HandleSubmit()
    {
        onSelect?.Invoke(current.Value);

        if (current.Value == botonPlay)
            PlayGame();
        else if (current.Value == botonOption)
            OpenOptions();
        else if (current.Value == botonQuit)
            QuitGame();
    }

    public void PlayGame()
    {
        BTNFeedBack?.PlayFeedbacks();
        CameraIU.Priority = 20;
        Canvas.SetActive(false);
        CorrutinaCamera.InitiationCoroutine();
        MusicPlayer.Instance.Stop();
    }

    public void OpenOptions()
    {
        BTNFeedBack?.PlayFeedbacks();
        Canvas.SetActive(false);
        CanvasOpciones.SetActive(true);

        OpenPanelGeneral();
    }

    public void QuitGame()
    {
        BTNFeedBack?.PlayFeedbacks();
        Application.Quit();
    }

    // Paneles
    // Paneles - lógica pura, sin sonido (usado también desde Start())
    public void OpenPanelGeneral() => OpenPanel(panelGeneral);
    public void OpenPanelSonidos() => OpenPanel(panelSonidos);
    public void OpenPanelControles() => OpenPanel(panelControles);
    public void OpenPanelCreditos() => OpenPanel(panelCreditos);

    // Wrappers con sonido — conectá ESTOS a los botones/tabs en el inspector (OnClick)
    public void OnClickPanelGeneral()
    {
        OpenPanelGeneral();
        BTNFeedBack?.PlayFeedbacks();
    }

    public void OnClickPanelSonidos()
    {
        OpenPanelSonidos();
        BTNFeedBack?.PlayFeedbacks();
    }

    public void OnClickPanelControles()
    {
        OpenPanelControles();
        BTNFeedBack?.PlayFeedbacks();
    }

    public void OnClickPanelCreditos()
    {
        OpenPanelCreditos();
        BTNFeedBack?.PlayFeedbacks();
    }

    private void CloseAllPanels()
    {
        panelGeneral.SetActive(false);
        panelSonidos.SetActive(false);
        panelControles.SetActive(false);
        panelCreditos.SetActive(false);
    }

    private void OpenPanel(GameObject panel)
    {
        CloseAllPanels();
        panel.SetActive(true);
    }

    public void HandleCancel()
    {
        if (CanvasOpciones != null && CanvasOpciones.activeSelf)
        {
            CloseAllPanels();

            CanvasOpciones.SetActive(false);
            Canvas.SetActive(true);
        }
    }
}