using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomUI
{
    public class DemoUIManager : MonoBehaviour
    {
        [SerializeField] private UIPolygon[] uiPolygons;

        private void Update()
        {
            foreach (var ui in uiPolygons)
            {
                ui.SetRotation(10f * Time.deltaTime);
                ui.SetFilled(Mathf.PingPong(Time.time, 0.9f) + 0.1f);
            }
        }
    }
}
