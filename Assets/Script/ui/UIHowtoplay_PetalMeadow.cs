using UnityEngine;

public class UIHowtoplay_PetalMeadow : UICanvas_PetalMeadow
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void back()
    {
        UIManager_PetalMeadow.Instance.EnableHowToPlay(false);
    }
}
