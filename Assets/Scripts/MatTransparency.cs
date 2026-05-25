using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MatTransparency : MonoBehaviour
{

    public Renderer targetRenderer; // The renderer of the object whose material you want to modify
    public Material material;
    public Slider transparencySlider;

    private Material targetMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transparencySlider = GetComponent<Slider>();
        if (targetRenderer != null)
        {

            targetMaterial = targetRenderer.sharedMaterial; // Get the material from the renderer
        }
        else
        {
            targetMaterial = material;
        }
        
        transparencySlider.onValueChanged.AddListener(OnSliderChanged); // Add listener to the slider
        OnSliderChanged(transparencySlider.value); // Set initial transparency based on slider value
        Debug.Log("Material got: " + targetMaterial.name);
    }

    private void OnSliderChanged(float value)
    {
        Color color = targetMaterial.color;
        color.a = value;
        targetMaterial.color = color; // Update the material's color with the new alpha value
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
