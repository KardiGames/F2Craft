using UnityEngine;

namespace WorkerLogic
{

    public class Commander : MonoBehaviour
    {
        [SerializeField] System.Collections.Generic.List<ICommand> _commandQueue=new();
        private Worker _worker;
        private Programmer _programmer;
        
        private void Awake()
        {
            _worker = GetComponent<Worker>();
            _programmer = GetComponent<Programmer>();
        }

        private void Start()
        {

            _worker.GetCommand(new Move());
        }

    }
}