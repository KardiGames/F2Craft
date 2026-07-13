using System.Collections.Generic;
using UnityEngine;

namespace WorkerLogic
{
    public class Programmer : MonoBehaviour
    {
        [SerializeField] int _logSize = 2;
        [SerializeField] private List<Program> _programLog = new();
        [SerializeField] private int _currentProgramIndex = -1;

        public int LogSize
        {
            get => _logSize; set
            {
                if (_logSize > value)
                {
                    _programLog.RemoveRange(0, _logSize - value);
                }
                _logSize = value;
            }
        }

        public IEnumerable<Program> GetLog()
        {
            for (int i=0; i<_programLog.Count; i++)
            {
                if (_programLog[i].Building==null)
                {
                    _programLog.RemoveAt(i--);
                }
            }
            return _programLog;
        }

        public void Log(Program program)
        {
            if (program == null)
                return;
            _programLog.Add(program);
            if (_programLog.Count > _logSize)
                _programLog.RemoveRange(0, _programLog.Count - _logSize);
        }

        public Program GetNextProgram()
        {
            if (_programLog.Count == 0)
                return null;

            _currentProgramIndex++;
            if (_currentProgramIndex >= _programLog.Count)
                _currentProgramIndex = 0;

            if (_programLog[_currentProgramIndex].IsAborted || _programLog[_currentProgramIndex].Building==null)
            {
                _programLog.RemoveAt(_currentProgramIndex);
                return GetNextProgram();
            }

            return _programLog[_currentProgramIndex];
        }
        public void RestartProgramSequence()
        {
            _currentProgramIndex = -1;
        }

        public void ResetLog()
        {
            _programLog.Clear();
            _currentProgramIndex = -1;
        }
    }
}