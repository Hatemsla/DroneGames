﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cakeslice;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Builder
{
    public class BuilderManager : MonoBehaviour
    {
        public string levelName;
        public float gridSize;
        public bool canPlace;
        public BuilderUI builderUI;
        public Transform ground;
        public LayerMask layerMask;
        public GameObject pendingObject;
        public TrackObject currentObjectType;
        public Vector3 mousePos;
        public GameObject[] objects;
        public List<GameObject> objectsPool;
        [HideInInspector] public Scene levelScene;
        
        private int _currentGroundIndex;
        private RaycastHit _hit;
        private Selection _selection;
            
        private void Start()
        {
            _selection = FindObjectOfType<Selection>();

            for (int i = 0; i < builderUI.createButtons.Count; i++)
            {
                var i1 = i;
                builderUI.createButtons[i].onClick.AddListener(delegate { SelectObject(i1); });
            }
            
            CreateObjectsPoolScene();
        }

        private void Update()
        {
            Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit, 10000, layerMask) && !EventSystem.current.IsPointerOverGameObject())
            {
                mousePos = _hit.point;
                
                if (pendingObject == null) return;
                
                switch (currentObjectType.objectType)
                {
                    case ObjectsType.Floor:
                        pendingObject.transform.position = _hit.point;
                        break;
                    default:
                        pendingObject.transform.position =
                            new Vector3(_hit.point.x, _hit.point.y + currentObjectType.yOffset, _hit.point.z);
                        break;
                }
            }

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceObject();
            }
                
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateObject(Vector3.up, -10, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateObject(Vector3.up, 10, Space.World);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
                var rotateAmount = mouseScroll > 0 ? 1 : -1; 
                RotateObject(Vector3.up, 10 * rotateAmount, Space.World);
            }

            if (Input.GetKey(KeyCode.W))
            {
                ChangeObjectHeight(2 * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                ChangeObjectHeight(-2 * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                switch (currentObjectType.objectType)
                {
                    case ObjectsType.Slant when currentObjectType.rotateStateIndex >= 0:
                        currentObjectType.rotateStateIndex--;
                        RotateObject(Vector3.forward, -20f, Space.Self);
                        break;
                    case ObjectsType.Gate when currentObjectType.heightStateIndex <= 0:
                        currentObjectType.heightStateIndex++;
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                switch (currentObjectType.objectType)
                {
                    case ObjectsType.Slant when currentObjectType.rotateStateIndex <= 0:
                        currentObjectType.rotateStateIndex++;
                        RotateObject(Vector3.forward, 20f, Space.Self);
                        break;
                    case ObjectsType.Gate when currentObjectType.heightStateIndex >= 0:
                        currentObjectType.heightStateIndex--;
                        break;
                }
            }
        }

        public void PlaceObject()
        {
            try
            {
                ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer("TrackGround"));
                currentObjectType = null;
                pendingObject = null;
            }
            catch
            {
                // ignored
            }
        }

        private void ChangeObjectHeight(float value)
        {
            if(currentObjectType == null)
                return;

            if (currentObjectType.objectType == ObjectsType.Gate)
            {
                currentObjectType.yOffset += value;
                // _mainCamera.transform.Translate(0, value, 0, Space.Self);
            }
        }
        
        public void ChangeLayerRecursively(Transform obj, int layer)
        {
            if (LayerMask.LayerToName(obj.gameObject.layer) != "FloorConnection" && LayerMask.LayerToName(obj.gameObject.layer) != "WallConnection" && LayerMask.LayerToName(obj.gameObject.layer) != "SlantConnection")
            {
                obj.gameObject.layer = layer;
            }

            foreach (Transform child in obj)
            {
                ChangeLayerRecursively(child, layer);
            }
        }

        public void OffOutlineRecursively(Transform obj)
        {
            if(obj.gameObject.GetComponent<Outline>())
                obj.gameObject.GetComponent<Outline>().enabled = false;

            foreach (Transform child in obj)
            {
                OffOutlineRecursively(child);
            }
        }

        
        private void RotateObject(Vector3 axis, float rotateAmount, Space space)
        {
            if(pendingObject == null)
                return;
            pendingObject.transform.Rotate(axis, rotateAmount, space);
        }
        
        public void SelectObject(int index)
        {
            pendingObject = Instantiate(objects[index], mousePos, transform.rotation);
            SceneManager.MoveGameObjectToScene(pendingObject, levelScene);
            objectsPool.Add(pendingObject);
            _selection.Deselect();
            _selection.Select(pendingObject);
            currentObjectType = pendingObject.GetComponent<TrackObject>();
            currentObjectType.isActive = true;
        }

        private void CreateObjectsPoolScene()
        {
            levelScene = SceneManager.CreateScene("ObjectsPool");
            MoveObjectsToPoolScene();
        }

        private void MoveObjectsToPoolScene()
        {
            foreach (var obj in objectsPool)
            {
                SceneManager.MoveGameObjectToScene(obj, levelScene);
            }
        }

        public void LoadScene()
        {
            Dictionary<string, Dictionary<string, string>> loadedData =
                new Dictionary<string, Dictionary<string, string>>();
            
            if(objectsPool.Count > 0)
                ClearObject();
            
            string jsonData = File.ReadAllText(Application.dataPath + "/Levels/" + levelName + ".json");
            loadedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in loadedData)
            {
                string objectName = kvp.Value["name"].Substring(0, kvp.Value["name"].IndexOf('('));
                Vector3 position = ParseVector3(kvp.Value["position"]);
                Vector3 rotation = ParseVector3(kvp.Value["rotation"]);
                Vector3 scale = ParseVector3(kvp.Value["scale"]);
                int layer = Convert.ToInt32(kvp.Value["layer"]);
                GameObject newObj = Instantiate(Resources.Load<GameObject>("TrackObjects/" + objectName), position, Quaternion.Euler(rotation));
                ChangeLayerRecursively(newObj.transform, layer);
                OffOutlineRecursively(newObj.transform);
                newObj.transform.localScale = scale;
                newObj.name = kvp.Value["name"];
                objectsPool.Add(newObj);
            }
        }

        private Vector3 ParseVector3(string str)
        {
            string[] values = str.Split(' ');
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);
            float z = float.Parse(values[2]);
            return new Vector3(x, y, z);
        }
        
        private void ClearObject()
        {
            foreach (var obj in objectsPool)
            {
                Destroy(obj);
            }
            objectsPool.Clear();
        }
        
        private float RoundToNearsGrid(float pos)
        {
            float xDiff = pos % gridSize;
            pos -= xDiff;

            if (xDiff > (gridSize / 2))
            {
                pos += gridSize;
            }

            return pos;
        }
    }
}