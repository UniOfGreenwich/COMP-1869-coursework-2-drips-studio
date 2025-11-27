using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout gridLayout;
    private Grid grid;
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private TileBase tileBase;

    public GameObject prefab1;
    public GameObject prefab2;

    private PlaceableObject objectToPlace;

    private void Awake()
    {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update()
    {
        if(Input.touchCount == 3 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            InitialiseWithObject(prefab1);
        }
        else if (Input.touchCount == 4 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            InitialiseWithObject(prefab2);
        }

        if(!objectToPlace)
        {
            return;
        }

        if (Input.touchCount == 5 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if(CanBePlaced(objectToPlace))
            {
                objectToPlace.Place();
                Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                TakeArea(start, objectToPlace.Size);
            }
            else
            {
                Destroy(objectToPlace.gameObject);
            }
        }
        else if (Input.touchCount == 6 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Destroy(objectToPlace.gameObject);
        }
    }

    public static Vector3 GetTouchWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPosition);
        return position;
    }

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (Vector3Int v in area.allPositionsWithin)
        {
            Vector3Int position = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(position);
            counter++;
        }

        return array;
    }

    public void InitialiseWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);

        GameObject gameObject = Instantiate(prefab, position, Quaternion.identity);
        objectToPlace = gameObject.GetComponent<PlaceableObject>();
        gameObject.AddComponent<ObjectDrag>();
    }    

    private bool CanBePlaced(PlaceableObject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        area.size = placeableObject.Size;

        TileBase[] baseArray = GetTilesBlock(area, mainTilemap);

        foreach(TileBase tb in baseArray)
        {
            if (tb == tileBase)
            {
                return false;
            }
        }

        return true;
    }

    public void TakeArea(Vector3Int start, Vector3Int size)
    {
        mainTilemap.BoxFill(start, tileBase, start.x, start.y, start.x + size.x, start.y + size.y);
    }
}
