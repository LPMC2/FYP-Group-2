using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DisplayBehaviour : MonoBehaviour
{
    public static DisplayBehaviour Singleton;
    [SerializeField] private TMP_Text DisplayText;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private Color defaultColor = Color.white;
    private Coroutine fadeCoroutine;
    private void Awake()
    {
        Singleton = this;
    }
    public void StartFadeInText(string text, Color color = default(Color), float duration = default)
    {
        if (color == default(Color))
        {
            color = Color.white;
        }
        // Stop any ongoing fade coroutine
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Get the item name from the item data using the provided item ID
        // Start the fade coroutine
        fadeCoroutine = StartCoroutine(FadeText(text, color, duration));
    }
    private IEnumerator FadeText(string text, Color color, float duration = default)
    {
        if (duration == default)
        {
            duration = displayDuration;
        }
        // Set the initial text and alpha value
        DisplayText.text = text;
        DisplayText.alpha = 0f;
        DisplayText.color = color;
        // Fade in animation
        float fadeInTimer = 0f;
        while (fadeInTimer < fadeInDuration)
        {
            fadeInTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeInTimer / fadeInDuration);
            DisplayText.alpha = alpha;
            yield return null;
        }

        // Display the text for a duration
        yield return new WaitForSeconds(duration);

        // Fade out animation
        float fadeOutTimer = 0f;
        while (fadeOutTimer < fadeOutDuration)
        {
            fadeOutTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeOutTimer / fadeOutDuration);
            DisplayText.alpha = alpha;
            yield return null;
        }

        // Reset the text and alpha value
        DisplayText.text = "";
        DisplayText.alpha = 0f;

        // Reset the fade coroutine reference
        fadeCoroutine = null;
    }
}
