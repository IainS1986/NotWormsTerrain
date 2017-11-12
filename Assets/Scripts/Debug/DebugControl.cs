using UnityEngine;
using System.Collections;

public class DebugControl : MonoBehaviour {
    
    Vector3 mouseScreen = new Vector3(-1,-1,-1);
    float z = -1;
    bool zooming = false;
    float lastDistance = 0;

    [SerializeField]
    private float m_zoomSpeed = 20;
    
    // Update is called once per frame
    void Update () {
        Vector3 v = gameObject.transform.position;      
        float prevz = gameObject.transform.position.z;
        v.x+=Input.GetAxis("Horizontal");
        v.y+=Input.GetAxis("Vertical");
        v.z+=Input.GetAxis("Mouse ScrollWheel")* m_zoomSpeed;
        z=gameObject.transform.position.z;

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
        if(Input.touchCount == 1 && !DebugGUI.SwallowTouch)
        {           
            float prevx = mouseScreen.x;
            float prevy = mouseScreen.y;
            mouseScreen = Input.touches[0].position;
            
            if(prevx!=-1){
                Vector3 temp1 = new Vector3(prevx, prevy, prevz);
                Vector3 temp2 = new Vector3(mouseScreen.x, mouseScreen.y, z);
                Vector3 world1 = Camera.main.ScreenToWorldPoint(temp1);
                Vector3 world2 = Camera.main.ScreenToWorldPoint(temp2);
                
                v.x+=(world2.x - world1.x);         
                v.y+=(world2.y - world1.y);
            }
            
            zooming = false;
            lastDistance = 0;
        }
        else if(Input.touchCount == 2 && !DebugGUI.SwallowTouch)
        {
            float distance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
            
            if(!zooming)
            {
                zooming = true;
            }
            else
            {
                //Shift Z
                v.z -= (lastDistance - distance) / m_zoomSpeed;
            }
            
            lastDistance=distance;
            mouseScreen.x=-1;
            mouseScreen.y=-1;
            mouseScreen.z=-1;
        }
        else
        {
            zooming = false;
            lastDistance=0;
            mouseScreen.x=-1;
            mouseScreen.y=-1;
            mouseScreen.z=-1;
        }
#else

        if (Input.GetMouseButton(1) && MouseOnScreen()){
            
            float prevx = mouseScreen.x;
            float prevy = mouseScreen.y;
            mouseScreen = Input.mousePosition;
            
            if(prevx!=-1){
                Vector3 temp1 = new Vector3(prevx, prevy, prevz);
                Vector3 temp2 = new Vector3(mouseScreen.x, mouseScreen.y, z);
                Vector3 world1 = Camera.main.ScreenToWorldPoint(temp1);
                Vector3 world2 = Camera.main.ScreenToWorldPoint(temp2);
                
                v.x+=(world2.x - world1.x);         
                v.y+=(world2.y - world1.y);
            }
            mouseScreen = Input.mousePosition;
        }
        else{
            mouseScreen.x=-1;
            mouseScreen.y=-1;
            mouseScreen.z=-1;
        }
        #endif  
        
        gameObject.transform.position = v;
    }
    
    private bool MouseOnScreen(){
        return Input.mousePosition.x>=0 && Input.mousePosition.x<=Screen.width &&
            Input.mousePosition.y>=0 && Input.mousePosition.y<=Screen.height;
    }
}
