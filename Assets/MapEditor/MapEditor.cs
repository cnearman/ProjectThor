using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEditor;
using System;
using System.Collections.Generic;

public class MapEditor : MonoBehaviour {
    public Text gridText;
    public Text shapesText;
    public GameObject cam;

    public GameObject roomCore;

    public bool isGrid;
    public GameObject curs;

    Quaternion placeRot;
    

    public List<GameObject> pieces = new List<GameObject>();
    int currentP;

    GameObject selectedObject;

    int wallMask = 1 << 12;
    int wallMaskInvert = ~(1 << 12);

    // Use this for initialization
    void Start () {
        if (isGrid)
        {
            gridText.text = "GridLock: ON";
        }
        else
        {
            gridText.text = "GridLock: OFF";
        }

        placeRot = new Quaternion();
        placeRot = Quaternion.identity;

        currentP = 0;
        selectedObject = pieces[currentP];

        curs.GetComponent<MeshFilter>().mesh = selectedObject.GetComponent<MeshFilter>().sharedMesh;
        curs.transform.localScale = selectedObject.transform.localScale;
        curs.transform.rotation = placeRot;

        UpdateStrings();
    }
	
	// Update is called once per frame
	void Update () {
        
        if(Input.GetButtonDown("GridLock"))
        {
            isGrid = !isGrid;
            if(isGrid)
            {
                gridText.text = "GridLock: ON";
            } else
            {
                gridText.text = "GridLock: OFF";
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, wallMask))
        {
            Vector3 place;

            if(isGrid)
            {
                place = new Vector3(Mathf.Round(hit.point.x), 1f, Mathf.Round(hit.point.z)); // Mathf.Round(hit.point.y)
            } else
            {
                place = hit.point;
                place.y = 1f;
            }

            curs.transform.position = place;

            if (Input.GetButtonDown("Remove"))
            {
                RaycastHit hitDel;
                if (Physics.Raycast(new Vector3(place.x, 3f, place.z), Vector3.down, out hitDel, 10f, wallMaskInvert))
                {
                    Destroy(hitDel.collider.gameObject);
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                RaycastHit hitStuff;
                if(!Physics.Raycast(new Vector3(place.x, 3f, place.z), Vector3.down, out hitStuff, 10f, wallMaskInvert))
                {
                    Instantiate(selectedObject, place, placeRot, roomCore.transform);
                }
            }

            if(Input.GetButtonDown("Select Left"))
            {
                currentP -= 1;
                if(currentP < 0)
                {
                    currentP = pieces.Count - 1;
                }

                selectedObject = pieces[currentP];
                curs.GetComponent<MeshFilter>().mesh = selectedObject.GetComponent<MeshFilter>().sharedMesh;
                curs.transform.localScale = selectedObject.transform.localScale;

                UpdateStrings();
            }

            if (Input.GetButtonDown("Select Right"))
            {
                currentP += 1;
                if (currentP > pieces.Count - 1)
                {
                    currentP = 0;
                }

                selectedObject = pieces[currentP];
                curs.GetComponent<MeshFilter>().mesh = selectedObject.GetComponent<MeshFilter>().sharedMesh;
                curs.transform.localScale = selectedObject.transform.localScale;

                UpdateStrings();
            }

            if (Input.GetButtonDown("Rotate Left"))
            {
                placeRot.eulerAngles = new Vector3(placeRot.eulerAngles.x, placeRot.eulerAngles.y - 90f, placeRot.eulerAngles.z);
                curs.transform.rotation = placeRot;
            }

            if (Input.GetButtonDown("Rotate Right"))
            {
                placeRot.eulerAngles = new Vector3(placeRot.eulerAngles.x, placeRot.eulerAngles.y + 90f, placeRot.eulerAngles.z);
                curs.transform.rotation = placeRot;
            }

            float windowZoom = Input.GetAxis("Mouse ScrollWheel");
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + windowZoom * (-5f), cam.transform.position.z);
            
            if(Input.GetButton("Fire3"))
            {
                float windowMoveX = Input.GetAxis("Mouse X");
                float windowMoveZ = Input.GetAxis("Mouse Y");

                cam.transform.position = new Vector3(cam.transform.position.x - windowMoveX, cam.transform.position.y, cam.transform.position.z - windowMoveZ);
            }
            
            if(Input.GetButtonDown("Reset Cam"))
            {
                cam.transform.position = new Vector3(0f, 15f, 0f);
                cam.transform.eulerAngles = new Vector3(60f, 0f, 0f);
            }

            if(Input.GetButtonDown("Save Map"))
            {
                SaveMap();
            }
        }
    }

    public void UpdateStrings()
    {
        string tempText = "";

        for(int i = 0; i < pieces.Count; i++)
        {
            if(i  == currentP)
            {
                tempText += "[[ ";
            }
            tempText += pieces[i].name;
            if (i == currentP)
            {
                tempText += " ]]";
            }
            tempText += "   ";
        }

        shapesText.text = tempText;
    }

    public void SaveMap()
    {
        //PrefabUtility.CreatePrefab("Assets/EditorMaps/" + "MapDaMap " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".prefab", roomCore);
    }
}
