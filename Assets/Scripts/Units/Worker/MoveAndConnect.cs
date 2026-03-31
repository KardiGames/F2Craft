using System;
using UnityEngine;
namespace WorkerLogic
{

    public class MoveAndConnect : ICommand
    {
        public event Action OnFinishedCommandExecuted;
        private enum State { Finished, Moving, Connecting }
        private Worker _worker;
        private Building _building;
        private Vector3 _point;
        private State _state = State.Moving;


        public MoveAndConnect(Worker worker, Building building)
        {
            if (worker == null || building == null)
            {
                Debug.Log("Error! Command Move and connect hasn't created!");
                _state = State.Finished;
            }
            else
            {
                _worker = worker;
                if (worker.ConnectedBuilding == building)
                    _state = State.Finished;
                else
                {
                    _building = building;
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
                        _state = State.Finished;
                    else
                        _state = State.Moving;
                    break;
                case State.Finished:
                    OnFinishedCommandExecuted?.Invoke();
                    break;
            }

        }
        
        public void Cancel()
        {
            OnFinishedCommandExecuted = null;
            _state = State.Finished;
        }
    }

}