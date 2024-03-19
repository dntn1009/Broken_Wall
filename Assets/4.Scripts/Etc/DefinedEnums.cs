using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinedEnums
{
  
    public enum PlayerMotion
    {
        None = -1,
        Idle,
        Jump,
        Slash1,
        Slash2,
        Shield
    }
    public enum SceneState
    {
        Menu = 0,
        Play,
        Finish
    }

    public enum IngameChapter
    { 
       One,
       Two,
       Three
    }

}
