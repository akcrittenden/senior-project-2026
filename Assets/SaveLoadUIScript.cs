using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveLoadUIScript : MonoBehaviour
{
    UIDocument uiDocument;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        Button save1Button = root.Q<Button>("m_save1");
        Button save2Button = root.Q<Button>("m_save2");
        Button load1Button = root.Q<Button>("m_load1");
        Button load2Button = root.Q<Button>("m_load2");

        if (save1Button != null && save2Button != null && load1Button != null && load2Button != null)
        {
            save1Button.clicked += OnSave1ButtonButtonClicked;
            save2Button.clicked += OnSave2ButtonButtonClicked;
            load1Button.clicked += OnLoad1ButtonButtonClicked;
            load2Button.clicked += OnLoad2ButtonButtonClicked;
        } else
        {
            Debug.LogError("One or more buttons not found in the UI Document.");
        }
}
    private void OnSave1ButtonButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void OnSave2ButtonButtonClicked()
    {
        throw new NotImplementedException();
    }
    private void OnLoad1ButtonButtonClicked()
    {
        throw new NotImplementedException();
    }
    private void OnLoad2ButtonButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        var root = uiDocument.rootVisualElement;
        Button save1Button = root.Q<Button>("m_save1");
        Button save2Button = root.Q<Button>("m_save2");
        Button load1Button = root.Q<Button>("m_load1");
        Button load2Button = root.Q<Button>("m_load2");
        if (save1Button != null && save2Button != null && load1Button != null && load2Button != null)
        {
            save1Button.clicked -= OnSave1ButtonButtonClicked;
            save2Button.clicked -= OnSave2ButtonButtonClicked;
            load1Button.clicked -= OnLoad1ButtonButtonClicked;
            load2Button.clicked -= OnLoad2ButtonButtonClicked;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
