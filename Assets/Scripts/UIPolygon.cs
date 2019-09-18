using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
    public class UIPolygon : MaskableGraphic
    {
        public int Sides { get { return sides; } }

        [SerializeField, Range(3, 360)] private int sides = 3;
        [SerializeField, Range(0, 360)] private float rotation;
        [SerializeField, Range(0f, 1f)] private float[] verticesDistance = new float[] { };
        [SerializeField, Range(0f, 1f)] private float filled;

        public void SetValue(float[] values)
        {
            if (values.Length != verticesDistance.Length)
            {
                Debug.LogError($"Can't set value, Wrong Length.");
            }

            for (int i = 0; i < values.Length; i++)
            {
                float value = Mathf.Clamp(values[i], 0f, 1f);
                verticesDistance[i] = value;
            }

            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            InitUserData();

            DrawMesh(vh);
        }

        private void InitUserData()
        {
            if (verticesDistance.Length != sides)
            {
                verticesDistance = new float[sides];
                for (int i = 0; i < verticesDistance.Length; i++)
                {
                    verticesDistance[i] = 1f;
                }
                filled = 1f;
            }
        }

        private void DrawMesh(VertexHelper vh)
        {
            Mesh mesh = new Mesh();
            float degrees = 360f / sides;
            if (filled == 1f)
            {
                DrawFilledMesh(vh, degrees);
            }
            else
            {
                DrawUnFilledMesh(vh, degrees);

            }
            vh.FillMesh(mesh);
        }

        private void DrawFilledMesh(VertexHelper vh, float degrees)
        {
            vh.AddVert(rectTransform.anchoredPosition3D, color, new Vector2(0, 0));
            for (int i = 0; i < verticesDistance.Length; i++)
            {
                float radian = Mathf.Deg2Rad * (degrees * i + rotation);
                float x = Mathf.Cos(radian);
                float y = Mathf.Sin(radian);
                x *= rectTransform.sizeDelta.x / 2 * verticesDistance[i];
                y *= rectTransform.sizeDelta.y / 2 * verticesDistance[i];

                Vector3 vertex = new Vector3(x, y, 0);
                vertex += rectTransform.anchoredPosition3D;
                vh.AddVert(vertex, color, new Vector2(0, 0));
            }
            for (int i = 0; i < verticesDistance.Length; i++)
            {
                int second = i + 1;
                int third = i + 2 > verticesDistance.Length ? 1 : i + 2;
                vh.AddTriangle(0, second, third);
            }
        }

        private void DrawUnFilledMesh(VertexHelper vh, float degrees)
        {
            List<Vector3> vertices = new List<Vector3>();

            for (int i = 0; i < verticesDistance.Length; i++)
            {
                float radian = Mathf.Deg2Rad * (degrees * i + rotation);
                float x = Mathf.Cos(radian);
                float y = Mathf.Sin(radian);
                x *= rectTransform.sizeDelta.x / 2 * verticesDistance[i];
                y *= rectTransform.sizeDelta.y / 2 * verticesDistance[i];
                Vector3 vertex = new Vector3(x, y, 0);
                vertex += rectTransform.anchoredPosition3D;

                var reverse = 1f - filled;
                float innerX = vertex.x * reverse;
                float innerY = vertex.y * Mathf.Abs(0f - reverse);
                Vector3 innerVertex = new Vector3(innerX, innerY, 0);

                vertices.Add(innerVertex);
                vertices.Add(vertex);
            }

            Vector2[] uvPos = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };

            for (int i = 0; i < vertices.Count; i += 2)
            {
                UIVertex[] uiVertices = new UIVertex[4];
                for (int j = 0; j < uiVertices.Length; j++)
                {
                    int index = i + j >= vertices.Count ? (i + j) - vertices.Count : i + j;
                    uiVertices[j].position = vertices[index];
                    uiVertices[j].uv0 = uvPos[j];
                    uiVertices[j].color = color;
                }
                var tempPos = uiVertices[3];
                uiVertices[3] = uiVertices[2];
                uiVertices[2] = tempPos;
                vh.AddUIVertexQuad(uiVertices);
            }
        }
    }
}
