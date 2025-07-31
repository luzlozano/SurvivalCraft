using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;                       // ← keep this

[RequireComponent(typeof(CharacterController))]
public class HeadBobScaler : MonoBehaviour
{
    [SerializeField] CinemachineCamera vcam;   // drag your CinemachineCamera
    [SerializeField] float walkAmplitude = 0.07f;
    [SerializeField] float lerpSpeed = 8f;

    CharacterController cc;
    CinemachineBasicMultiChannelPerlin noise;  // ← correct type

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (vcam == null)
            vcam = FindAnyObjectByType<CinemachineCamera>();

        // fetch the noise extension on that camera
        noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
            Debug.LogWarning("No Noise extension found on the CinemachineCamera!");
    }

    void Update()
    {
        if (noise == null) return;             // safety check

        bool isMoving = cc.velocity.sqrMagnitude > 0.05f;
        float target = isMoving ? walkAmplitude : 0f;

        noise.AmplitudeGain = Mathf.Lerp(noise.AmplitudeGain,
                                         target,
                                         lerpSpeed * Time.deltaTime);
    }
}
