using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUI : MonoBehaviour {

    [SerializeField] Image energyBar;


    public void SetEnergyBarValue(float energy)
    {
        energyBar.fillAmount = energy;
    }

}
