using TMPro;
using UnityEngine;

public class TextUserInterface : MonoBehaviour
{
    public static TextUserInterface Instance;
    [SerializeField] private TextMeshProUGUI _text;    
    

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        if (_text == null)
        {
            Debug.LogError("Link is not set");
        }
    }

    public void Show (string text)
    {
        _text.text = text;
    }
    public void Add(string text)
    {
        _text.text += text;
    }
}
