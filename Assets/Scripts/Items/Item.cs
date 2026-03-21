using UnityEngine;

[CreateAssetMenu(fileName ="Item", menuName ="Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string _id = string.Empty;
    public string Id => _id; 
}
