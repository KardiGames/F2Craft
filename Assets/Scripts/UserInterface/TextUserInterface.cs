using TMPro;
using UnityEngine;
using WorkerLogic;

public class TextUserInterface : MonoBehaviour
{
    public static TextUserInterface Instance;
    [SerializeField] private SelectionManager _selectionManager;
    [SerializeField] private TextMeshProUGUI _text;
    private string _buildMenuText = "";
    private string _selectionText = "";
    private string _currentText = "";
    private ActiveEntity _storedCurrentSelected;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        if (_text == null || _selectionManager == null)
        {
            Debug.LogError("Link is not set");
        }
    }

    private void OnEnable()
    {
        _selectionManager.OnSelectionChanged += UpdateSelectionInfo;
        _selectionManager.OnCurrentChanged += AskNewCurrent;
    }
    private void OnDisable()
    {
        _selectionManager.OnSelectionChanged -= UpdateSelectionInfo;
        _selectionManager.OnCurrentChanged -= AskNewCurrent;
    }

    public void SetBuildMenuText(string text)
    {
        _buildMenuText = text;
        RefreshUIText();
    }
    private void RefreshUIText()
    {
        _text.text = _buildMenuText
            + "\n===================\n"
            + _selectionText
            + "===================\n"
            + _currentText;

    }

    private void AskNewCurrent()
    {

        ActiveEntity newCurrent = _selectionManager.CurrentInSelection;
        if (_storedCurrentSelected == newCurrent)
            return;

        if (_storedCurrentSelected != null)
            _storedCurrentSelected.OnParameterChaged -= UpdateCurrentInfo;
        _storedCurrentSelected = newCurrent;
        if (_storedCurrentSelected != null)
            _storedCurrentSelected.OnParameterChaged += UpdateCurrentInfo;
        UpdateCurrentInfo();
    }

    private void UpdateCurrentInfo()
    {
        _currentText = "";
        if (_storedCurrentSelected != null)
        {
            _currentText += "HP " + _storedCurrentSelected.HP + "/" + _storedCurrentSelected.MaxHp + " " + PlaceText(_storedCurrentSelected.transform) + "\n";
            if (_storedCurrentSelected is Worker worker)
            {
                _currentText += worker.ItemsSlot.Quantity + "/" + worker.ItemsSlot.QuantityLimit + " " + worker.ItemsSlot.Item?.name + "\n";
                Programmer programmer = worker.GetComponent<Programmer>();
                if (programmer == null)
                {
                    Debug.LogError("Worker without programmer", worker);
                    return;
                }
                _currentText += "Program log:\n";
                foreach (Program program in programmer.GetLog())
                {
                    _currentText += PlaceText(program.Building.transform) + " ";
                    if (program.IsGettingItem)
                        _currentText += "=> ";
                    else
                        _currentText += "<= ";
                    if (program.ForceQuantity)
                        _currentText += "!";
                    _currentText += program.Quantity+" ";
                    if (program.ForceItem)
                        _currentText += "!";
                    _currentText += program.Item?.name + "\n";

                }
            }
            RefreshUIText();
        }
    }
    private void UpdateSelectionInfo()
    {
        _selectionText = "";
        ActiveEntity currentInSelected = _selectionManager.CurrentInSelection;
        foreach (ActiveEntity selectedEntity in _selectionManager.SelectedEntities)
        {
            if (selectedEntity == currentInSelected)
                _selectionText += "->";
            _selectionText += selectedEntity.name + " [" + selectedEntity.HP * 100 / selectedEntity.MaxHp + "%]\n";
        }
        RefreshUIText();
    }

    private string PlaceText (Transform transform)
    {
        return "[" + Mathf.RoundToInt(transform.position.x) + " " + Mathf.RoundToInt(transform.position.z) + "]";
    }
}
