using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
	TileContent content;
    Tile north, east, south, west , nextOnPath;
    private int distance;
    public Tile NextTileOnPath => nextOnPath;
    public Vector3 ExitPoint { get; private set; }
	public bool HasPath => distance != int.MaxValue;
	public bool IsAlternative { get; set; }
	public TileContent Content {
		get => content;
		set {
			Debug.Assert(value != null, "Null assigned to content!");
			if (content != null) {
				content.Recycle();
			}
			content = value;
			content.transform.localPosition = transform.localPosition;
		}
	}
	public Direction PathDirection { get; private set; }
	static Quaternion
		northRotation = Quaternion.Euler(90f, 0f, 0f),
		eastRotation = Quaternion.Euler(90f, 90f, 0f),
		southRotation = Quaternion.Euler(90f, 180f, 0f),
		westRotation = Quaternion.Euler(90f, 270f, 0f);
	
	public Tile GrowPathNorth () => GrowPathTo(north, Direction.South);
	public Tile GrowPathEast () => GrowPathTo(east, Direction.West);
	public Tile GrowPathSouth () => GrowPathTo(south, Direction.North);
	public Tile GrowPathWest () => GrowPathTo(west, Direction.East);
    public static void MakeEastWestNeighbors (Tile east, Tile west) 
    {
		Debug.Assert(
			west.east == null && east.west == null, "Redefined neighbors!"
		);
		west.east = east;
		east.west = west;
    }
    public static void MakeNorthSouthNeighbors (Tile north, Tile south) 
    {
		Debug.Assert(
			south.north == null && north.south == null, "Redefined neighbors!"
		);
		south.north = north;
		north.south = south;
    }
    public void ClearPath () 
    {
	    distance = int.MaxValue;
	    nextOnPath = null;
    }
    public void BecomeDestination () 
    {
	    distance = 0;
	    nextOnPath = null;
	    ExitPoint = transform.localPosition;
    }
    Tile GrowPathTo (Tile neighbor, Direction direction) 
    {
	    if (!HasPath || neighbor == null || neighbor.HasPath) {
		    return null;
	    }
	    neighbor.distance = distance + 1;
	    neighbor.nextOnPath = this;
	    neighbor.ExitPoint = neighbor.transform.localPosition + DirectionExtensions.GetHalfVector(direction);
	    neighbor.PathDirection = direction;
	    
	    return neighbor.Content.BlocksPath ? null : neighbor;
    }
}
