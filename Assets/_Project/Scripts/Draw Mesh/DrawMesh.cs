using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawCrusher.DrawingField
{
    /// <summary>
    /// Convert the points in PolygonCollider2D to the corresponding 2D or 3D mesh.
    /// </summary>
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(PolygonCollider2D)), RequireComponent(typeof(MeshRenderer))]
    public class DrawMesh : MonoBehaviour
    {
        public static int cloneNumber;

        public float Length { get; private set; }

        public float Area
        {
            get
            {
                if (drawSettings != null)
                {
                    return Length * (drawSettings.startWidth + drawSettings.endWidth) * 0.5f;
                }
                return 0;
            }
        }

        private DrawSettings drawSettings;
        private PolygonCollider2D polygonCollider2d;
        private Rigidbody2D rigidbody2d;
        private Mesh mesh;

        private int grid;
        private int line;
        private int ring;

        private List<float> lengths = new List<float>();
        private List<Vector2> points = new List<Vector2>();
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Vector2> uvs = new List<Vector2>();

        public void Init(DrawSettings drawSettings)
        {
            this.drawSettings = drawSettings;
            gameObject.layer = drawSettings.drawingMeshLayer;
            gameObject.name = "Drawed" + cloneNumber;
            cloneNumber++;

            lengths.Clear();
            Length = 0;
            lengths.Add(Length);

            mesh = new Mesh { name = "Draw Mesh" };
            mesh.MarkDynamic();
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshRenderer>().sharedMaterial = drawSettings.drawingMeshMaterial;
            polygonCollider2d = GetComponent<PolygonCollider2D>();
            rigidbody2d = GetComponent<Rigidbody2D>();
            rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
            rigidbody2d.gravityScale = drawSettings.gravityScale;
            rigidbody2d.sharedMaterial = drawSettings.drawedMeshPhysicsMaterial;
            polygonCollider2d.sharedMaterial = drawSettings.drawedMeshPhysicsMaterial;
        }

        public void SetPoints(List<Vector2> points, float addLength)
        {
            this.points = points;
            grid = this.points.Count / 2 - 1;
            line = grid + 1;
            ring = line * 2;
            Length += addLength;
            lengths.Add(Length);

            if (drawSettings.createColliderWhenDrawing) polygonCollider2d.SetPath(0, points);

            GenerateMesh();
        }

        public void EndDraw(Vector2 centOfMass)
        {
            if (!drawSettings.createColliderWhenDrawing) polygonCollider2d.SetPath(0, points);

            rigidbody2d.centerOfMass = centOfMass;
            rigidbody2d.mass = SetMass();

            GetComponent<MeshRenderer>().sharedMaterial = drawSettings.drawedMeshMaterial;


            gameObject.layer = drawSettings.drawedMeshLayer;

            rigidbody2d.bodyType = RigidbodyType2D.Static;
            if (drawSettings.autoDisappear)
            {
                DelayDestroy(gameObject, drawSettings.survivalTime).Forget();
            }
        }
        private async UniTaskVoid DelayDestroy(GameObject gameObject,float survivalTime)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(survivalTime), ignoreTimeScale: false);
            Destroy(gameObject);

        }
        public void Destroy()
        {
            cloneNumber--;
            Destroy(gameObject);
        }

        private float SetMass()
        {
            if (drawSettings.useDynamicMass == true)
            {
                // Roughly as a trapezoid, calculate the weight according to the area.
                return Area;
            }
            else
            {
                return rigidbody2d.mass;
            }
        }

        #region Generate Mesh
        private void GenerateMesh()
        {
            CalculateVerticesAndUV3D();
            CalculateTriangles3D();

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
        }
        #region Normal3D
        private void CalculateVerticesAndUV3D()
        {
            vertices.Clear();
            uvs.Clear();

            // Front
            for (int i = 0; i < line; i++)
            {
                vertices.Add(new Vector3(points[i].x, points[i].y, -drawSettings.meshDepth * 0.5f));
                uvs.Add(GetUVWithLerp(i, false));
            }
            for (int i = 0; i < line; i++)
            {
                vertices.Add(new Vector3(points[points.Count - 1 - i].x, points[points.Count - 1 - i].y, -drawSettings.meshDepth * 0.5f));
                uvs.Add(GetUVWithLerp(i, true));
            }

            // Top
            for (int i = 0; i < line; i++)
            {
                vertices.Add(new Vector3(points[points.Count - 1 - i].x, points[points.Count - 1 - i].y, -drawSettings.meshDepth * 0.5f));
                uvs.Add(GetUVForWidth(i, drawSettings.meshDepth, false));
            }
            for (int i = 0; i < line; i++)
            {
                vertices.Add(new Vector3(points[points.Count - 1 - i].x, points[points.Count - 1 - i].y, drawSettings.meshDepth * 0.5f));
                uvs.Add(GetUVForWidth(i, drawSettings.meshDepth, true));
            }

            // Back
            for (int i = 0; i < line; i++)
            {
                vertices.Add(new Vector3(points[points.Count - 1 - i].x, points[points.Count - 1 - i].y, drawSettings.meshDepth * 0.5f));
                uvs.Add(GetUVWithLerp(i, false));
            }
            for (int i = 0; i < line; i++)
            {
                vertices.Add(new Vector3(points[i].x, points[i].y, drawSettings.meshDepth * 0.5f));
                uvs.Add(GetUVWithLerp(i, true));
            }

            // Bottom
            for (int i = 0; i < line; i++)
            {
                vertices.Add(new Vector3(points[i].x, points[i].y, drawSettings.meshDepth * 0.5f));
                uvs.Add(GetUVForWidth(i, drawSettings.meshDepth, false));
            }
            for (int i = 0; i < line; i++)
            {
                vertices.Add(new Vector3(points[i].x, points[i].y, -drawSettings.meshDepth * 0.5f));
                uvs.Add(GetUVForWidth(i, drawSettings.meshDepth, true));
            }

            // Left
            {
                vertices.Add(new Vector3(points[0].x, points[0].y, -drawSettings.meshDepth * 0.5f));
                vertices.Add(new Vector3(points[points.Count - 1].x, points[points.Count - 1].y, -drawSettings.meshDepth * 0.5f));
                vertices.Add(new Vector3(points[points.Count - 1].x, points[points.Count - 1].y, drawSettings.meshDepth * 0.5f));
                vertices.Add(new Vector3(points[0].x, points[0].y, drawSettings.meshDepth * 0.5f));

                //left uvs
                float uFullRate, uLow, uHigh, vFullRate, vLow, vHigh;

                if (drawSettings.textureAllowScale)
                {
                    uLow = 0;
                    uHigh = 1;

                    vLow = 0;
                    vHigh = 1;
                }
                else
                {
                    uFullRate = drawSettings.meshDepth / drawSettings.textureNormalizedWidth;
                    uLow = 0.5f - uFullRate * 0.5f;
                    uHigh = 0.5f + uFullRate * 0.5f;

                    vFullRate = drawSettings.endWidth / drawSettings.textureNormalizedWidth;
                    vLow = 0.5f - vFullRate * 0.5f;
                    vHigh = 0.5f + vFullRate * 0.5f;
                }

                uvs.Add(new Vector2(uLow, vHigh));
                uvs.Add(new Vector2(uLow, vLow));
                uvs.Add(new Vector2(uHigh, vLow));
                uvs.Add(new Vector2(uHigh, vHigh));
            }

            // Right
            {
                vertices.Add(new Vector3(points[grid].x, points[grid].y, -drawSettings.meshDepth * 0.5f));
                vertices.Add(new Vector3(points[line].x, points[line].y, -drawSettings.meshDepth * 0.5f));
                vertices.Add(new Vector3(points[line].x, points[line].y, drawSettings.meshDepth * 0.5f));
                vertices.Add(new Vector3(points[grid].x, points[grid].y, drawSettings.meshDepth * 0.5f));

                //right uvs
                float uFullRate, uLow, uHigh, vFullRate, vLow, vHigh;

                if (drawSettings.textureAllowScale)
                {
                    uLow = 0;
                    uHigh = 1;

                    vLow = 0;
                    vHigh = 1;
                }
                else
                {
                    uFullRate = drawSettings.meshDepth / drawSettings.textureNormalizedWidth;
                    uLow = 0.5f - uFullRate * 0.5f;
                    uHigh = 0.5f + uFullRate * 0.5f;

                    vFullRate = drawSettings.startWidth / drawSettings.textureNormalizedWidth;
                    vLow = 0.5f - vFullRate * 0.5f;
                    vHigh = 0.5f + vFullRate * 0.5f;
                }

                uvs.Add(new Vector2(uHigh, vHigh));
                uvs.Add(new Vector2(uHigh, vLow));
                uvs.Add(new Vector2(uLow, vLow));
                uvs.Add(new Vector2(uLow, vHigh));
            }
        }

        /// <summary>
        /// Create a triangle index, the mesh created in this way, call to recalculate the normal, you can get the correct normal.
        /// </summary>
        private void CalculateTriangles3D()
        {
            triangles.Clear();

            // front-top-back-bottom
            for (int j = 0; j < 4; j++)
            {
                for (int i = ring * j; i < ring * j + grid; i++)
                {
                    SetQuad(i, i + 1, i + line, i + line + 1);
                }
            }

            int count = vertices.Count;

            // left
            SetQuad(count - 5, count - 8, count - 6, count - 7);

            // right
            SetQuad(count - 4, count - 1, count - 3, count - 2);
        }
        #endregion

        /// <summary>
        /// Calculate v in uv by interpolating the left and right width according to the index.
        /// </summary>
        private Vector2 GetUVWithLerp(int index, bool isLow)
        {
            Vector2 uv = Vector2.zero;

            if (drawSettings.textureScrolling)
            {
                uv.x = (Length - lengths[lengths.Count - 1 - index]) / drawSettings.textureNormalizedWidth;
            }
            else
            {
                uv.x = lengths[lengths.Count - 1 - index] / drawSettings.textureNormalizedWidth;
            }

            if (drawSettings.textureAllowScale)
            {
                uv.y = isLow ? 0 : 1;
            }
            else
            {
                float vFullRate = Mathf.Lerp(drawSettings.endWidth, drawSettings.startWidth, lengths[index] / Length) / drawSettings.textureNormalizedWidth;
                uv.y = isLow ? (0.5f - vFullRate * 0.5f) : (0.5f + vFullRate * 0.5f);
            }

            return uv;
        }

        /// <summary>
        /// Calculate v in uv by specifying width.
        /// </summary>
        private Vector2 GetUVForWidth(int index, float width, bool isLow)
        {
            Vector2 uv = Vector2.zero;

            if (drawSettings.textureScrolling)
            {
                uv.x = (Length - lengths[lengths.Count - 1 - index]) / drawSettings.textureNormalizedWidth;
            }
            else
            {
                uv.x = lengths[lengths.Count - 1 - index] / drawSettings.textureNormalizedWidth;
            }

            if (drawSettings.textureAllowScale)
            {
                uv.y = isLow ? 0 : 1;
            }
            else
            {
                float vFullRate = width / drawSettings.textureNormalizedWidth;
                uv.y = isLow ? (0.5f - vFullRate * 0.5f) : (0.5f + vFullRate * 0.5f);
            }

            return uv;
        }

        /// <summary>
        /// Set the triangle index of a single Quad, a Quad contains two triangles with shared vertices.
        /// </summary>
        private void SetQuad(int v00, int v10, int v01, int v11)
        {
            triangles.Add(v00);
            triangles.Add(v01);
            triangles.Add(v10);
            triangles.Add(v10);
            triangles.Add(v01);
            triangles.Add(v11);
        }
        #endregion
    }
}
