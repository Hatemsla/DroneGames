using System;
using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;

namespace Builder
{
    public class CheckPlacement : MonoBehaviour
    {
        private BuilderManager _builderManager;

        private void Start()
        {
            _builderManager = FindObjectOfType<BuilderManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("TrackGround"))
            {
                _builderManager.canPlace = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("TrackGround"))
            {
                _builderManager.canPlace = true;
            }
        }
    }
}