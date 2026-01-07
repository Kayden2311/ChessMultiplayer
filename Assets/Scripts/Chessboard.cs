using System;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [Header("Graphics Settings")]
    [SerializeField] private Material tileMaterial;
    
    //Logic fields
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;

    private void Awake()
    {
        GenerataAllFiles(1, TILE_COUNT_X, TILE_COUNT_Y);
    }

    private void Update()
    {
        if(!currentCamera)
        {
            currentCamera = Camera.current;
            return;
        }
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")))
        {
            //Get indexes off tiles being hit
            Vector2Int hitPosition = LookupTileIndex(info.collider.gameObject);
            
            //Display hover effect after not hovering any tiles
            if(currentHover == -Vector2Int.one)
            {
                //First time hovering
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            //Display hover effect after hitting a tiles
            if (currentHover != hitPosition)
            {
                //First time hovering
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
        }
        else
        {
            //If hovering out of bound reset hover effect
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
    }

    //Generate board
    private void GenerataAllFiles(float tileSize, int tileCountX, int tileCountY)
    {
        tiles = new GameObject[tileCountX, tileCountY];
        //Nested loops to generate tiles
        for (int x= 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X: {0}, Y:{1}", x, y));
        tileObject.transform.parent = this.transform;
        
        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        //Add materials on creation 
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        //Define vertices
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, 0, y * tileSize);
        vertices[1] = new Vector3(x * tileSize, 0, (y + 1) * tileSize);
        vertices[2] = new Vector3((x + 1) * tileSize, 0, y * tileSize);
        vertices[3] = new Vector3((x + 1) * tileSize, 0, (y + 1) * tileSize);

        
        int[] tris = new int[] {0, 1, 2, 1, 3, 2 };
        
        mesh.vertices = vertices;
        mesh.triangles = tris;
        
        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();
        return tileObject;
    }

    //Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        //Vectoring through all tiles to find the hit one
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
    }

}
