#nullable enable
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class EntitiesObserver : MonoBehaviour
{
    [SerializeField] private List<ActiveEntity> _entities=new List<ActiveEntity>();

    private void Refresh (object? sender, NotifyCollectionChangedEventArgs e)
    {
        _entities.Clear();
        _entities.AddRange(ActiveEntity.GetEntitiesList());
    }

    private void Start()
    {
        ActiveEntity.AddObserver(Refresh);
    }
}
