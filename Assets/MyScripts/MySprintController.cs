using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;                         // make sure this is present

public class SprintFOV : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] CinemachineCamera vcam;     // ← new type name

    [Header("FOV Settings")]
    [SerializeField] float normalFOV = 60f;
    [SerializeField] float sprintFOV = 72f;
    [SerializeField] float lerpSpeed = 5f;

    PlayerInput input;

    void Awake()
    {
        input = GetComponent<PlayerInput>();

        // Auto-grab the first active CinemachineCamera if the field is empty
        if (vcam == null)
            vcam = FindAnyObjectByType<CinemachineCamera>();
    }

    void Update()
    {
        bool sprintHeld = input.actions["Player/Sprint"].IsPressed();
        float target = sprintHeld ? sprintFOV : normalFOV;

        // Lens is a struct; copy-modify-assign pattern
        var lens = vcam.Lens;
        lens.FieldOfView = Mathf.Lerp(lens.FieldOfView,
                                      target,
                                      lerpSpeed * Time.deltaTime);
        vcam.Lens = lens;
    }
}
