using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeaLlightControls : MonoBehaviour
{

    private Light headLight;
    // Start is called before the first frame update
    void Start()
    {
        headLight = gameObject.GetComponent<Light>();
        print(headLight);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            if (headLight.enabled)
            {
                headLight.enabled = false;
            }
            else
            {
                headLight.enabled = true;
            }
            
        }
    }
}
