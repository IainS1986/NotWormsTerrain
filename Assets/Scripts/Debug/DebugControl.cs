using UnityEngine;
using System.Collections;

namespace Terrain.Debugging
{
    /// <summary>
    /// Debug class that pans the camera with RMB click and drag
    /// and zooms in and out with the mouse wheel.
    ///
    /// There is also (mostly untested) iOS and Android touch controls with
    /// one finger drag and 2 fingers pinch zoom
    /// </summary>
    public class DebugControl : MonoBehaviour
    {
        /// <summary>
        /// Current input screen position
        /// </summary>
        Vector3 mouseScreen = new Vector3(-1,-1,-1);

        /// <summary>
        /// Z Axis amount used for clamping position when dragging
        /// </summary>
        float z = -1;

        /// <summary>
        /// bool value to denote if the control is current zooming in or out
        /// True if zooming, otherwise false
        /// </summary>
        bool zooming = false;

        /// <summary>
        /// Last distance value last frame used to monitor the difference in pinch
        /// amount. Used by touch controls only.
        /// </summary>
        float lastDistance = 0;

        /// <summary>
        /// The rate at which the object moves in the Z access based on the
        /// scroll wheel or pinch zoom rate. The higher the value the more
        /// the object will move.
        /// </summary>
        [SerializeField]
        private float m_zoomSpeed = 20;
    
        /// <summary>
        /// Unity function called once per frame for Update.
        /// Will handle input and drag the attached gameobject based on
        /// user input and moves the gameobject in the Z depth axis
        /// based on scroll wheel or pinch zoom value
        /// </summary>
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
    
        /// <summary>
        /// Helper function to check if the input position is on the screen
        /// </summary>
        /// <returns>True is the input position is on the screen, otherwise false</returns>
        private bool MouseOnScreen(){
            return Input.mousePosition.x>=0 && Input.mousePosition.x<=Screen.width &&
                Input.mousePosition.y>=0 && Input.mousePosition.y<=Screen.height;
        }
    }
}

