using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayers : MonoBehaviour {

    PlayerInGame obj;
    ControllerPoll controllers;

    SpriteRenderer[] selectSprites = new SpriteRenderer[4];
    SpriteRenderer[] unselectSprites = new SpriteRenderer[4];


    // Use this for initialization
    void Start () {
        obj = GameObject.Find("PlayerVal").GetComponent<PlayerInGame>();
        controllers = FindObjectOfType<ControllerPoll>();

        for(int i = 0; i < 4; i++)
        {
            selectSprites[i] = GameObject.Find("Player" + (i + 1) + "Select").GetComponent<SpriteRenderer>();
            unselectSprites[i] = GameObject.Find("Player" + (i + 1) + "Unselect").GetComponent<SpriteRenderer>();
        }
    }
	
	// Update is called once per frame
	void Update () {
		//try to listen out for controllers
        for(int i = 0; i < 4; i++)
        {
            if(!controllers.Controllers[i])
            {
                obj.playerExists[i] = false;
            }

            if(obj.playerExists[i])
            {
                selectSprites[i].enabled = true;
                unselectSprites[i].enabled = false;
            }
            else
            {
                selectSprites[i].enabled = false;
                unselectSprites[i].enabled = true;
            }
        }
	}

    public void ControllerOne(variableData _var)
    {
        if(_var.state.controllerVariables[0] == button.pressed)
        {
            obj.playerExists[0] = true;
        }
        else if (_var.state.controllerVariables[1] == button.pressed)
        {
            obj.playerExists[0] = false;
        }
    }

    public void ControllerTwo(variableData _var)
    {
        if (_var.state.controllerVariables[0] == button.pressed)
        {
            obj.playerExists[1] = true;
        }
        else if (_var.state.controllerVariables[1] == button.pressed)
        {
            obj.playerExists[1] = false;
        }
    }

    public void ControllerThree(variableData _var)
    {
        if (_var.state.controllerVariables[0] == button.pressed)
        {
            obj.playerExists[2] = true;
        }
        else if (_var.state.controllerVariables[1] == button.pressed)
        {
            obj.playerExists[2] = false;
        }
    }

    public void ControllerFour(variableData _var)
    {
        if (_var.state.controllerVariables[0] == button.pressed)
        {
            obj.playerExists[3] = true;
        }
        else if (_var.state.controllerVariables[1] == button.pressed)
        {
            obj.playerExists[3] = false;
        }
    }
}
