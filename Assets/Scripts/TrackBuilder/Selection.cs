﻿using System;
using UnityEngine;
using UnityEngine.UI;
using cakeslice;
using UnityEngine.EventSystems;
using Outline = cakeslice.Outline;

namespace Builder
{
    public class Selection : MonoBehaviour
    {
        public GameObject selectedObject;
        public LayerMask layerMask;
        private BuilderManager _builderManager;

        private void Start()
        {
            _builderManager = FindObjectOfType<BuilderManager>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10000, layerMask))
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("TrackGround"))
                        Select(hit.collider.transform.root.gameObject);
            }

            if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.KeypadEnter)) && selectedObject != null)
            {
                _builderManager.PlaceObject();
                Deselect();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Move()
        {
            if (selectedObject == null) return;
            _builderManager.ChangeLayerRecursively(selectedObject.transform.root.transform, LayerMask.NameToLayer("Track"));
            _builderManager.pendingObject = selectedObject.transform.root.gameObject;
            _builderManager.currentObjectType = selectedObject.GetComponentInParent<TrackObject>();
            _builderManager.currentObjectType.isActive = true;
        }

        public void Delete()
        {
            if (selectedObject == null) return;
            if (selectedObject.CompareTag("Player")) return;
            var obj = selectedObject;
            Deselect();
            _builderManager.objectsPool.Remove(obj.transform.root.gameObject);
            _builderManager.droneBuilderCheckNode.RemoveNode(obj.transform.root.transform);
            Destroy(obj.transform.root.gameObject);
        }

        public void Select(GameObject obj)
        {
            if (obj == selectedObject)
                return;
            
            if(selectedObject != null)
                Deselect();

            var outlines = obj.transform.root.GetComponentsInChildren<Outline>();
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }
            selectedObject = obj;
        }

        public void Deselect()
        {
            if (selectedObject == null)
                return;

            var outlines = selectedObject!.transform.root.GetComponentsInChildren<Outline>();
            foreach (var outline in outlines)
            {
                outline.enabled = false;
            }

            selectedObject.GetComponent<TrackObject>().isActive = false;
            selectedObject = null;
        }
    }
}