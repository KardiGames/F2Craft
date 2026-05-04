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
            _storedCurrentSelected.OnParameterChanged -= UpdateCurrentInfo;
        _storedCurrentSelected = newCurrent;
        if (_storedCurrentSelected != null)
            _storedCurrentSelected.OnParameterChanged += UpdateCurrentInfo;
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
                    if (program.Building == null)
                    {
                        Debug.LogWarning("Building link in program uncontrollable lost");
                        continue;
                    }
                    _currentText += PlaceText(program.Building.transform) + " ";
                    if (program.IsGettingItem)
                        _currentText += "=> ";
                    else
                        _currentText += "<= ";
                    if (program.ForceQuantity)
                        _currentText += "!";
                    _currentText += program.Quantity + " ";
                    if (program.ForceItem)
                        _currentText += "!";
                    _currentText += program.Item?.name + "\n";

                }
            }
            else if (_storedCurrentSelected is ConstructionSite site)
            {
                _currentText += site.ConstructedBuildingName + " under construction\n";
                _currentText += "Nesassary resources:\n";
                foreach (ItemsSlot slot in site.ResourcesStorage)
                {
                    _currentText += slot.Quantity + "/" + slot.QuantityLimit + " ";
                    if (slot.Item != null)
                        _currentText += slot.Item.name;
                    else if (slot.MonoItem != null)
                        _currentText += slot.MonoItem.name;
                    _currentText += "\n";
                }
                if (site.StageTimer > 0)
                    _currentText += "Stage timer: " + site.StageTimer;
            }
            else if (_storedCurrentSelected is UnitProducer producer)
            {
                if (producer.Blueprint != null)
                {

                    _currentText += producer.Blueprint.ProducedUnit.name + " producer\n";
                    _currentText += producer.QueuedUnits + " units queued";
                    if (producer.Autoproduction)
                        _currentText += " (auto)";
                    _currentText += "\n";
                    _currentText += "Resources:\n";
                    foreach (ItemsSlot slot in producer.ResourcesStorage)
                    {
                        _currentText += slot.Quantity + "/" + slot.QuantityLimit + " ";
                        if (slot.Item != null)
                            _currentText += slot.Item.name;
                        else if (slot.MonoItem != null)
                            _currentText += slot.MonoItem.name;
                        _currentText += "\n";
                    }
                    if (producer.UnitTimer > 0)
                        _currentText += "Unit ready in: " + producer.UnitTimer;
                }
                else
                    _currentText += "Unit to produce isn't selected";
            }
            else if (_storedCurrentSelected is Factory factory)
            {
                if (factory.Blueprint != null)
                {
                    _currentText += "Factory\nResources:\n";
                    foreach (ItemsSlot slot in factory.ResourcesStorage)
                    {
                        _currentText += slot.Quantity + "/" + slot.QuantityLimit + " ";
                        if (slot.Item != null)
                            _currentText += slot.Item.name;
                        else if (slot.MonoItem != null)
                            _currentText += slot.MonoItem.name;
                        _currentText += "\n";
                    }
                    _currentText += "Production:\n";
                    foreach (ItemsSlot slot in factory.ProductionStorage)
                    {
                        _currentText += slot.Quantity + "/" + slot.QuantityLimit + " ";
                        if (slot.Item != null)
                            _currentText += slot.Item.name;
                        else if (slot.MonoItem != null)
                            _currentText += slot.MonoItem.name;
                        _currentText += "\n";
                    }

                    if (factory.ItemTimer > 0)
                        _currentText += "Items ready in: " + factory.ItemTimer;
                }
                else
                    _currentText += "Item to produce isn't selected";
            }
            RefreshUIText();
        }
    }
    private void UpdateSelectionInfo()
    {
        _selectionText = "";
        AskNewCurrent();
        ActiveEntity currentInSelected = _selectionManager.CurrentInSelection;
        foreach (ActiveEntity selectedEntity in _selectionManager.SelectedEntities)
        {
            if (selectedEntity == currentInSelected)
                _selectionText += "->";
            _selectionText += selectedEntity.name + " [" + selectedEntity.HP * 100 / selectedEntity.MaxHp + "%]\n";
        }
        RefreshUIText();
    }

    private string PlaceText(Transform transform)
    {
        return "[" + Mathf.RoundToInt(transform.position.x) + " " + Mathf.RoundToInt(transform.position.z) + "]";
    }
}
