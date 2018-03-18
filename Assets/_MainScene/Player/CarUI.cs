using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI { 
    public class CarUI : MonoBehaviour {

        [HideInInspector]
        public Image energyBar;

        public void Start(){
            energyBar = FindObjectOfType<SpeedUICircleFillImage>().GetComponent<Image>();
        }

        public void SetEnergyBarValue(float energy)
        {
            energyBar.fillAmount = energy;
        }

    }
}