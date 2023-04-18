using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject UIPanelPrefab;
    public GameObject UIButtonPrefab;
    public Transform UIParent;

    private GameObject currentPanel;

    public event Action OnCorrectAnswerSelected;

    public void ShowUIPanel(GameObject prefab)
    {
        if (currentPanel != null)
        {
            Destroy(currentPanel);
        }

        currentPanel = Instantiate(UIPanelPrefab, UIParent);

        for (int number = 1; number <= 10; number++)
        {
            GameObject buttonObject = Instantiate(UIButtonPrefab, currentPanel.transform);
            Button button = buttonObject.GetComponent<Button>();
            Text buttonText = buttonObject.GetComponentInChildren<Text>();
            buttonText.text = number.ToString();

            int prefabNameAsInt = int.Parse(prefab.name);
            button.onClick.AddListener(() => OnButtonClick(prefab, number == prefabNameAsInt));
        }
    }

    private void OnButtonClick(GameObject prefab, bool isCorrect)
    {
        if (isCorrect)
        {
            Destroy(currentPanel);
            currentPanel = null;
            OnCorrectAnswerSelected?.Invoke(); // Invoke the event
        }
    }
}
