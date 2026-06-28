using Unity.Cinemachine;
using UnityEngine;

public class MouseSensitivityManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineInputAxisController inputController;

    private const string SensitivityKey = "MouseSensitivity";

    private void Start()
    {
        ApplySensitivity();
    }

    public void ApplySensitivity()
    {
        float sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1.5f);

        foreach (var controller in inputController.Controllers)
        {
            if (controller.Name == "Look X (Pan)")
            {
                controller.Input.Gain = sensitivity;
            }

            if (controller.Name == "Look Y (Tilt)")
            {
                controller.Input.Gain = -sensitivity;
            }
        }
    }
}