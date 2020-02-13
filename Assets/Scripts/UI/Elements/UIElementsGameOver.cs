using System;
using TMPro;

using UnityEngine;

// UI elements for rebind menu
public class UIElementsGameOver : MonoBehaviour
{
    // Element references
    [SerializeField]
    public TextMeshProUGUI GameOverText;

    // Game over
    public void GameOver()
    {
        this.GameOverText.text = $@"GAME OVER{Environment.NewLine}{Environment.NewLine}Time: {GameController.FinalTime}{Environment.NewLine}Score: {GameController.FinalScore}";
    }
}
