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
}
