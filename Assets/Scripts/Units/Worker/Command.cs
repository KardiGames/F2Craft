using UnityEngine;

namespace WorkerLogic
{
    public interface ICommand
    {
        public void Execute(Worker worker);
        public void Reset();
    }
}