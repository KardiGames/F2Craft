using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActiveEntitiesManager : MonoBehaviour
{
    [SerializeField] private List<ActiveEntity> _entities = new List<ActiveEntity>();

    public void AddEntity(ActiveEntity entity)
    {
        if (entity == null || _entities.Contains(entity))
        {
            print("ERROR! Entity not registred!");
            return;
        }
        _entities.Add(entity);
    }

    public void RemoveEntity(ActiveEntity entity)
    {
        if (!_entities.Contains(entity))
        {
            print ("Error. Can't remove entity.");
            return;
        }
        _entities.Remove(entity);
    }

    public IEnumerable<ActiveEntity> GetEnemiesList (int playerNumber)
    {
        return _entities.Where(entity => entity.PlayerNumber != playerNumber);
    }

}
