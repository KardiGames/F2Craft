using UnityEngine;
namespace WorkerLogic
{

    public class Move : ICommand
    {
        private Vector3 point;
        
        public void Execute(Worker worker)
        {
            worker.MoveTo(point);
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }

}