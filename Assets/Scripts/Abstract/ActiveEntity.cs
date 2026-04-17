using Unity.VisualScripting;
using UnityEngine;

public class ActiveEntity : MonoBehaviour
{
    [SerializeField] protected int _playerNumber;
    [SerializeField] protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected ActiveEntitiesManager _activeEntitiesManager;
    [SerializeField] protected GameObject _selectionIndicator;

    public int PlayerNumber => _playerNumber;
    public int HP => _hp;
    public int MaxHp => _maxHp;

    protected virtual void Init(int playerNumber, ActiveEntitiesManager activeEntitiesManager)
    {
        if (_activeEntitiesManager!=null)
        {
            print("Error. Re-initiation. Canceling. GO: "+gameObject.name+". Trying to continue");
            if (_activeEntitiesManager.Contains(this))
                return;
            print("Error! Fail. Destroyng");
            Destroy(gameObject);
        }

        if (activeEntitiesManager == null || _maxHp<=0 || _hp<=0 || _hp>_maxHp)
        {
            print ("Error! Active Entity initialisation aborted. GO: " + gameObject.name);
            Destroy(gameObject);
            return;
        }
        
        _playerNumber = playerNumber;
        _activeEntitiesManager = activeEntitiesManager;
        _activeEntitiesManager.AddEntity(this);
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

    }
    
    protected void Destroy ()
    {
        _activeEntitiesManager?.RemoveEntity(this);
        Destroy(gameObject);
    }

    private void Start()
    {
        if (_selectionIndicator == null)
            Debug.LogError("Selection indicator isn't set on " + gameObject.name);
    }
}
