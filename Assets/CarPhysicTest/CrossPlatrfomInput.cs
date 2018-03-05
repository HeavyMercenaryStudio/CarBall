using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossPlatrfomInput : MonoBehaviour {

    [SerializeField] PCInput pcInput= new PCInput();
    [SerializeField] PCInput androidInput = new PCInput();

    public string MovementAxis{
        get { return pcInput.HorizontalMovementInput; }
    }

	
}


[System.Serializable]
public class PCInput
{
    public string HorizontalMovementInput;
    public string VerticalMovementInput;
    public string BoostButton;
}
[System.Serializable]
public class AndroidInput
{
    public string HorizontalMovementInput;
    public string VerticalMovementInput;
    public Button BoostButton;
}