using Unity.VisualScripting;
using UnityEngine;

public class ActiveEntity : MonoBehaviour
{
    
    [SerializeField] private int _playerNumber=0;
    [SerializeField] protected int _hp=0;
    [SerializeField] private ActiveEntitiesManager _activeEntitiesManager; //TODO delete SerField

    public int PlayerNumber => _playerNumber;
    public int HP => _hp;

    public void Init(int playerNumber, int hp, ActiveEntitiesManager activeEntitiesManager)
    {
        if (_playerNumber != 0 || hp != 0 || _activeEntitiesManager!=null)
        {
            print("Error. Re-initiation. Canceling");
            return;
        }

        if (hp <= 0 || activeEntitiesManager == null)
        {
            print ("Error! Active Entity initialisation aborted");
            return;
        }
        
        _playerNumber = playerNumber;
        _hp = hp;
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
            _activeEntitiesManager.RemoveEntity(this);
            Destroy(gameObject);
        }
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
