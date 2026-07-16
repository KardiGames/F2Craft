using UnityEngine;
using Battle;
using System;

public class BattleUnit : Unit
{
    [SerializeField] private Weapon _weapon;
    public new void Init(int playerNumber)
    {
        base.Init(playerNumber);
        if (_weapon != null )
            _weapon.PlayerNumber = playerNumber;
    }

    protected override void Start()
    {
        base.Start();
        if (_weapon == null)
        {
            Debug.LogWarning("LINK to weapon was lost!");
            _weapon = GetComponent<Weapon>();
        }
    }
    private void Update()
    {
        Move();
    }

    private void Move ()
    {
        if (_weapon.Target == null || _weapon.TargetInRange) 
            return;
        Vector3 targetPoint = _weapon.Target.transform.position;
        targetPoint.y = transform.position.y;
        transform.LookAt(targetPoint);
        Vector3 moveVector = ((targetPoint - transform.position).normalized)*_moveSpeed*Time.deltaTime;
        
        transform.position = transform.position+ moveVector;
    }




    private void SwitchTarget ()
    {
        throw new NotImplementedException();
    }
}
