using UnityEngine;

namespace DrawCrusher.DrawingField
{
    [System.Serializable]
    public class DrawSettings
    {
        [Header("Layer Settings")]
        [Tooltip("The obstacle detection layer")]
        public LayerMask cantDrawLayers;
        [Tooltip("The layer of the mesh while drawing, don't equal the cant draw layers")]
        public int drawingMeshLayer = 8;
        [Tooltip("The layer of the mesh after finishing drawing")]
        public int drawedMeshLayer = 0;


        [Header("Material Settings")]
        [Tooltip("The material of the mesh while drawing")]
        public Material drawingMeshMaterial;
        [Tooltip("The material of the mesh after finishing drawing")]
        public Material drawedMeshMaterial;
        [Tooltip("The physics material of the mesh after finishing drawing")]
        public PhysicsMaterial2D drawedMeshPhysicsMaterial;


        [Header("Mesh Size Settings")]
        [Range(0.05f, 1f), Tooltip("Each time the interval is increased, the smaller the smoother")]
        public float stepLength;
        [Range(0.02f, 5f), Tooltip("The start width of the mesh")]
        public float startWidth;
        [Range(0.02f, 5f), Tooltip("The end width of the mesh")]
        public float endWidth;
        [Range(0.02f, 20f), Tooltip("Mesh depth, only works for 3D mode")]
        public float meshDepth;


        [Header("Texture Settings")]
        [Tooltip("Whether the texture scrolls when drawing")]
        public bool textureScrolling;
        [Tooltip("Whether the texture is scaled")]
        public bool textureAllowScale;
        [Range(0.02f, 20f)]
        [Tooltip("Standard texture width")]
        public float textureNormalizedWidth;


        [Header("Length Limit Settings")]
        [Tooltip("Whether to limit the length")]
        public bool lengthLimit;
        [Tooltip("If limit length is enabled, this is the maximum length allowed to draw")]
        public float maxLength;


        [Header("Auto Disappear Settings")]
        [Tooltip("Whether to enable auto disappear")]
        public bool autoDisappear;
        [Tooltip("If autoDisappear is enabled, the mesh will automatically disappear after survivalTime")]
        public float survivalTime;


        [Header("Other Settings")]
        [Tooltip("Whether to generate collider realtime when drawing (expensive)")]
        public bool createColliderWhenDrawing;
    }
}
