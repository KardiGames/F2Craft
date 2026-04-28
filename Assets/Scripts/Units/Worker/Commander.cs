using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Search;
using UnityEngine;

namespace WorkerLogic
{
    public class Commander : MonoBehaviour
    {
        [SerializeField] bool _isProgramActivated=false;
        private Worker _worker;
        private Programmer _programmer;
        [SerializeField] private List<ICommand> _commandQueue=new(); //TODO remove SerField

        public void Move(Vector3 point, bool enqueue = false) {
            if (enqueue && _worker.Command == null)
                _commandQueue.Add(new Move(_worker, point));
            else
                AssignCommand(new Move(_worker, point));
        }
        
        public void Connect(Building building, bool queue = false)
        {
            if (queue && _worker.Command == null)
                _commandQueue.Add(new MoveAndConnect(_worker, building));
            else
                AssignCommand(new MoveAndConnect(_worker, building));
        }

        public void Interact (Building building, bool enqueue = false)
        {
            if (enqueue && _worker.Command == null)
                _commandQueue.Add(new Interact(_worker, building));
            else
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