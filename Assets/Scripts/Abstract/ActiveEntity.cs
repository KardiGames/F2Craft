using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class ActiveEntity : MonoBehaviour
{
    private static ObservableCollection<ActiveEntity> _entities = new ObservableCollection<ActiveEntity>();

    /*
    private static void AddEntity(ActiveEntity entity)
    {
        if (entity == null || _entities.Contains(entity))
        {
            Debug.LogError("Entity not registred!");
            return;
        }
        _entities.Add(entity);
    }

    private static void RemoveEntity(ActiveEntity entity)
    {
        if (!_entities.Contains(entity))
        {
            Debug.LogError("Error. Can't remove entity.");
            return;
        }
        _entities.Remove(entity);
    }
    */
    public static void AddObserver (NotifyCollectionChangedEventHandler handler)
    {
        _entities.CollectionChanged += handler;
    }
    public static void RemoveObserver (NotifyCollectionChangedEventHandler handler)
    {
        _entities.CollectionChanged -= handler;
    }

    public static IEnumerable<ActiveEntity> GetEnemiesList(int playerNumber)
    {
        return _entities.Where(entity => entity.PlayerNumber != playerNumber);
    }
    public static IEnumerable<ActiveEntity> GetEntitiesList() => _entities;

    public static bool Contains(ActiveEntity entity) =>
        _entities.Contains(entity);

    [SerializeField] protected int _playerNumber;
    [SerializeField] protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected GameObject _selectionIndicator;

    public int PlayerNumber => _playerNumber;
    public int HP => _hp;
    public int MaxHp => _maxHp;

    protected void Init(int playerNumber)
    {
        if (_maxHp<=0 || _hp<=0 || _hp>_maxHp || _selectionIndicator == null)
        {
            Debug.LogError("Active Entity initialisation aborted. Destroying. GO: " + gameObject.name);
            Destroy();
            return;
        }
        
        _playerNumber = playerNumber;
    }

    public void GetDamage (int damage)
    {
        if (damage <= 0)
            return;
        _hp-= damage;
        
        if (_hp<0)
        {
            Destroy();
        }
    }

    public void SetSelection (bool isSelecting)
    {
        _selectionIndicator.SetActive(isSelecting);
    }
    
    protected virtual void Destroy ()
    {
        _entities.Remove(this);
        Destroy(gameObject);
    }

    protected virtual void Start()
    {
        if (_selectionIndicator == null)
            Debug.LogError("Selection indicator isn't set on " + gameObject.name);

        _entities.Add(this);
    }
}
