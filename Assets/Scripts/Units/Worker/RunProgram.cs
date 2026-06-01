using NUnit.Framework.Internal.Filters;
using System;
using UnityEngine;
namespace WorkerLogic
{

    public class RunProgram : ICommand
    {
        public event Action OnFinishedCommandExecuted;
        private enum State { Finished, Moving, Connecting, Interacting}
        private Worker _worker;
        private Program _program;
        private Vector3 _point;
        private State _state = State.Moving;

        public RunProgram(Worker worker, Program program)
        {
            if (worker == null || program == null || program.Quantity <= 0 || program.IsAborted)
            {
                Debug.Log("Error! Command RunProgram failed to create and finished!");
                _state = State.Finished;
            }
            else if (program.Building == null)
            {
                _state = State.Finished;
            }
            else
            {
                _worker = worker;
                _program = program;
                if (worker.ConnectedBuilding == program.Building)
                    _state = State.Interacting;
                else
                {
                    _point = _program.Building.transform.position;
                    _state = State.Moving;
                }
            }
        }

        public void Execute()
        {
            if (_program.Building == null)
                _state = State.Finished;
            switch (_state)
            {
                case State.Moving:
                    if (_worker.TryReachThePoint(_point))
                    {
                        _state = State.Connecting;
                    }
                    break;
                case State.Connecting:
                    if (_worker.TryConnectBuilding(_program.Building))
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
            if (_program.IsGettingItem)
            {
                if (_program.ForceItem && (_worker.ItemsSlot.Item == _program.Item || _worker.ItemsSlot.Item == null))
                {
                    if (_program.ForceQuantity && TryGetItemOfType(_program.Item, _program.Quantity))
                        _state = State.Finished;
                    else if (TryGetItemOfType(_worker.ItemsSlot.Item))
                        _state = State.Finished;
                }
                else if (_program.ForceItem == false)
                {
                    foreach (Item item in _program.Building.ItemsToGive())
                    {
                        if (_program.ForceQuantity && TryGetItemOfType(_program.Item, _program.Quantity))
                        {
                            _state = State.Finished;
                            return;
                        }
                        else if (TryGetItemOfType(item))
                        {
                            _state = State.Finished;
                            return;
                        }
                    }
                    if (!_program.ForceQuantity)
                        _state = State.Finished;
                }
                return;
            }

            if (_program.ForceItem && _worker.ItemsSlot.Item == _program.Item)
            {
                if (_program.ForceQuantity && TryGiveItemOfType(_program.Item, _program.Quantity))
                    _state = State.Finished;
                else if (TryGiveItemOfType(_program.Item))
                    _state = State.Finished;
            }
            else if (_program.ForceItem == false)
            {
                if (_worker.ItemsSlot.Item == null)
                {
                    _state = State.Finished;
                    return;
                }

                if (_program.ForceQuantity && TryGiveItemOfType(_worker.ItemsSlot.Item, _program.Quantity))
                    _state = State.Finished;
                else
                {
                    TryGiveItemOfType(_worker.ItemsSlot.Item);
                    _state = State.Finished;
                }

            }
        }

        private bool TryGetItemOfType(Item item)
        {
            int capasity = _worker.ItemsSlot.FreeCapacity(item);
            if (capasity <= 0)
                return false;

            ItemsSlot slot;
            int quantity = Mathf.Min(capasity, _program.Building.ItemsOfTypeToGive(item, out slot));
            if (quantity > 0
                && slot != null
                && slot.TryGive(_worker.ItemsSlot, quantity))
            {
                return true;
            }
            return false;
        }

        private bool TryGetItemOfType(Item item, int quantity)
        {
            if (quantity <= 0 || item == null)
                return false;
            int capasity = _worker.ItemsSlot.FreeCapacity(item);
            if (capasity < quantity)
                return false;

            ItemsSlot slot;
            if (_program.Building.ItemsOfTypeToGive(item, out slot) >= quantity
                && slot != null
                && slot.TryGive(_worker.ItemsSlot, quantity))
            {
                return true;
            }
            return false;
        }

        private bool TryGiveItemOfType(Item item)
        {
            ItemsSlot slot;
            int capasity = _program.Building.ItemsOfTypeToGet(_worker.ItemsSlot.Item, out slot);
            int quantity = Mathf.Min(capasity, _worker.ItemsSlot.Quantity);
            if (quantity > 0
                && slot != null
                && _worker.ItemsSlot.TryGive(slot, quantity)
                )
            {
                return true;
            }
            return false;
        }
        private bool TryGiveItemOfType(Item item, int quantity)
        {
            ItemsSlot slot;
            
            if (_program.Building.ItemsOfTypeToGet(_worker.ItemsSlot.Item, out slot) >= quantity
                && slot != null
                && _worker.ItemsSlot.TryGive(slot, quantity)
                )
            {
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