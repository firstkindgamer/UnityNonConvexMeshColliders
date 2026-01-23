using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class VoxelCollider : NonConvexCollider<VoxelizedUserSettings, VoxelizedColliderData>{

    
    //Optimization options
    // use a BVH to speed up raycasts
    // parallelize voxel processing
    // reuse collider components instead of adding / removing repeatedly (not much gain without other optimizations)
    /// <summary>
    /// Bake voxel colliders for the attached mesh
    /// 
    /// </summary>
    [ContextMenu("Bake Colliders")]
    public override void BakeColliders() {

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        MeshFilter mf = GetComponent<MeshFilter>();
        if(mf == null || mf.sharedMesh == null) {
            Debug.LogWarning("VoxelColliderTest: No MeshFilter / Mesh found on this GameObject.");
            return;
        }

        Mesh mesh = mf.sharedMesh;
        Bounds b = MeshColliderData.GetBounds(mesh, subMeshIndices);
        MeshColliderData meshData = new MeshColliderData(mesh, subMeshIndices);

        


        colliderData = new VoxelizedColliderData();
        VoxelColliderCreator voxelCreator = new VoxelColliderCreator(transform.lossyScale, b, colliderData);
        voxelCreator.BakeCollider(meshData, settings);
        stopwatch.Stop();
        if(TimeIt) {
            Debug.Log($"VoxelColliderTest: Voxel collider generation took {stopwatch.ElapsedMilliseconds} ms.");
        }
        if(PrintCreatedPoints) {
            Debug.Log($"VoxelColliderTest: Generated {colliderData.VoxelCenters.Count} filled voxels.");
        }
        
        if(GenerateColliders) {
            stopwatch.Reset();
            stopwatch.Start();
            ClearColliders();
            GenerateCollisionColliders();
            stopwatch.Stop();
            if(TimeIt) {
                Debug.Log($"VoxelColliderTest: Collision Creation took {stopwatch.ElapsedMilliseconds} ms.");
            }
        }


    }

    [ContextMenu("Generate Collisions")]
    public void GenerateCollisions() {
        ClearColliders();
        GenerateCollisionColliders();
    }
    [ContextMenu("Clear Colliders")]
    public void ClearAllColliders() {
        ClearColliders();
    }
}