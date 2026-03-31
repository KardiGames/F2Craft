using System;
using UnityEngine;
namespace WorkerLogic
{

    public class Move : ICommand
    {
        public event Action OnFinishedCommandExecuted;
        private enum State {Finished, Moving}
        private Worker _worker;
        private Vector3 _point;
        private State _state = State.Moving;

        public Move (Worker worker, Vector3 point)
        {
            _worker = worker;
            _point = point;
            _state = State.Moving;
            if (_worker == null)
            {
                Debug.Log("Error! Command Move hasn't created!");
                _state = State.Finished;
            }
        }

        public void Execute()
        {
            if (_state == State.Finished)
            {
                OnFinishedCommandExecuted?.Invoke();
                return;
            }

            if (_worker.TryReachThePoint(_point))
            {
                _state = State.Finished;
            }
        }

        public void Cancel()
        {
            _state = State.Finished;
            OnFinishedCommandExecuted = null;
        }
    }

}