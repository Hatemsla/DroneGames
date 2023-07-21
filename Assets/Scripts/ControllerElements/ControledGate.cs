using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Builder
{
    public class ControledGate : InteractiveObject
    {
        public GameObject gateObject;
        // public bool is_activ;
        private MeshRenderer mesh;
        private Collider col;
        private Renderer objectRenderer;

        public float glowIntensity = 1f;

        public enum ColorOption
        {
            Красный, 
            Зелёный,
            Жёлтый,
            Синий            
        }
        public ColorOption selectedColorOption;
        private bool isActiv;
        private bool previousIsActiv;

        // Start is called before the first frame update
        void Start()
        {
            objectRenderer = gateObject.GetComponent<Renderer>();
            SetColor(GetColorFromOption(selectedColorOption)); 
            mesh = gateObject.GetComponent<MeshRenderer>();
            col = gateObject.GetComponent<Collider>();
            if (!isActive)
            {
                DisableMeshAndCollider();
            }        
        }

        // Update is called once per frame
        void Update()
        {
            if (CheckColorActivChange(selectedColorOption))
            {
                Debug.Log("CheckColorActivChange");
                if (col.enabled)
                {
                    DisableMeshAndCollider();
                }
                else 
                {
                    EnableMeshAndCollider();
                }
            }        
        }

        private void DisableMeshAndCollider()
        {
            mesh.enabled = false;
            col.enabled = false;
        }

        private void EnableMeshAndCollider()
        {
            mesh.enabled = true;
            col.enabled = true;
        }

        private Color GetColorFromOption(ColorOption option)
        {
            switch (option)
            {
                case ColorOption.Красный:
                    return Color.red;
                case ColorOption.Зелёный:
                    return Color.green;
                case ColorOption.Жёлтый:
                    return Color.yellow;
                case ColorOption.Синий:
                    return Color.blue;
                default:
                    return Color.red;
            }
        }

        private void SetColor(Color newColor)
        {            
            objectRenderer.material.SetColor("_Color", newColor);
            objectRenderer.material.EnableKeyword("_EMISSION");
            objectRenderer.material.SetColor("_EmissionColor", newColor * glowIntensity);
                        
        }

        private bool CheckColorActivChange(ColorOption option)
        {
            if (option == ColorOption.Красный)
            {
                isActiv = BuilderManager.Instance.isActivRed;
            }
            else if (option == ColorOption.Зелёный)
            {
                isActiv = BuilderManager.Instance.isActivGreen;
                
            }
            else if (option == ColorOption.Жёлтый)
            {
                isActiv = BuilderManager.Instance.isActivYellow;
            }
            else if (option == ColorOption.Синий)
            {
                isActiv = BuilderManager.Instance.isActivBlue;
            }
            if (previousIsActiv != isActiv)
            {
                previousIsActiv = isActiv;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void set_activ()
        {
            isActive = !isActive;
            if (!isActive)
                {
                    DisableMeshAndCollider();
                }
                else 
                {
                    EnableMeshAndCollider();
                }
        }

        public void set_color_index(int value)
        {
            color_index = value;
            selectedColorOption = (ColorOption)value;
            SetColor(GetColorFromOption(selectedColorOption));
        }

        public override void SetActive(bool active)
        {
            
        }
    }
}