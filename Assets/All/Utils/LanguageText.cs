using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LanguageText : MonoBehaviour
{
    public Text text { get { return GetComponent<Text>(); } }

    [Header("Languages")]
    [SerializeField]
    private Language editorLanguage;

    [SerializeField]
    [TextArea(1, 10)]
    private string m_english;
    [SerializeField]
    [TextArea(1, 10)]
    private string m_french;

    public void Awake()
    {
        ShowCurrentLanguage();
        Languages.AddLanguageText(this);
    }

    public void Set(string en, string fr)
    {
        m_english = en;
        m_french = fr;

        ShowCurrentLanguage();
    }
    public void SetAll(string text)
    {
        m_english = text;
        m_french = text;

        ShowCurrentLanguage();
    }
    public void SetText(Language language, string text)
    {
        switch (language)
        {
            case Language.English:
                m_english = text;
                break;
            case Language.French:
                m_french = text;
                break;
            default:
                m_english = text;
                break;
        }
    }
    public void SetEditorLanguage(Language language)
    {
        if (language != editorLanguage)
        {
            editorLanguage = language;

            ShowEditorLanguage();
        }
    }

    public string GetText(Language language)
    {
        switch (language)
        {
            case Language.English:
                return m_english;
            case Language.French:
                return m_french;
            default:
                return m_english;
        }
    }
    public string GetCurrentText()
    {
        return GetText(Languages.GetCurrentOne());
    }
    public string GetEditorText()
    {
        return GetText(editorLanguage);
    }

    public void ShowLanguage(Language language)
    {
        text.text = GetText(language);
    }
    public void ShowCurrentLanguage()
    {
        text.text = GetCurrentText();
    }
    public void ShowEditorLanguage()
    {
        text.text = GetEditorText();
    }

    public void OnValidate()
    {
        UpdateText();
    }
    public void UpdateText()
    {
        ShowEditorLanguage();
    }
}
