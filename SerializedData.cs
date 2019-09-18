using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.CommonLibrary
{
    [Serializable]
    public class SerializedData
    {
        public Type type;
        public string instanceData;
        public bool isSceneObject;
        public int instanceId;
        public List<string> fieldName = new List<string>();
        public List<string> assetGuids = new List<string>();
        public static SerializedData Serialize(object obj)
        {
            if (obj != null)
            {
                var result = new SerializedData()
                {
                    type = obj.GetType(),
                    instanceData = JsonUtility.ToJson(obj)
                };

#if UNITY_EDITOR

                //TODO:determine how to check if instance or uninstantiated asset
                bool isAsset = false;
                if (!isAsset) {
                    result.isSceneObject = true;
                    result.instanceId = ((UnityEngine.Object)obj).GetInstanceID();
                }
                else {
                    var fields = result.type
                        .GetFields()
                        .Where(x => x.FieldType
                        .IsSubclassOf(typeof(UnityEngine.Object)))
                        .ToList();
                    foreach (var asset in fields)
                    {
                        try
                        {
                            var assetPath = AssetDatabase.GetAssetPath((UnityEngine.Object)asset.GetValue(obj));
                            if (assetPath != null)
                            {
                                var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
                                if (assetGuid != null)
                                {
                                    result.assetGuids.Add(assetGuid);
                                    result.fieldName.Add(asset.Name);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex);
                        }
                    }
                }
#endif

                return result;
            }
            return null;
        }
        //Returns as an object, which can then be cast
        public static object Deserialize(SerializedData sd)
        {

            if (sd != null)
            {
                try
                {
                    object obj;

                    if (sd.isSceneObject)
                    {
                        obj = EditorUtility.InstanceIDToObject(sd.instanceId);
                    }
                    else
                    {
                        obj = JsonUtility.FromJson(sd.instanceData, sd.type);

#if UNITY_EDITOR
                        var assetsToLoad = sd.type
                            ?.GetFields()
                            ?.Where(x => x?.FieldType
                            ?.IsSubclassOf(typeof(UnityEngine.Object)) ?? false)
                            ?.ToList();

                        foreach (var assetInfo in assetsToLoad)
                        {
                            try
                            {
                                var assetPath = AssetDatabase.GUIDToAssetPath(sd.assetGuids[sd.fieldName.IndexOf(assetInfo.Name)]);
                                if (assetPath != null)
                                {
                                    var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetInfo.DeclaringType);

                                    if (asset != null)
                                        assetInfo.SetValue(obj, asset);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex);
                            }
                        }
#endif
                    }
                    return obj;
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
            return null;
        }
    }
}
