using LifeRPG.Data;
using UnityEngine;
using UnityEngine.UI;

namespace LifeRPG.UI.MainPanel
{
    public class DimensionRadarChart : Graphic
    {
        [SerializeField] private Color currentFillColor = new Color(0.35f, 0.75f, 0.35f, 0.24f);
        [SerializeField] private Color currentLineColor = new Color(0.25f, 0.68f, 0.28f, 1f);
        [SerializeField] private Color targetLineColor = new Color(1f, 0.54f, 0.16f, 1f);
        [SerializeField] private Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.25f);
        [SerializeField] private float maxValue = 10f;

        private readonly float[] currentValues = new float[6];
        private readonly float[] targetValues = new float[6];

        public void Refresh(DimensionSet currentDimensions, DimensionSet targetDimensions)
        {
            DimensionType[] types =
            {
                DimensionType.Body,
                DimensionType.Knowledge,
                DimensionType.Career,
                DimensionType.Relationship,
                DimensionType.Wealth,
                DimensionType.Happiness
            };

            for (int i = 0; i < types.Length; i++)
            {
                currentValues[i] = currentDimensions != null ? currentDimensions.GetValue(types[i]) : 0f;
                targetValues[i] = targetDimensions != null ? targetDimensions.GetValue(types[i]) : 0f;
            }

            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect rect = rectTransform.rect;
            Vector2 center = rect.center;
            float radius = Mathf.Min(rect.width, rect.height) * 0.38f;

            for (int ring = 1; ring <= 4; ring++)
            {
                DrawPolygonLine(vh, BuildPoints(center, radius * ring / 4f, null), gridColor, 1.5f);
            }

            Vector2[] currentPoints = BuildPoints(center, radius, currentValues);
            Vector2[] targetPoints = BuildPoints(center, radius, targetValues);

            DrawFilledPolygon(vh, currentPoints, currentFillColor);
            DrawPolygonLine(vh, currentPoints, currentLineColor, 3f);
            DrawPolygonLine(vh, targetPoints, targetLineColor, 2f);
        }

        private Vector2[] BuildPoints(Vector2 center, float radius, float[] values)
        {
            Vector2[] points = new Vector2[6];
            for (int i = 0; i < points.Length; i++)
            {
                float valueScale = values == null ? 1f : Mathf.Clamp01(values[i] / Mathf.Max(1f, maxValue));
                float angle = Mathf.PI * 0.5f - i * Mathf.PI * 2f / points.Length;
                points[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * valueScale;
            }

            return points;
        }

        private void DrawFilledPolygon(VertexHelper vh, Vector2[] points, Color fillColor)
        {
            int startIndex = vh.currentVertCount;
            vh.AddVert(rectTransform.rect.center, fillColor, Vector2.zero);

            foreach (Vector2 point in points)
            {
                vh.AddVert(point, fillColor, Vector2.zero);
            }

            for (int i = 0; i < points.Length; i++)
            {
                int next = i == points.Length - 1 ? 1 : i + 2;
                vh.AddTriangle(startIndex, startIndex + i + 1, startIndex + next);
            }
        }

        private void DrawPolygonLine(VertexHelper vh, Vector2[] points, Color lineColor, float width)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 start = points[i];
                Vector2 end = points[(i + 1) % points.Length];
                DrawLine(vh, start, end, lineColor, width);
            }
        }

        private void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, Color lineColor, float width)
        {
            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * width * 0.5f;
            int index = vh.currentVertCount;

            vh.AddVert(start - normal, lineColor, Vector2.zero);
            vh.AddVert(start + normal, lineColor, Vector2.zero);
            vh.AddVert(end + normal, lineColor, Vector2.zero);
            vh.AddVert(end - normal, lineColor, Vector2.zero);
            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index, index + 2, index + 3);
        }
    }
}
