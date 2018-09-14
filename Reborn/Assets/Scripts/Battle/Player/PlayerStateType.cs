using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum PlayerStateType
    {
        None,

        Idle,       // 休闲状态
        Move,       // 移动状态
        Attack,     // 攻击状态
        Skill,      // 技能状态
        Frozen,     // 冰冻状态
        Dizziness,  //眩晕状态
        Sneer,      // 嘲讽状态

        Quantity
    }
}
