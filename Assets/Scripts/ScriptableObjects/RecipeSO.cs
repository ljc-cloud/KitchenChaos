using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{

    public int id;
    public List<KitchenObjectSO> kitchenObjectSOList;
    public string recipeName;
    public int tip;
}