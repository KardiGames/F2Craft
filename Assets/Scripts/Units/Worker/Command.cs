using System;
using UnityEngine;

namespace WorkerLogic
{
    public interface ICommand
    {
        public event Action OnFinishedCommandExecuted;
        public void Execute();
        public void Cancel();
    }
}