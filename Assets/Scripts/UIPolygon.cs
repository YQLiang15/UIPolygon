using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
    public class UIPolygon : MaskableGraphic
    {
        public int Sides { get { return sides; } }

        [SerializeField, Range(3, 360)] private int sides = 3;
        [SerializeField] private float rotation;
        [SerializeField, Range(0f, 1f)] private float[] verticesDistance = new float[] { };
        [SerializeField, Range(0f, 1f)] private float filled;
        private List<Vector3> unfilledVertices = new List<Vector3>();
        private Vector2[] uvVector = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };

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

        public void SetRotation(float rotation)
        {
            this.rotation += rotation;
            SetVerticesDirty();
        }

        public void SetFilled(float filled)
        {
            this.filled = filled;
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
            vh.AddVert(Vector3.zero, color, new Vector2(0, 0));
            for (int i = 0; i < verticesDistance.Length; i++)
            {
                Vector3 vertex = GetVertex(degrees, i);
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
            unfilledVertices.Clear();

            for (int i = 0; i < verticesDistance.Length; i++)
            {
                Vector3 vertex = GetVertex(degrees, i);

                // Use "formula of inner division point" to get point.
                var reverse = 1f - filled;
                float innerX = vertex.x * reverse;
                float innerY = vertex.y * reverse;
                Vector3 innerVertex = new Vector3(innerX, innerY, 0);

                unfilledVertices.Add(innerVertex);
                unfilledVertices.Add(vertex);
            }

            for (int i = 0; i < unfilledVertices.Count; i += 2)
            {
                UIVertex[] uiVertices = new UIVertex[4];
                for (int j = 0; j < uiVertices.Length; j++)
                {
                    int index = i + j >= unfilledVertices.Count ? (i + j) - unfilledVertices.Count : i + j;
                    uiVertices[j].position = unfilledVertices[index];
                    uiVertices[j].uv0 = uvVector[j];
                    uiVertices[j].color = color;
                }
                // Swap positon to make quad.
                var tempPos = uiVertices[3];
                uiVertices[3] = uiVertices[2];
                uiVertices[2] = tempPos;
                vh.AddUIVertexQuad(uiVertices);
            }
        }

        private Vector3 GetVertex(float degrees, int index)
        {
            float radian = Mathf.Deg2Rad * (degrees * index + rotation);
            float x = Mathf.Cos(radian);
            float y = Mathf.Sin(radian);
            x *= rectTransform.sizeDelta.x / 2 * verticesDistance[index];
            y *= rectTransform.sizeDelta.y / 2 * verticesDistance[index];
            Vector3 vertex = new Vector3(x, y, 0);
            return vertex;
        }
    }
}