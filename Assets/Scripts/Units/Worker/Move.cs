using UnityEngine;
namespace WorkerLogic
{

    public class Move : ICommand
    {
        private Vector3 point;
        
        public void Execute(Worker worker)
        {
            if (worker.TryReachThePoint(point))
            {

            }
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }

}