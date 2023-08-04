﻿using System;
using Drone;
using UnityEngine;

namespace Builder
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] public int coin = 1;

        private void OnTriggerEnter(Collider other)
        {
            if(!BuilderManager.Instance.isMove)
                return;
            
            var player = other.GetComponentInParent<DroneRpgController>();
            if (player)
            {
                EffectsManager.Intsance.GetGetEffect(transform.position);
                player.Coins += coin;
                if(BuilderManager.Instance.isGameMode)
                    Destroy(transform.root);
                else
                    transform.root.gameObject.SetActive(false);
            }
        }
    }
}