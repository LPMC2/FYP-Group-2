using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static QuizSO;

[CreateAssetMenu(fileName = "ItemStorage", menuName ="ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] item;
    [System.Serializable]
    public class Item
    {
        public GameObject itemObject;
        [SerializeField]
        private LocalizableString[] itemName;
        public Sprite itemSprite;

        public string GetItemName(int lanIndex = default)
        {
            if (itemName.Length == 0) return null;

            int index = 0;
            string currentLanguage = "";
            if(lanIndex == default)
            {
                Debug.Log("default");
                currentLanguage = CultureInfo.CurrentCulture.Name.ToString();
                currentLanguage = currentLanguage.Replace('-', '_');
            } else
            {
                Language language = default;
                Debug.Log("1");
                switch (lanIndex)
                {
                    case 1:
                        language = Language.en;
                        break;
                    case 2:
                        language = Language.zh_HK;
                        break;

                    case 3:
                        language = Language.zh_CN;
                        break;
                }
                currentLanguage = language.ToString();
            }
            foreach(LocalizableString lan in itemName)
            {
                string languageText = lan.language.ToString();

                Debug.Log(languageText + "/" + currentLanguage);
                if (languageText == currentLanguage.ToString())
                {
                    break;
                }
                if(index < itemName.Length-1)
                index++;
            }
            if (itemName[index].text == null || itemName[index].text == "")
            {
                return itemObject.name;
            } else
            {
                return itemName[index].text;
            }
        }
    }
}
