using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Attached to Camera
/// Checks if the player is behind a wall or not with a raycast
/// </summary>
public class CutoutTracker : MonoBehaviour
{
    public static int PositionID = Shader.PropertyToID("_ScreenPosition");
    public static int SizeID = Shader.PropertyToID("_CutoutSize");

    [Header("Shader Settings")]
    public float BaseSize = 0.8f;//How big the circle should be at default camera distance

    [Range(0f, 1f)]
    public float Smoothness;
    [Range(0f, 1f)]
    public float Opacity;
    public float NoiseScale;



    [Header("Component References/Values")]
    public LayerMask Mask;

    private Camera Camera;
    [SerializeField]private Transform Player; // Tracks the Player

    public Vector3 PlayerOffset; // Allows adjustments of where the middle of the player is

    //Shrinks and expands the size of the circle for coroutine
    private float SizeMultiplier = 0f;

    [Header("Editor")]
    public bool ShowInEditor;//Lets the designers see the circle in the editor for testing

    //Should Get Camera and Player position
    private void Start()
    {
        //Gets the Camera and Player
        Camera = this.GetComponent<Camera>();
        ///GAMEMANAGER WASNT UPDATED/MERGED YET
        ///Player = GameManager.Instance.GetPlayer(); 
    }
    //Hides the circle in editor after exiting
    void OnApplicationQuit()
    {
        //WallMaterial.SetFloat(SizeID, 0);
        Shader.SetGlobalFloat(SizeID, 0f);
    }
    void Update()
    {
        Vector3 CameraPos = GetCameraPosition();
        Vector3 PlayerPos = GetPlayerOffset();
        Vector3 dir = CameraPos - PlayerPos;
        Ray ray = new Ray(PlayerPos, dir.normalized);

        bool hitWall = Physics.Raycast(ray, 3000, Mask);

        //If raycast hits any layers in LayerMask it sets WallMaterials Variables to be large enough to be visible
        if (hitWall && SizeMultiplier < 1f) //If hit and not running anything then do so
        {
            SizeMultiplier += 1.25f * Time.deltaTime; //How fast the cutout opens up
            Shader.SetGlobalFloat(SizeID, BaseSize * SizeMultiplier);
        }
        else if (!hitWall && SizeMultiplier > 0f)
        {
            SizeMultiplier -= 1.25f * Time.deltaTime; //How fast the cutout closes up
            Shader.SetGlobalFloat(SizeID, BaseSize * SizeMultiplier);
        }

        //Gets the players position in the world from the screens position
        Vector3 view = Camera.WorldToViewportPoint(PlayerPos);

        Shader.SetGlobalVector(PositionID, view);
    }
    //Min Recommended size is 1.8
    //Furthest Cam is recommended size 0.7
    public void SetPlayer(Transform player)
    {
        Player = player;
    }
    public Vector3 GetCameraPosition()
    {
        //Returns camera pos if Camera does exist and this if no camera found
        return  Camera == null ? this.transform.position : Camera.transform.position;
    }
    public Vector3 GetPlayerOffset()
    {
        return Player.transform.position + PlayerOffset;
    }
}
///Potential Quality of life improvents for performance
///GetDistance(Player,Camera)
///     Getting distance between the two could calculate the size of the circle keeping it the same size for the player view
///     Performance: GetDistance Every frame is CPU Intensive <summary>
/// Potential Quality of life improvents for performance



///An editor for the cutout system to allow developers to edit in editor
[CustomEditor(typeof(CutoutTracker))]
public class CutoutTrackerEditor : Editor
{
    private void OnSceneGUI()
    {
        CutoutTracker cutout = (CutoutTracker)target;

        Shader.SetGlobalFloat("_CutoutSmoothness", cutout.Smoothness);
        Shader.SetGlobalFloat("_CutoutOpacity", cutout.Opacity);
        Shader.SetGlobalFloat("_CutoutNoiseScale", cutout.NoiseScale);

        Handles.color = Color.green;
        Handles.DrawLine(cutout.GetCameraPosition(), cutout.GetPlayerOffset());
        //Prevents changing stuff in game and editor forgets to disable ShowInEditor
        //for anything below
        if (Application.isPlaying) { return; }


        if (cutout.ShowInEditor)
        {
            Shader.SetGlobalFloat("_CutoutSize", cutout.BaseSize);
        } else
        {
            Shader.SetGlobalFloat("_CutoutSize", 0f);
        }
    }

}