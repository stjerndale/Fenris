using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    #region UI Events
    public event Action<List<SeedStack>> onStacksChanged;
    public void StacksChanged(List<SeedStack> stacks)
    {
        onStacksChanged?.Invoke(stacks);
    }

    public event Action<int> onNewSelection;
    public void NewSelection(int index)
    {
        onNewSelection?.Invoke(index);
    }

    public event Action<bool> onImmobilizePlayer;
    public void ImmobilizePlayer(bool immobility)
    {
        onImmobilizePlayer?.Invoke(immobility);
    }
    #endregion

    #region Quest Related Events
    public event Action<FlowerInformation> onFlowerPlanted;
    public void FlowerPlanted(FlowerInformation info)
    {
        onFlowerPlanted?.Invoke(info);
    }

    public event Action<FlowerInformation> onFlowerBloomed;
    public void FlowerBloomed(FlowerInformation info)
    {
        onFlowerBloomed?.Invoke(info);
    }
    #endregion
}
