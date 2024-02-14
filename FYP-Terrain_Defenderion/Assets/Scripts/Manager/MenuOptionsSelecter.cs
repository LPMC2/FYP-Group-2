using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuOptionsSelecter : MonoBehaviour
{
    [SerializeField] private GameObject m_MenuOptionPrefab;
    [SerializeField] private List<Option> m_Options = new List<Option>();
    [SerializeField] private string m_TargetTag = "Options";
    [SerializeField] private int currentOption = 0;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }


    private void Initialize()
    {
        int index = 0;
        foreach(Option child in m_Options)
        {
            GameObject target = Instantiate(m_MenuOptionPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            target.name = m_Options[index].Name;
            MenuOptionManager menuOptionManager = target.GetComponent<MenuOptionManager>();
            if (menuOptionManager == null)
            {
                menuOptionManager = target.AddComponent<MenuOptionManager>();
            }
            menuOptionManager.Initialize(this, index);
            TMP_Text text = GameObjectExtension.GetGameObjectWithTagFromChilds(target, "Text").GetComponent<TMP_Text>();
            text.text = m_Options[index].Name;
            index++;
        }
        SelectOption(currentOption);
    }
    public void SelectOption(int page)
    {
        int index = 0;
        currentOption = page;
        
        foreach (Transform child in transform)
        {
            Button button = child.GetComponent<Button>();
            if (index == page) {
                m_Options[index].ReferencedMenu.SetActive(true);
                button.interactable = false;
            } else
            {
                m_Options[index].ReferencedMenu.SetActive(false);
                button.interactable = true;
            }
            index++;
        }
    }
    [System.Serializable]
    public class Option
    {
        [SerializeField] private string name = "";
        [SerializeField] private GameObject referencedMenu;
        public string Name { get { return name; } }
        public GameObject ReferencedMenu { get { return referencedMenu; } }
    }
}
