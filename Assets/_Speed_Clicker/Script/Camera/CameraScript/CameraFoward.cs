using NabaGame.Core.Runtime.TickManager;
using UnityEngine;

public class CameraFoward : TickableBehaviour
{
    private Camera main;
    //[SerializeField] private bool RegisCullingOnStart = true;
   
    private void Start()
    {
        main = Camera.main;
        //if (RegisCullingOnStart && cullingObject != null)
        //    GameController.Instance.frustumCullingManager.RegisterCullingObject(cullingObject);
    }

    public override void OnTickableUpdated(float dt)
    {
        if (main != null)
        {
            transform.forward = main.transform.forward;
        }
        else
        {
            main = Camera.main;
        }
    }
    //void Update()
    //{
    //    if (main != null)
    //    {
    //        transform.forward = main.transform.forward;
    //    }
    //    else
    //    {
    //        main = Camera.main;
    //    }
    //}
}
