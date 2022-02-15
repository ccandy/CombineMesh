using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;




public class MeshCombiner : MonoBehaviour
{

    MeshFilter[] mfs;
    CombineInstance[] combineMeshs;
    private void Start()
    {

        mfs = gameObject.GetComponentsInChildren<MeshFilter>();
        DoCombine();
    }

    private void CombineMesh()
    {
        combineMeshs = new CombineInstance[mfs.Length];
        for(int n = 0; n < mfs.Length; n++)
        {
            combineMeshs[n].mesh = mfs[n].sharedMesh;
            combineMeshs[n].transform = mfs[n].transform.localToWorldMatrix;
            mfs[n].gameObject.SetActive(false);
            
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combineMeshs, false);
        transform.gameObject.SetActive(true);
    }
    



    public void DoCombine()
    {

    }

    private Material CombineMat()
    {
        Material[] materials = new Material[mfs.Length];
        Texture2D[] textures = new Texture2D[mfs.Length];
        
        
        
        for(int n =0; n < mfs.Length; n++)
        {
            MeshRenderer mr = mfs[n].GetComponent<MeshRenderer>();
            materials[n] = mr.sharedMaterial;

            Texture2D tx = mr.sharedMaterial.GetTexture("_MainTex") as Texture2D;
            Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.ARGB32, false);
            tx2D.SetPixels(tx.GetPixels(0, 0, tx.width, tx.height));
            tx2D.Apply();

            textures[n] = tx2D;
        }

        Material combineMat = new Material(materials[0].shader);
        combineMat.CopyPropertiesFromMaterial(materials[0]);

        Texture2D combineTex = new Texture2D(1024, 1024);
        combineMat.SetTexture("_MainTex", combineTex);
        Rect[] rects = combineTex.PackTextures(textures, 10, 1024);


        for (int n = 0; n < mfs.Length; n++)
        {
            Rect rect = rects[n];
            Mesh mesh = mfs[n].sharedMesh;
            Vector2[] uvs = new Vector2[mesh.uv.Length];

            for(int j = 0; j < uvs.Length; n++)
            {
                uvs[j].x = rect.x + mesh.uv[j].x * rect.width;
                uvs[j].y = rect.x + mesh.uv[j].x * rect.width;
            }

            mesh.uv = uvs;
            combineMeshs[n].mesh = mesh;
        }


        return combineMat;

    }
}
