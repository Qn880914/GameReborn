using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor.Tool
{
    public class MeshAnimationExporter
    {
        public static void ExportCombinedTexture(GameObject prefab, MeshAnimationExportParameter param, System.Action<float> pProgressBarUpdater = null)
        {
            if(!CheckExport(prefab, param))
            {
                return;
            }

            AnimationClip[] animationClips = param.animationClips;
            string[] animationClipNames = param.animationClipNames;
            Transform[] boneTransforms = param.boneTransforms;
            string[] boneNames = param.boneNames;
            GameObject model = prefab;
            Transform rootTransform = prefab.transform;
            Animation animation = model.GetComponentInChildren<Animation>();

            SkinnedMeshRenderer[] meshRenders = model.GetComponentsInChildren<SkinnedMeshRenderer>();
            int meshRenderCount = meshRenders.Length;

            Mesh[] exportMeshs = new Mesh[meshRenderCount];
            for(int i = 0; i < meshRenderCount; ++ i)
            {
                exportMeshs[i] = meshRenders[i].sharedMesh;
            }

            float frameInterval = 1.0f / param.framerate;


            //清空并重建目标目录
            if (System.IO.Directory.Exists(param.outputFilePath))
                System.IO.Directory.Delete(param.outputFilePath, true);
            System.IO.Directory.CreateDirectory(param.outputFilePath);

            int totalFrameCount = 0;
            totalFrameCount = 1 + meshRenderCount;

            int totalAniClipFrameCount = 0;
            foreach(var clip in animationClips)
            {
                totalAniClipFrameCount += (int)(clip.length / frameInterval) + 1;
            }

            // txture height
            // 1 + mesh count + all clip frame * mesh count
            totalFrameCount += totalAniClipFrameCount * meshRenderCount;

            int[] vertexCount = new int[meshRenderCount];
            int maxVertexCount = 0;
            for(int i = 0; i < meshRenderCount; ++ i)
            {
                int count = exportMeshs[i].vertexCount;
                vertexCount[i] = count;
                if (count > maxVertexCount)
                    maxVertexCount = count;
            }

            // 行 ： 每帧顶点数据
            // 列 : 1 + mesh count + all clip frame * mesh count
            Texture2D combinedTex = new Texture2D(maxVertexCount, totalFrameCount, TextureFormat.RGBAHalf, false);



            Vector3[][] defaultAnimationInfos = new Vector3[meshRenderCount][];
            for (int i = 0; i < meshRenderCount; i++)
            {
                defaultAnimationInfos[i] = new Vector3[8];
                for (int j = 0; j < 8; j++)
                {
                    defaultAnimationInfos[i][j] = new Vector3(0, 0, 0);
                }

            }

            string[] defaultAnimationArray = new string[] { "Attack1", "Attack2", "Dead", "Hit", "Run", "Skill1", "Wait1", "Wait2" };
            List<string> defaultAnimationList = new List<string>();
            for (int i = 0; i < defaultAnimationArray.Length; i++)
            {
                defaultAnimationList.Add(defaultAnimationArray[i].ToUpper());

            }

            // （0， 0）点存储   render的个数 和 最大顶点数
            combinedTex.SetPixel(0, 0, new Color(meshRenderCount, maxVertexCount, 0));

            int countData = 1 + meshRenderCount;
            for (int i = 0; i < meshRenderCount; ++ i)
            {
                combinedTex.SetPixel(i + 1, 0, new Color(vertexCount[i], 0, 0));
                countData += exportMeshs[i].uv.Length / 2 + exportMeshs[i].triangles.Length / 3;
            }

            // 配置纹理 fps 、 uv count、uv、triangle count、 triangle
            int texSize = 64;
            int texHeight = (int)Mathf.Ceil(countData / (float)texSize);
            int texWidth = texSize;
            Texture2D cfgTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBAHalf, false);
            Color[] colors = new Color[texWidth * texHeight];
            int cfgDataIndex = 0;
            colors[cfgDataIndex++] = new Color(meshRenderCount, 0, 0);
            for(int i = 0; i < meshRenderCount; ++ i)
            {
                colors[cfgDataIndex++] = new Color(param.framerate, exportMeshs[i].uv.Length, exportMeshs[i].triangles.Length);
            }

            int frame = 1 + meshRenderCount;
            for(int subMeshCount = 0; subMeshCount < meshRenderCount; ++subMeshCount)
            {
                for(int i = 0; i < param.clipCount; ++ i)
                {
                    MeshAnimationBoneGroup boneGroup = new MeshAnimationBoneGroup(param.boneNames.ToList());
                    AnimationClip clip = param.animationClips[i];

                    if (null == clip)
                        continue;

                    int infoIndex = -1;
                    string clipName = clip.name;
                    if (clipName.Equals("walk"))
                        clipName = "Hit";
                    else if (clipName.Equals("cut"))
                        clipName = "Skill1";
                    else if (clipName.Equals("Victory"))
                        clipName = "Dead";

                    string upperName = clipName.ToUpper();
                    if (defaultAnimationList.Contains(upperName))
                    {
                        infoIndex = defaultAnimationList.IndexOf(upperName);
                        defaultAnimationInfos[subMeshCount][infoIndex].x = 1;
                    }

                    animation.AddClip(clip, clip.name);
                    animation.clip = clip;
                    AnimationState state = animation[clip.name];
                    state.enabled = true;
                    state.weight = 1;

                    float clipLength = clip.length;

                    List<float> frameTimes = GetFrameTimes(clipLength, frameInterval);
                    try
                    {
                        defaultAnimationInfos[subMeshCount][infoIndex].y = frame;
                    }
                    catch
                    {
                        Debug.LogError(defaultAnimationInfos.Length + "," + defaultAnimationInfos.LongLength + "," + subMeshCount + "," + infoIndex);
                    }
                    foreach (float time in frameTimes)
                    {
                        state.time = time;

                        animation.Play();
                        animation.Sample();

                        // Grab the position and rotation for each bone at the current frame
                        for (int k = 0; k < param.boneTransforms.Length; k++)
                        {
                            string name = param.boneNames[k];

                            Vector3 pos = rootTransform.InverseTransformPoint(param.boneTransforms[k].position);
                            Quaternion rot = param.boneTransforms[k].rotation * Quaternion.Inverse(rootTransform.rotation);

                            boneGroup.boneInfos[name].positions.Add(pos);
                            boneGroup.boneInfos[name].rotations.Add(rot);


                        }

                        Mesh bakeMesh = null;

                        if (param.quaternionOffset != Quaternion.identity)
                        {
                            Matrix4x4 matrix = new Matrix4x4();
                            matrix.SetTRS(Vector2.zero, param.quaternionOffset, Vector3.one);
                            bakeMesh = BakeFrameAfterMatrixTransform(meshRenders[subMeshCount], matrix);
                        }
                        else
                        {
                            bakeMesh = new Mesh();
                            meshRenders[subMeshCount].BakeMesh(bakeMesh);
                        }

                        for (int k = 0; k < bakeMesh.vertexCount; k++)
                        {
                            Vector3 vertex = bakeMesh.vertices[k];
                            combinedTex.SetPixel(k, frame, new Color(vertex.x, vertex.y, vertex.z));
                        }

                        bakeMesh.Clear();
                        Object.DestroyImmediate(bakeMesh);

                        frame++;

                        animation.Stop();
                    }
                    //end frame position,exclude
                    defaultAnimationInfos[subMeshCount][infoIndex].z = frame;
                }

                for (int i = 0; i < exportMeshs[subMeshCount].uv.Length / 2; i++)
                {
                    int uvIdx = i * 2;
                    colors[cfgDataIndex++] = new Color(exportMeshs[subMeshCount].uv[uvIdx].x, exportMeshs[subMeshCount].uv[uvIdx].y,
                        exportMeshs[subMeshCount].uv[uvIdx + 1].x, exportMeshs[subMeshCount].uv[uvIdx + 1].y);

                }
                for (int i = 0; i < exportMeshs[subMeshCount].triangles.Length / 3; i++)
                {
                    int triIdx = i * 3;
                    colors[cfgDataIndex++] = new Color(exportMeshs[subMeshCount].triangles[triIdx], exportMeshs[subMeshCount].triangles[triIdx + 1], 
                        exportMeshs[subMeshCount].triangles[triIdx + 2]);
                }
            }

            for (int j = 0; j < meshRenderCount; j++)
            {

                for (int i = 0; i < 8; i++)
                {
                    combinedTex.SetPixel(i, j + 1, new Color(defaultAnimationInfos[j][i].x, defaultAnimationInfos[j][i].y, defaultAnimationInfos[j][i].z));
                }

            }






            combinedTex.Apply(false);

            string dataPath = param.outputFilePath + "/" + "combinedTex" + ".asset";

            AssetDatabase.CreateAsset(combinedTex, dataPath);





            cfgTexture.SetPixels(colors);
            cfgTexture.Apply();



            AssetDatabase.CreateAsset(cfgTexture, param.outputFilePath + "/" + prefab.name + ".asset");

            if (pProgressBarUpdater != null)
            {
                pProgressBarUpdater((float)param.animationClips.Length / (float)(param.animationClips.Length + 1));
            }

            //MeshAnimationProtobufHelper.SerializeObject<MeshAnimationGroupSerializable>(pSettings.outputFilePath + "/" + pFbxInstance.name + ".bytes", group);

            EditorUtility.DisplayDialog("Tip", "Mesh Animation Export Complete" + prefab.name, "OK");
            AssetDatabase.Refresh();
        }

        private static bool CheckExport(GameObject prefab, MeshAnimationExportParameter param)
        {
            if (string.IsNullOrEmpty(param.outputFilePath.Trim()))
            {
                EditorUtility.DisplayDialog("Missing Output File", "Please set a output file.", "OK");
                return false;
            }

            if (prefab == null)
            {
                EditorUtility.DisplayDialog("Missing Base FBX", "Please specify a base FBX.", "OK");
                return false;
            }
            if (PrefabUtility.GetPrefabParent(prefab) == null)
            {
                EditorUtility.DisplayDialog("GameObject must be an instance of a ModelPrefab", "Please select a valid GameObject", "OK");
            }

            bool clipsNotSet = true;

            for (int i = 0; i < param.animationClips.Length; i++)
            {
                if (param.animationClips[i] != null && string.IsNullOrEmpty(param.animationClipNames[i].Trim()))
                {
                    EditorUtility.DisplayDialog("Missing Animation Name", "Please specify a name for all animation files.", "OK");
                    return false;
                }

                if (param.animationClips[i] != null)
                {
                    clipsNotSet = false;
                }
            }

            if (clipsNotSet)
            {
                EditorUtility.DisplayDialog("Missing Animation", "Please specify at least one animation file.", "OK");
                return false;
            }

            for (int i = 0; i < param.boneTransforms.Length; i++)
            {
                if (param.boneTransforms[i] != null && string.IsNullOrEmpty(param.boneNames[i].Trim()))
                {
                    EditorUtility.DisplayDialog("Missing Bone Name", "Please specify a name for all bone transforms.", "OK");
                    return false;
                }
            }

            return true;
        }

        // <summary>
        /// Calculate the time periods when a frame snapshot should be taken
        /// </summary>
        /// <returns>The frame times.</returns>
        /// <param name="pLength">P length.</param>
        /// <param name="pInterval">P interval.</param>
        public static List<float> GetFrameTimes(float pLength, float pInterval)
        {
            List<float> times = new List<float>();

            float time = 0;

            do
            {
                times.Add(time);
                time += pInterval;
            } while (time < pLength);

            times.Add(pLength);

            return times;
        }

        public static Mesh BakeFrameAfterMatrixTransform(SkinnedMeshRenderer pRenderer, Matrix4x4 matrix)
        {
            Mesh result = new Mesh();
            pRenderer.BakeMesh(result);
            result.vertices = TransformVertices(matrix, result.vertices);
            return result;
        }

        /// <summary>
        /// Convert a set of vertices using the given transform matrix.
        /// </summary>
        /// <returns>Transformed vertices</returns>
        /// <param name="pLocalToWorld">Transform Matrix</param>
        /// <param name="pVertices">Vertices to transform</param>
        public static Vector3[] TransformVertices(Matrix4x4 pLocalToWorld, Vector3[] pVertices)
        {
            Vector3[] result = new Vector3[pVertices.Length];

            for (int i = 0; i < pVertices.Length; i++)
            {
                result[i] = pLocalToWorld * pVertices[i];
            }

            return result;
        }
    }
}
