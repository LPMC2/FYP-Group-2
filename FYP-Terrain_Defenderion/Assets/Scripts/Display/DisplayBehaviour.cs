using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DisplayBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Text DisplayText;
    public TMP_Text TMP_DisplayText { get { return DisplayText; } }
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private Color defaultColor = Color.white;
    private Coroutine fadeCoroutine;
    private void Awake()
    {
        DisplayText = gameObject.GetComponent<TMP_Text>();
    }
    public virtual void SetText(string text, Color color = default(Color), float resetDuration = -1)
    {
        if(color == default(Color))
        {
            color = Color.white;
        }
        DisplayText.text = text;
        if(resetDuration > 0)
            Invoke("ResetText", resetDuration);
            
    }
    private void ResetText()
    {
        DisplayText.color = Color.white;
        DisplayText.text = "";
    }
    public virtual void StartFadeInText(string text, Color color = default(Color),float fadeInDuration = default, float duration = default, float fadeOutDuration = default)
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
        fadeCoroutine = StartCoroutine(FadeText(text, color, fadeInDuration, duration, fadeOutDuration));
    }
    private IEnumerator FadeText(string text, Color color, float fadeInDuration = default, float duration = default, float fadeOutDuration = default)
    {
        //Set Defualt Values
        if (duration == default)
        {
            duration = displayDuration;
        }
        if(fadeInDuration == default)
        {
            fadeInDuration = this.fadeInDuration;
        }
        if (fadeOutDuration == default)
        {
            fadeOutDuration = this.fadeOutDuration;
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
            if (fadeInDuration > 0f)
            {
                DisplayText.alpha = alpha;
            } else
            {
                Debug.Log("Test");
                DisplayText.alpha = 1f;
            }
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
            if (fadeOutDuration > 0f)
            {
                DisplayText.alpha = alpha;
            }
            else
            {
                DisplayText.alpha = 1f;
            }
            yield return null;
        }

        // Reset
        DisplayText.text = "";
        DisplayText.alpha = 0f;

        fadeCoroutine = null;
    }
}
