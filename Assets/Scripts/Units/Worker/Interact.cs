using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace WorkerLogic
{

    public class Interact : ICommand
    {
        public event Action OnFinishedCommandExecuted;
        private enum State { Finished, Moving, Connecting, Interacting }
        private Worker _worker;
        private Building _building;
        private Vector3 _point;
        private State _state = State.Moving;

        public Interact(Worker worker, Building building)
        {
            if (worker == null || building == null)
            {
                Debug.Log("Error! Command Interact hasn't created!");
                _state = State.Finished;
            }
            else
            {
                _worker = worker;
                _building = building;
                if (worker.ConnectedBuilding == building)
                    _state = State.Interacting;
                else
                {
                    _point = building.transform.position;
                    _state = State.Moving;
                }
            }
        }

        public void Execute()
        {
            switch (_state)
            {
                case State.Moving:
                    if (_worker.TryReachThePoint(_point))
                    {
                        _state = State.Connecting;
                    }
                    break;
                case State.Connecting:
                    if (_worker.TryConnectBuilding(_building))
                        _state = State.Interacting;
                    else
                        _state = State.Moving;
                    break;
                case State.Interacting:
                    SelectInteraction();
                    break;
                case State.Finished:
                    OnFinishedCommandExecuted?.Invoke();
                    break;
            }

        }

        private void SelectInteraction()
        {
            if (_worker.ItemsSlot.Item != null)
            {
                if (TryGiveItemOfType(_worker.ItemsSlot.Item))
                    return;
                if (TryGetItemOfType(_worker.ItemsSlot.Item))
                    return;
            }
            
            foreach (Item item in _building.ItemsToGive())
            {
                if (TryGetItemOfType(item))
                    return;
            }
            _state = State.Finished; //THINK m.b. better without this
        }

        private bool TryGetItemOfType (Item item)
        {
            int capasity = _worker.ItemsSlot.FreeCapacity(item);
            if (capasity <= 0)
                return false;

            ItemsSlot slot;
            int quantity = Mathf.Min(capasity, _building.ItemsOfTypeToGive(item, out slot));
            if (quantity > 0
                && slot != null
                && slot.TryGive(_worker.ItemsSlot, quantity))
            {
                _worker.GetComponent<Programmer>().Log(new Program(_building, _worker.ItemsSlot.Item, quantity, false));
                _state = State.Finished;
                return true;
            }
            return false;
        }
        private bool TryGiveItemOfType(Item item)
        {
            ItemsSlot slot;
            int capasity = _building.ItemsOfTypeToGet(_worker.ItemsSlot.Item, out slot);
            int quantity = Mathf.Min(capasity, _worker.ItemsSlot.Quantity);
            if (quantity > 0
                && slot != null
                && _worker.ItemsSlot.TryGive(slot, quantity)
                )
            {
                _worker.GetComponent<Programmer>().Log(new Program(_building, slot.Item, quantity, true));
                _state = State.Finished;
                return true;
            }
            return false;
        }

        public void Cancel()
        {
            _state = State.Finished;
            OnFinishedCommandExecuted = null;
        }
    }

}