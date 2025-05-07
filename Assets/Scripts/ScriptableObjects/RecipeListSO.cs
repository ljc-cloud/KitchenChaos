using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu()]
public class RecipeListSO : ScriptableObject {

    public List<RecipeSO> recipeSOList;


    public RecipeSO GetRecipeSOFromId(int id)
    {
        RecipeSO recipeSO = null;
        foreach (var recipeSo in recipeSOList)
        {
            if (recipeSo.id == id)
            {
                recipeSO = recipeSo;
                break;
            }
        }
        return recipeSO;
    }
}