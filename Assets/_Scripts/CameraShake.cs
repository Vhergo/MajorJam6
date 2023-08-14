using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakePower;
    [SerializeField] private float shakeRate;
    [SerializeField] private float shakeTimer;
    
    private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin noise;

    void Start() {
        vCam = GetComponent<CinemachineVirtualCamera>();
        noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            TriggerCameraShake(shakePower, shakeRate, shakeTimer);
        }
    }

    public void TriggerCameraShake(float shakeMagnitude, float shakeFrequency, float shakeDuration) {
        ShakeCamera(shakeMagnitude, shakeFrequency);
        Invoke("StopShake", shakeDuration);
    }

    void ShakeCamera(float shakeMagnitude, float shakeFrequency){
        noise.m_AmplitudeGain = shakeMagnitude;
        noise.m_FrequencyGain = shakeFrequency;
    }

    void StopShake() {
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 1f;
    }
}
