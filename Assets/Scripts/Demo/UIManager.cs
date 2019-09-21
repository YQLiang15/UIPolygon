using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomUI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIPolygon ability;

        public void OnClickRoll()
        {
            float[] values = new float[ability.Sides];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Random.Range(0f, 1f);
            }

            ability.SetValue(values);
        }
    }
}
