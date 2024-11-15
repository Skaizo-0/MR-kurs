using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public List<RectTransform> buttons;
    public List<GameObject> prefabs;
    public Transform spawn;
    private bool pressed;

    public Rigidbody newIsSnappedValue;


    // Start is called before the first frame update
    void Start()
    {

    }
    private void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    UnityEngine.Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (buttons[0].localScale.x != 1 && pressed == false)
        {
            pressed = true;
            GameObject instance = Instantiate(prefabs[0], spawn.position, Quaternion.identity);
            instance.SetActive(true);
            StartCoroutine(Reset());
        }
        if (buttons[1].localScale.x != 1 && pressed == false)
        {
            pressed = true;
            GameObject instance = Instantiate(prefabs[1], spawn.position, Quaternion.identity);
            instance.SetActive(true);
            StartCoroutine(Reset());
        }
        if (buttons[2].localScale.x != 1 && pressed == false)
        {
            pressed = true;
            
            Exit();
                            
        }
     

    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(1);
        pressed = false;
    }

}