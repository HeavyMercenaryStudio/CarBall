using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollision : MonoBehaviour {

    float effectTime;
    [SerializeField] float maxEffectTime = 0.5f;

    Renderer renderer;
    void Start(){
       renderer = GetComponent<Renderer>();
       renderer.sharedMaterial.SetFloat("_EffectTime", 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            effectTime = maxEffectTime;
            renderer.sharedMaterial.SetVector("_Position", contact.point);
            renderer.sharedMaterial.SetFloat("_EffectTime", 1);
            StopAllCoroutines();
        }
    }

    IEnumerator OnCollisionExit(Collision collision)
    {
        while (effectTime > 0)
        {
            yield return new WaitForEndOfFrame();
            effectTime -= Time.deltaTime;
            var time = effectTime / maxEffectTime;
            renderer.sharedMaterial.SetFloat("_EffectTime", time);
        }
        
    }
}
