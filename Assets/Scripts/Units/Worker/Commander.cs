using System.Collections.Generic;
using UnityEngine;

namespace WorkerLogic
{

    public class Commander : MonoBehaviour
    {
        [SerializeField] bool _isProgramActivated=false;
        private Worker _worker;
        private Programmer _programmer;
        [SerializeField] List<ICommand> _commandQueue=new();

        public void Move(Vector3 point) {
            AssignCommand(new Move(_worker, point));
        }        
        
        public void Connect(Building building)
        {
            AssignCommand(new MoveAndConnect(_worker, building));
        }

        public void Interact (Building building)
        {
            AssignCommand(new Interact(_worker, building));
        }

        public void SwitchProgrammedMode()
        {
            _isProgramActivated = !_isProgramActivated;
            if (_isProgramActivated)
                NextCommand();
        }
        private void Awake()
        {
            _worker = GetComponent<Worker>();
            _programmer = GetComponent<Programmer>();
        }

        private void Start()
        {

            //_worker.GetCommand(new Move(GameObject.Find("Factory").transform.position));
            //_worker.GetCommand(new MoveAndConnect(_worker, GameObject.Find("Factory").GetComponent<Factory>()));
        }
        private void AssignCommand (ICommand command)
        {
            if (command == null) 
                return;

            _worker.Command = command;
            command.OnFinishedCommandExecuted += NextCommand;

        }
        private void NextCommand ()
        {
            if (_isProgramActivated)
                AssignCommand(new RunProgram(_worker, _programmer.GetNextProgram()));
            else if (_commandQueue.Count>0)
            {
                AssignCommand(_commandQueue[0]);
                _commandQueue.RemoveAt(0);
            } else
            {
                _worker.Command = null;
            }
        }

    }
}