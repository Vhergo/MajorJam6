using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxControl;
    [SerializeField] private ParallaxLayer[] layers;

    private Transform vCam;
    private Vector3 lastCamPos;

    void Start() {
        vCam = GameObject.Find("Virtual Camera").transform;
        lastCamPos = vCam.position;
    }

    void Update() {
        AdjustParallaxLayers();
        lastCamPos = vCam.position;
    }

    void AdjustParallaxLayers() {
        Vector3 parallaxDelta = vCam.position - lastCamPos;
        for (int i = 0; i < layers.Length; i++) {
            ParallaxLayer layerData = layers[i];
            float parallaxX = parallaxDelta.x * layerData.parallaxFactorX * parallaxControl;
            float parallaxY = parallaxDelta.y * layerData.parallaxFactorY * parallaxControl;
            
            Vector3 newPosition = layerData.layer.position + new Vector3(parallaxX, parallaxY, 0f);
            
            layerData.layer.position = newPosition;
        }
    }
}

[System.Serializable]
public class ParallaxLayer
{
    [Header("Prallax Layer")]
    public Transform layer;
    public float parallaxFactorX;
    public float parallaxFactorY;
}

