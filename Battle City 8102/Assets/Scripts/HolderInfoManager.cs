using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolderInfoManager : MonoBehaviour {

    public static HolderInfo holderInfo;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
