using NabaGame.Core.Runtime.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public PlayerController PlayerController;
    public PetManager PetManager;
    public override void Init()
    {
        PetManager.LoadPetBag();
    }
    private void Start()
    {
        PetManager.InitEquip();
    }
    public void Update()
    {
        
    }

}
