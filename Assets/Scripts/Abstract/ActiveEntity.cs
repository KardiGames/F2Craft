using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class ActiveEntity : MonoBehaviour
{
    [Flags] public enum Flags
    {
        None = 0,
        Building = 1 << 0,
        Mechanical = 1 << 1,
        Biological = 1 << 2,
        Flying = 1 << 3,
        Hover = 1 << 4,
        Massive = 1 << 5,
        Superheavy = 1 << 6,
        Cloaked = 1 << 7,
        Detector = 1 << 8,
        Regeneration = 1 << 9,
        Carrier = 1 << 10,
        Burrow = 1 << 11
    }

    public static event Action<ActiveEntity> S_OnEntityRemoved;
    public static event Action<ActiveEntity, ActiveEntity> S_OnEntityReplaced;
    private static ObservableCollection<ActiveEntity> s_entities = new ObservableCollection<ActiveEntity>();
    private static List<ActiveEntity> s_replaced = new List<ActiveEntity>();
    public static IEnumerable<ActiveEntity> GetEntitiesList() => s_entities;

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
    public static void AddObserver(NotifyCollectionChangedEventHandler handler)
    {
        s_entities.CollectionChanged += handler;
    }
    public static void RemoveObserver(NotifyCollectionChangedEventHandler handler)
    {
        s_entities.CollectionChanged -= handler;
    }

    public static IEnumerable<ActiveEntity> GetEnemiesList(int playerNumber)
    {
        return s_entities.Where(entity => entity.PlayerNumber != playerNumber);
    }

    public static IEnumerable<ActiveEntity> GetEntitiesList(int playerNumber) =>
        s_entities.Where(entity => entity.PlayerNumber == playerNumber);

    public static bool Contains(ActiveEntity entity) =>
        s_entities.Contains(entity);

    protected static void Replace(ActiveEntity oldEntity, ActiveEntity newEntity)
    {
        if (newEntity == null || s_entities.Contains(oldEntity) == false)
        {
            Debug.LogError("Replacing by NULL of not existing entity. Canceled.");
            return;
        }
        if (oldEntity == null)
            Debug.LogWarning("Replacing NULL entity. Strange. Continuing");

        s_entities[s_entities.IndexOf(oldEntity)] = newEntity;
        s_replaced.Add(newEntity);
        S_OnEntityReplaced?.Invoke(oldEntity, newEntity);

        Destroy(oldEntity.gameObject);
    }

    public event Action OnParameterChanged;

    [SerializeField] protected int _playerNumber;
    [SerializeField] protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected Flags _tags;
    [SerializeField] protected GameObject _selectionIndicator;

    public int PlayerNumber => _playerNumber;
    public int HP => _hp;
    public int MaxHp => _maxHp;
    public Flags Tags => _tags;

    protected void Init(int playerNumber)
    {
        if (_maxHp <= 0 || _hp <= 0 || _hp > _maxHp || _selectionIndicator == null)
        {
            Debug.LogError("Active Entity initialisation aborted. Destroying. GO: " + gameObject.name);
            Destroy(); //TODO Think! M.b. here must be Destroy (go) ?
            return;
        }

        _playerNumber = playerNumber;
    }

    protected void OnParameterChangedInvoke()
    {
        OnParameterChanged?.Invoke();
    }

    public virtual void TakeDamage(Combat.DamageData damage)
    {
        if (damage.Amount <= 0)
            return;
        damage = ApplyDamageModifiers(damage);
        _hp -= damage.Amount;
        OnParameterChanged?.Invoke();

        if (_hp <= 0)
        {
            Destroy();
        } 
    }

    public void SetSelection(bool isSelecting)
    {
        _selectionIndicator.SetActive(isSelecting);
    }

    public float SqrDistanceTo(Transform targetTransform)
    {
        if (targetTransform == null)
        {
            Debug.LogError("No target for SqrDistanceTo");
            return float.MaxValue;
        }
        float sqrDistance = (transform.position - targetTransform.position).sqrMagnitude; //TODO m.b. change to 2D distance &&|| cash transform
        sqrDistance -= transform.localScale.x / 2;
        sqrDistance -= targetTransform.localScale.x / 2;
        return sqrDistance;
    }

    protected virtual void Destroy()
    {
        s_entities.Remove(this);
        S_OnEntityRemoved?.Invoke(this);
        Destroy(gameObject);
    }

    protected virtual void Start()
    {
        if (_selectionIndicator == null)
            Debug.LogError("Selection indicator isn't set on " + gameObject.name);

        if (s_entities.Contains(this) == false)
            s_entities.Add(this);
        else if (s_replaced.Contains(this))
            s_replaced.Remove(this);
        else
            Debug.LogWarning("New ActiveEntity already in global list");
    }

    private Combat.DamageData ApplyDamageModifiers(Combat.DamageData damage)
    {
        return damage;
    }
}
