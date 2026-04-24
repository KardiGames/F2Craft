using UnityEngine;
using System.Collections.Generic;
using System.Data.SqlTypes;

public class HotkeyUserInterface : MonoBehaviour
{
    private enum HotkeyState { Default, SelectBuilding, SelectItemBlueprint, SelectUnit, SelectFoundation }

    [Header("General links")]
    [SerializeField] private TextUserInterface _textUI;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _foundationMask;
    [SerializeField] private int _playerNumber;

    [Header("Blueprints")]
    [SerializeField] private List<BlueprintForBuilding> _buildings = new List<BlueprintForBuilding>();
    [SerializeField] private List<BlueprintForItem> _itemBlueprints = new List<BlueprintForItem>();
    [SerializeField] private List<BlueprintForUnit> _unitBlueprints = new List<BlueprintForUnit>();

    private List<string> _keyTexts = new List<string>() { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
    private List<KeyCode> _keyCodes = new List<KeyCode>() { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P };

    private HotkeyState _hotkeyState = HotkeyState.Default;
    private BlueprintForBuilding _selectedBuilding;
    private BlueprintForUnit _selectedUnit;
    private BlueprintForItem _selectedItem;
    private Dictionary<int, string> _stateText = new Dictionary<int, string>();


    private void Start()
    {
        if (_textUI == null || _camera == null || _foundationMask == 0)
            Debug.LogError("Link is not set");
        if (_buildings.Count > _keyCodes.Count
            || _itemBlueprints.Count > _keyCodes.Count
            )
            Debug.LogError("Crutch error. Some list is too long");

        _stateText.Add((int)HotkeyState.Default, "B - Build\n");

        _stateText.Add((int)HotkeyState.SelectBuilding, "Select building\n");
        for (int i = 0; i < _buildings.Count; i++)
            _stateText[(int)HotkeyState.SelectBuilding] += _keyTexts[i] + " - " + _buildings[i].ConstructingBuilding.name + "\n";

        _stateText.Add((int)HotkeyState.SelectItemBlueprint, "Select item blueprint\n");
        for (int i = 0; i < _itemBlueprints.Count; i++)
            _stateText[(int)HotkeyState.SelectItemBlueprint] += _keyTexts[i] + " - " + _itemBlueprints[i].name + "\n";

        _stateText.Add((int)HotkeyState.SelectUnit, "Select produced unit\n");
        for (int i = 0; i < _unitBlueprints.Count; i++)
            _stateText[(int)HotkeyState.SelectUnit] += _keyTexts[i] + " - " + _unitBlueprints[i].name + "\n";

        ResetState();
    }

    private void Update()
    {
        switch (_hotkeyState)
        {
            case HotkeyState.SelectFoundation:
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, _foundationMask))
                    {
                        Foundation selectedFoundation = hit.collider.GetComponent<Foundation>();
                        if (selectedFoundation == null)
                        {
                            Debug.LogError("Missed foundation component under click");
                            return;
                        }

                        if (_selectedUnit != null)
                        {
                            selectedFoundation.BuildUnitProducer(_playerNumber, _selectedBuilding, _selectedUnit);
                            ResetState();
                        }
                        else if (_selectedItem != null)
                        {
                            selectedFoundation.BuildFactory(_playerNumber, _selectedBuilding, _selectedItem);
                            ResetState();
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                    ResetState();
                break;
            case HotkeyState.SelectBuilding:
                if (TryGetIndexOfPressedKey(out int buildingIndex))
                {
                    _selectedBuilding = _buildings[buildingIndex];
                    switch (_selectedBuilding.ConstructingBuilding)
                    {
                        case Factory factory:
                            _hotkeyState = HotkeyState.SelectItemBlueprint;
                            _textUI.SetBuildMenuText(_stateText[(int)HotkeyState.SelectItemBlueprint]);
                            break;
                        case UnitProducer pruducer:
                            _hotkeyState = HotkeyState.SelectUnit;
                            _textUI.SetBuildMenuText(_stateText[(int)HotkeyState.SelectUnit]);
                            break;
                        default:
                            Debug.LogError("Building type is not detected");
                            ResetState();
                            break;
                    }

                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                    ResetState();
                break;

            case HotkeyState.SelectItemBlueprint:
                if (TryGetIndexOfPressedKey(out int itemBlueprintIndex))
                {
                    _hotkeyState = HotkeyState.SelectFoundation;
                    _selectedItem = _itemBlueprints[itemBlueprintIndex];
                    _textUI.SetBuildMenuText("Select place for\n" + _selectedBuilding + "\n with blueprint for\n" + _selectedItem.name);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                    ResetState();
                break;

            case HotkeyState.SelectUnit:
                if (TryGetIndexOfPressedKey(out int unitIndex))
                {
                    _hotkeyState = HotkeyState.SelectFoundation;
                    _selectedUnit = _unitBlueprints[unitIndex];
                    _textUI.SetBuildMenuText("Select place for\n" + _selectedBuilding + "\n with blueprint for\n" + _selectedUnit.ProducedUnit.name);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                    ResetState();
                break;
            case HotkeyState.Default:
                if (Input.GetKeyDown(KeyCode.B))
                {
                    _hotkeyState = HotkeyState.SelectBuilding;
                    _textUI.SetBuildMenuText(_stateText[(int)HotkeyState.SelectBuilding]);
                }
                break;

        }
    }

    private void ResetState()
    {
        _selectedBuilding = null;
        _selectedItem = null;
        _selectedUnit = null;
        _hotkeyState = HotkeyState.Default;
        _textUI.SetBuildMenuText(_stateText[(int)HotkeyState.Default]);
    }

    private bool TryGetIndexOfPressedKey(out int index)
    {
        index = -1;
        for (int i = 0; i < _keyCodes.Count; i++)
            if (Input.GetKeyDown(_keyCodes[i]))
            {
                index = i;
                return true;
            }
        return false;
    }
}
