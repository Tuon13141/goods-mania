using Kit.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : Singleton<LevelController>
{
    [SerializeField] InputManager m_InputManager;
    public InputManager inputManager => m_InputManager; 

}
