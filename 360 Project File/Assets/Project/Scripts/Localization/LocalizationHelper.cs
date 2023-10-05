using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationHelper : MonoBehaviour
{
    public void ChangeLocale(int id)
    {
        Debug.Assert(0 <= id && id < LocalizationSettings.AvailableLocales.Locales.Count, $"Invalid locale id {id}.");
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
    }
}
