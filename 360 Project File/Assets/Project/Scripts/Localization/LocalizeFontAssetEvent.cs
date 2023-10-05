using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;

[System.Serializable]
public class LocalizedFontAsset : LocalizedAsset<TMP_FontAsset> {}

[System.Serializable]
public class FontAssetEvent : UnityEvent<TMP_FontAsset> {}

[AddComponentMenu("Localization/Asset/Localize Font Asset Event")]
public class LocalizeFontAssetEvent : LocalizedAssetEvent<TMP_FontAsset, LocalizedFontAsset, FontAssetEvent>
{
}
