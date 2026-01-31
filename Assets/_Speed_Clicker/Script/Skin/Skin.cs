using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    public CharacterType CharacterType;
    public Animator _animator;
    public RuntimeAnimatorController _controller;
    public void OnValidate()
    {
        if (_animator == null) 
        {
            _animator = GetComponentInChildren<Animator>(true);
            if (_animator == null)
            {
                _animator = transform.GetChild(0).gameObject.AddComponent<Animator>();

            }
        }
        if(_animator.runtimeAnimatorController == null && _controller != null) 
        {
            _animator.runtimeAnimatorController = _controller;
        }

    }

}
