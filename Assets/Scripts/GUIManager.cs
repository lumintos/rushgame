using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	[HideInInspector]
	public float screenWidth = 0.0f;
	[HideInInspector]
	public float screenHeight = 0.0f;
	
	public GUITexture Tex_TurnLeft;
	public GUITexture Tex_TurnRight;
	public GUITexture Tex_Jump;
	public GUITexture Tex_HPLeft;
	public GUITexture Tex_HPRight;
	public GUITexture Tex_PlayerIconL;
	public GUITexture Tex_PlayerIconR;
    public GUITexture Tex_StoneStatus;
	public GUIText Text_Timer;
    public GUIText Text_GameResult;
	
	private float inputGUI_h = 0.0f;
	private float inputGUI_v = 0.0f;
	private float speed_v = 1.0f;
	private float speed_h = 1.0f;
	private int HPLeft = 0;
	private int HPRight = 0;
	
	private int MaxHP = 0;
	private string[] HP_Textures = new string[]{"UI/HP_07","UI/HP_06","UI/HP_05","UI/HP_04","UI/HP_03","UI/HP_02","UI/HP_01","UI/HP",};
	
	private Size ScreenSizeUnit = new Size(750,422);
	private Size HPSize = new Size(200,37);
	private Size BtnTurnLR = new Size(86,86);
	private Size BtnJump = new Size(128,128);
	private Size PlayerIconSize = new Size(90,90);
    private Size StoneStatusSize = new Size(94, 94);
	
	public int fontSizeUnit = 1;

    public void ChangeStoneStatusTexture(bool isStoneTaken, NetworkPlayer stoneKeeper)
    {
        if (isStoneTaken)
        {
            if (Network.player == stoneKeeper)
            {
                Tex_StoneStatus.texture = (Texture2D)Resources.Load("UI/btn-timer-good");
            }
            else
            {
                Tex_StoneStatus.texture = (Texture2D)Resources.Load("UI/btn-timer-bad");
            }
        }
        else
        {
            Tex_StoneStatus.texture = (Texture2D)Resources.Load("UI/btn-timer-normal");
        }
    }

	public void UpdateGUIElementsSize(Size p_ScreenSize)
	{
		float scaleWidth = (float)p_ScreenSize.Width/(float)ScreenSizeUnit.Width;
		float scaleHeight = (float)p_ScreenSize.Height/(float)ScreenSizeUnit.Height;
		float widthRatio = 0,heightRatio = 0;
		float heightRatioIconR =0, heightRatioIconL = 0;
		// Scaling Control according to screen width&height
		Tex_TurnLeft.pixelInset = new Rect(Tex_TurnLeft.pixelInset.x,Tex_TurnLeft.pixelInset.y,
		                                   (float) BtnTurnLR.Width * scaleWidth, (float) BtnTurnLR.Height * scaleHeight);
		Tex_TurnRight.pixelInset = new Rect(Tex_TurnRight.pixelInset.x,Tex_TurnRight.pixelInset.y,
		                                    (float) BtnTurnLR.Width * scaleWidth,(float) BtnTurnLR.Height * scaleHeight);
		Tex_Jump.pixelInset = new Rect(Tex_Jump.pixelInset.x,Tex_Jump.pixelInset.y,
		                               (float) BtnJump.Width * scaleWidth,(float) BtnJump.Height * scaleHeight);
		Tex_PlayerIconL.pixelInset = new Rect(Tex_PlayerIconL.pixelInset.x,Tex_PlayerIconL.pixelInset.y,
		                                      (float)PlayerIconSize.Width*scaleWidth, (float)PlayerIconSize.Height*scaleHeight);
		Tex_PlayerIconR.pixelInset = new Rect(Tex_PlayerIconR.pixelInset.x,Tex_PlayerIconR.pixelInset.y,
		                                      (float)PlayerIconSize.Width*scaleWidth, (float)PlayerIconSize.Height*scaleHeight);
		Tex_HPLeft.pixelInset = new Rect(Tex_HPLeft.pixelInset.x,Tex_HPLeft.pixelInset.y,
		                                 (float)HPSize.Width*scaleWidth, (float)HPSize.Height*scaleHeight);
		Tex_HPRight.pixelInset = new Rect(Tex_HPRight.pixelInset.x,Tex_HPRight.pixelInset.y,
		                                  (float)HPSize.Width*scaleWidth, (float)HPSize.Height*scaleHeight);
        Tex_StoneStatus.pixelInset = new Rect(Tex_StoneStatus.pixelInset.x, Tex_StoneStatus.pixelInset.y,
                                           StoneStatusSize.Width* scaleWidth, StoneStatusSize.Height * scaleHeight);

		// Positioning Controls 
		widthRatio = (float)Tex_TurnLeft.pixelInset.width/p_ScreenSize.Width;
		Tex_TurnRight.transform.position = new Vector3(Tex_TurnLeft.transform.position.x+widthRatio, Tex_TurnRight.transform.position.y,0.0f);
		
		widthRatio = (float)Tex_Jump.pixelInset.width/p_ScreenSize.Width;
		Tex_Jump.transform.position = new Vector3(1-widthRatio,Tex_Jump.transform.position.y,0.0f);
		
		heightRatioIconL = heightRatio = (float) Tex_PlayerIconL.pixelInset.height/p_ScreenSize.Height;
		Tex_PlayerIconL.transform.position = new Vector3(Tex_PlayerIconL.transform.position.x,1-heightRatio,0.0f);

		heightRatioIconR = heightRatio = (float) Tex_PlayerIconR.pixelInset.height/p_ScreenSize.Height;
		widthRatio = (float) Tex_PlayerIconR.pixelInset.width/p_ScreenSize.Width;
		Tex_PlayerIconR.transform.position = new Vector3(1-widthRatio,1-heightRatio,0.0f);
		
		heightRatio = (float) Tex_HPLeft.pixelInset.height/p_ScreenSize.Height;
		widthRatio = (float) Tex_PlayerIconL.pixelInset.width/p_ScreenSize.Width;
		Tex_HPLeft.transform.position = new Vector3(Tex_PlayerIconL.transform.position.x + widthRatio,1-heightRatio-heightRatioIconL/4.386f,0.0f);
		
		heightRatio = (float) Tex_HPRight.pixelInset.height/p_ScreenSize.Height;
		widthRatio = (float) Tex_HPRight.pixelInset.width/p_ScreenSize.Width;
		Tex_HPRight.transform.position = new Vector3(Tex_PlayerIconR.transform.position.x-widthRatio,1-heightRatio-heightRatioIconR/4.386f,0.0f);

		//Text_Timer.transform.position = new Vector3(0.5f-0.01f,1.0f-0.03f,0.0f);
        Text_Timer.fontSize = (int) (20 * scaleHeight);
	}
	
	public float GetInputGUI_h()
	{
		return inputGUI_h;
	}
	
	public float GetInputGUI_v()
	{
		return inputGUI_v;
	}
	
	public void UpdateTouchInput()
	{
		if(Input.touchCount==0)
		{
			inputGUI_h = 0.0f;
			inputGUI_v = 0.0f;
			Tex_TurnLeft.texture = Resources.Load("UI/btn-left-normal") as Texture2D;
			Tex_TurnRight.texture = Resources.Load("UI/btn-right-normal") as Texture2D;
			Tex_Jump.texture = Resources.Load("UI/btn-jump-normal") as Texture2D;
		}
		
		for(int i=0;i<Input.touchCount;i++)
		{
			Touch current = Input.GetTouch(i);
			
			if(Tex_Jump.HitTest(current.position))
			{
                Debug.Log("Double");
				inputGUI_v = speed_v;
				Tex_Jump.texture = Resources.Load("UI/btn-jump-clicked") as Texture2D;
			}
			else
			{
				inputGUI_v = 0.0f;
				Tex_Jump.texture = Resources.Load("UI/btn-jump-normal") as Texture2D;
			}
			
			if(Tex_TurnLeft.HitTest(current.position))
			{
				inputGUI_h = -speed_h;
				Tex_TurnLeft.texture = Resources.Load("UI/btn-left-clicked") as Texture2D;
			}
			else
			{
				Tex_TurnLeft.texture = Resources.Load("UI/btn-left-normal") as Texture2D;
			}
			
			if(Tex_TurnRight.HitTest(current.position))
			{
				inputGUI_h = speed_h;	
				Tex_TurnRight.texture = Resources.Load("UI/btn-right-clicked") as Texture2D;
			}
			else
			{
				Tex_TurnRight.texture = Resources.Load("UI/btn-right-normal") as Texture2D;
			}
			
			if(current.phase == TouchPhase.Ended && Tex_HPLeft.HitTest(current.position))
			{
				UpdateHP(--HPLeft,-1);
			}
			
			if(current.phase == TouchPhase.Ended && Tex_HPRight.HitTest(current.position))
			{
				UpdateHP(++HPRight,-1);
			}
			
		}
	}
	
	public void SetMaxHP(int p_MaxHP)
	{
		MaxHP = p_MaxHP;
	}
	
	public void UpdateHP(int p_HP,int side)
	{
		int temp = p_HP%(MaxHP+1);
		if(side<0)
		{
			HPLeft = temp;
			Tex_HPLeft.texture = Resources.Load(HP_Textures[HPLeft]) as Texture2D;
		}
		else
		{
			HPRight = temp;
			Tex_HPRight.texture = Resources.Load(HP_Textures[HPRight]) as Texture2D;
		}
	}

}
