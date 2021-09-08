using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphwayTest : MonoBehaviour 
{
    public const int MAX_SPEED = 5;
    public const int ACCELERATION = 1;
	public const float ROTATION_SPEED = 0.3f;

    [Tooltip("Enable Debug Mode to see algoritm in action slowed down. MAKE SURE GIZMOS ARE ENABLED!")]
    public bool debugMode = false;

    private GwWaypoint[] waypoints;
	private float speed = 0;
	public int steps = 0;
	PlayerStats player;

    private void Start()
    {
		player = gameObject.GetComponent<PlayerStats>();
    }

	void Update()
	{
		if (player.isLocal && player.moveAllowed)
		{       
			// Handle mouse click
			if (Input.GetMouseButtonDown(0) && steps > 0)
			{
				if (EventSystem.current.IsPointerOverGameObject())    // is the touch on the GUI
				{
					// GUI Action
				}
				else // Your raycast code
				{
					// Check if an object in the scene was targeted
					RaycastHit hit;

					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

					if (Physics.Raycast(ray, out hit))
					{
						// Check if ray hit the hex
						if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("hexes") && hit.collider.gameObject.layer != LayerMask.NameToLayer("UI"))
						{
							// Object hit
							// Use Graphway to try and find a path to hit position
							Graphway.FindPath(transform.position, hit.point, FindPathCallback, true, debugMode);
						}

					}
				}
				
			}
		
			// Move towards waypoints (if has waypoints)
			if (waypoints != null && waypoints.Length > 0 && steps >= waypoints.Length-1)
			{
				player.animationToggle(true);
				// Increase speed
				speed = Mathf.Lerp(speed, MAX_SPEED, Time.deltaTime * ACCELERATION);
				speed = Mathf.Clamp(speed, 0, MAX_SPEED);

				// Look at next waypoint 
				//transform.LookAt(waypoints[0].position);
				Vector3 TargetDir = waypoints[0].position - transform.position;
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(TargetDir), Time.time * ROTATION_SPEED);
				// Move toward next waypoint
				transform.position = Vector3.MoveTowards(transform.position, waypoints[0].position, Time.deltaTime * waypoints[0].speed * speed);
			
				// Check if reach waypoint target
				if (Vector3.Distance(transform.position, waypoints[0].position) < 0.1f)
				{
					// Move on to next waypoint
					NextWaypoint();
				}	
			}
			else
			{
				// Reset speed
				player.animationToggle(false);
				speed = 0;
			}
		
			// Draw Path
			if (debugMode && waypoints != null && waypoints.Length > 0)
			{
				Vector3 startingPoint = transform.position;
			
				for (int i = 0 ; i < waypoints.Length ; i++)
				{
					Debug.DrawLine(startingPoint, waypoints[i].position, Color.green);
				
					startingPoint = waypoints[i].position;
				}
			}
		}
	}
	
	private void NextWaypoint()
	{
		if (waypoints.Length > 1)
		{
			steps = steps - 1;
			GwWaypoint[] newWaypoints = new GwWaypoint[waypoints.Length - 1];
			
			for (int i = 1 ; i < waypoints.Length ; i++)
			{
				newWaypoints[i-1] = waypoints[i];
			}
			
			waypoints = newWaypoints;
		}
		else
		{
			waypoints = null;
		}  
	}
	
	private void FindPathCallback(GwWaypoint[] path)
	{
        // Graphway returns 'null' if no path found
        // OR GwWaypoint array of waypoints to destination

        if (path == null)
		{
			Debug.Log("Path to target position not found!");
		}
		else
		{
			waypoints = path;
		}
	}

	protected void LateUpdate()
	{
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
	}


	void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;

        //GUI.Label(new Rect(20, 20, 200, 20), "INSTRUCTIONS: Left click on a roadway node to find the quickest path to it.", style);
        //GUI.Label(new Rect(Screen.width - 260, 20, 200, 20), "Make sure GIZMOS are ENABLED! ^^^", style);
    }
}
