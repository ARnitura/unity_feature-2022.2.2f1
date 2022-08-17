using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RedChair : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Transform ob;
        if (this.gameObject.transform.childCount == 1)
        {
            ob = this.gameObject.transform.GetChild(0);
        }
        else
        {
            ob = this.gameObject.transform;
        }
        for (int i = 0; i < ob.childCount; i++) // Проход по всем мешам
        {
            Debug.Log("Count mesh: " + i);
            var comp = ob.GetChild(i).GetComponent<MeshRenderer>();
            for (int j = 0; j < comp.materials.Length; j++) // Проход по всем материалам
            {
                var maps =
                    "/var/mobile/Containers/Data/Application/B9164A04-40F3-4142-BF40-EF19F2B8B066/Documents/files/1/models/textures/1/Wood_Roughness.png, /var/mobile/Containers/Data/Application/B9164A04-40F3-4142-BF40-EF19F2B8B066/Documents/files/1/models/textures/1/Leather_Specular.png, /var/mobile/Containers/Data/Application/B9164A04-40F3-4142-BF40-EF19F2B8B066/Documents/files/1/models/textures/1/Wood_Normal.png, /var/mobile/Containers/Data/Application/B9164A04-40F3-4142-BF40-EF19F2B8B066/Documents/files/1/models/textures/1/Leather_Rough.png, /var/mobile/Containers/Data/Application/B9164A04-40F3-4142-BF40-EF19F2B8B066/Documents/files/1/models/textures/1/Leather_BaseColor.jpg, /var/mobile/Containers/Data/Application/B9164A04-40F3-4142-BF40-EF19F2B8B066/Documents/files/1/models/textures/1/Leather_Normal.jpg, /var/mobile/Containers/Data/Application/B9164A04-40F3-4142-BF40-EF19F2B8B066/Documents/files/1/models/textures/1/Wood_BaseColor.png, /var/mobile/Containers/Data/Application/B9164A04-40F3-4142-BF40-EF19F2B8B066/Documents/files/1/models/textures/1/Leather_AO.jpg"
                        .Split(", ").ToList(); // Карты для мешей
                for (int k = 0; k < maps.Count; k++) // Распределение текстур относительно материалов
                    {
                        var find_material = ob.GetChild(i).GetComponent<MeshRenderer>().materials[j].name.Split(".")[0];
                        if (maps[k].Contains(find_material + "_BaseColor"))
                        {
                            /*var BaseColor = LoadTextureData(maps[k]); 
                            comp.material.SetTexture("_MainTex", BaseColor);*/
                            maps.RemoveAt(k);
                            Debug.Log("_MainTex: successful");
                        } // Поиск BaseColor для материала
                        else if (maps[k].Contains(find_material + "_Normal"))
                        {
                            /*var normalMap = LoadTextureData(maps[k]);
                            comp.material.SetTexture("_BumpMap", normalMap);*/
                            maps.RemoveAt(k);
                            Debug.Log("_BumpMap: successful");
                        } // Поиск Normal для материала
                        else if (maps[k].Contains(find_material + "_Height"))
                        {
                            /*var HeightMap = LoadTextureData(maps[k]);
                            comp.material.SetTexture("_Height", HeightMap);*/
                            maps.RemoveAt(k);
                            Debug.Log("_Height: successful");
                        } // Поиск HeightMap для материала
                        else if (maps[k].Contains(find_material + "_MetallicGlossMap"))
                        {
                            /*var MetallicGlossMap = LoadTextureData(maps[k]);
                            comp.material.SetTexture("_MetallicGlossMap", MetallicGlossMap);*/
                            maps.RemoveAt(k);
                            Debug.Log("_MetallicGlossMap: successful");
                        } // Поиск _MetallicGlossMap для материала
                        else if (maps[k].Contains(find_material + "_OcclusionMap"))
                        {
                            /*var OcclusionMap = LoadTextureData(maps[k]);
                            comp.material.SetTexture("_OcclusionMap", OcclusionMap);*/
                            maps.RemoveAt(k);
                            Debug.Log("_OcclusionMap: successful");
                        } // Поиск _OcclusionMap для материала
                        else if (maps[k].Contains(find_material + "_EmissionMap"))
                        {
                            /*var EmissionMap = LoadTextureData(maps[k]);
                            comp.material.SetTexture("_EmissionMap", EmissionMap);*/
                            maps.RemoveAt(k);
                            Debug.Log("_EmissionMap: successful");
                        } // Поиск _EmissionMap для материала
                        else if (maps[k].Contains(find_material + "_Glossiness"))
                        {
                            /*var GlossinessMap = LoadTextureData(maps[k]);
                            comp.material.SetTexture("_Glossiness", GlossinessMap);*/
                            maps.RemoveAt(k);
                            Debug.Log("_Glossiness: successful");
                        }
                    }
                
            } // Цикл прохода по картам
        } // Цикл для прохода по мешам
    }
    // Update is called once per frame
    void Update()
    {
    }
}