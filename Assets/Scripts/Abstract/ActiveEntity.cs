using Unity.VisualScripting;
using UnityEngine;

public class ActiveEntity : MonoBehaviour
{
    [SerializeField] protected int _playerNumber=0;
    [SerializeField] protected int _hp=0;
    [SerializeField] protected int _maxHp=0;
    [SerializeField] protected ActiveEntitiesManager _activeEntitiesManager; //TODO delete SerField

    public int PlayerNumber => _playerNumber;
    public int HP => _hp;
    public int MaxHp => _maxHp;

    public void Init(int playerNumber, int hp, int maxHp, ActiveEntitiesManager activeEntitiesManager)
    {
        if (_playerNumber != 0 || _hp != 0 || _maxHp!=0 || _activeEntitiesManager!=null)
        {
            print("Error. Re-initiation. Canceling. GO: "+gameObject.name);
            Destroy(gameObject);
            return;
        }

        if (hp <= 0 || activeEntitiesManager == null)
        {
            print ("Error! Active Entity initialisation aborted");
            Destroy(gameObject);
            return;
        }
        
        _playerNumber = playerNumber;
        _hp = hp;
        _maxHp = maxHp;
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
    
    protected void Destroy ()
    {
        _activeEntitiesManager?.RemoveEntity(this);
        Destroy(gameObject);
    }
    
    private void Start()
    {
        if (_activeEntitiesManager == null)
        {
            print("ERROR! Active Entity hasn't initiated. Destroying");
            Destroy(gameObject);
        }
    }

    
}
