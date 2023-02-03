using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLockManager : MonoSingleton<InputLockManager>
{
    [SerializeField] GameObject mLockPanel;
    private int mLockTimes = 0;
    public void Lock()
    {
        ++mLockTimes;
        UpdateLockState();
    }

    public void UnLock()
    {
        --mLockTimes;
        UpdateLockState();
    }

    protected override void Awake()
    {
        base.Awake();
        UpdateLockState();
    }

    void UpdateLockState()
    {
        mLockPanel.SetActive(mLockTimes > 0);
    }
}
