using System.Collections.Generic;
using UnityEngine;

public class WorkerRemover : MonoBehaviour
{
    [SerializeField] private List<Worker> _list = new List<Worker>();
    private Dictionary<Worker, int> _dict = new Dictionary<Worker, int>();
    int c = 0;

    void Update()
    {
        if (c == 0)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                _dict[_list[i]] = i;
            }

            foreach (var w in _list)
                w.GetDamage(100);
            c++;
        }
        else if (c == 1)
        {
            foreach (var w in _list)
            {
                Debug.Log(w + _dict[w].ToString());
            }
            Worker kw = _list[0];
            Debug.Log(kw + _dict[kw].ToString());
            enabled = false;
        }
    }
}
