using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jam : Collectable
{
    [SerializeField] private JamData _jamData;

    protected override void Collect(GameObject player)
    {
        var _playerHealth = player.GetComponent<PlayerHealth>();
        var _inventory = player.GetComponent<PlayerInventory>();

        if (_playerHealth.health < _playerHealth.maxHealth)
        {
            _playerHealth.AddHealth(_jamData.healthPoints);
        }
        else
        {
            _inventory.AddJam(_jamData);
        }
    }
}
