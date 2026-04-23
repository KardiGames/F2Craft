using UnityEngine;
using System.Collections.Generic;

public class HotkeyUserInterface : MonoBehaviour
{
    private enum HotkeyState { Default, SelectBuilding, SelectItem, SelectUnit, SelectFoundation }

    [Header("General links")]
    [SerializeField] private TextUserInterface _textUI;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _foundationMask;
    [SerializeField] private int _playerNumber;

    [Header("Blueprints")]
    [SerializeField] private List<BlueprintForBuilding> _buildings = new List<BlueprintForBuilding>();

    private HotkeyState _hotkeyState = HotkeyState.Default;
    private BlueprintForBuilding _selectedBuilding;
    private BlueprintForUnit _selectedUnit;
    private BlueprintForItem _selectedItem;
    private Dictionary<int, string> _stateText = new Dictionary<int, string>();


    private void Start()
    {
        if (_textUI == null || _camera == null || _foundationMask == 0)
            Debug.LogError("Link is not set");

        _stateText.Add((int)HotkeyState.Default, "B - Build\n");

        _stateText.Add((int)HotkeyState.SelectBuilding, "Select building\n");
        for (int i = 0; i < _buildings.Count; i++)
            _stateText[(int)HotkeyState.SelectBuilding] += i+" - "+_buildings[i].ConstructingBuilding.name + "\n";

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
                if (Input.GetKeyDown(KeyCode.B))
                    print("lala");
                else if (Input.GetKeyDown(KeyCode.Escape))
                    ResetState();
                break;
            case HotkeyState.Default:
                if (Input.GetKeyDown(KeyCode.B))
                {
                    _hotkeyState = HotkeyState.SelectBuilding;
                    _textUI.Show(_stateText[(int)HotkeyState.SelectBuilding]);
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
        _textUI.Show(_stateText[(int)HotkeyState.Default]);
    }
}
