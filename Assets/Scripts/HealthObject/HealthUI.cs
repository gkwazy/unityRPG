using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.HealthObject
{
    public class HealthUI : MonoBehaviour
    {

        [SerializeField] Health healthObject = null;
        [SerializeField] RectTransform uiText = null;
        [SerializeField] Canvas mainCanvas = null;

        void Update()
        {
            if (Mathf.Approximately(healthObject.convertToFraction(), 0) 
            || Mathf.Approximately(healthObject.convertToFraction(), 1))
            {
                mainCanvas.enabled = false;
                return;
            }
            mainCanvas.enabled = true;
            uiText.localScale = new Vector3(healthObject.convertToFraction(),1,1);
        }
    }
}
