using UnityEngine;
using UnityEngine.InputSystem;

public class Chessboard : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Material whiteMat;
    [SerializeField] private Material blackMat;
    [SerializeField] private Material hoverMat;
    [SerializeField] private float cameraHeight = 7f;
    [SerializeField] private float cameraDistance = 6f;


    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;

    private GameObject[,] tiles;
    private Material[,] originalMats;

    private Camera currentCamera;
    private Vector2Int currentHover = -Vector2Int.one;

    private void Awake()
    {
        GenerateAllTiles(1);
        PositionCamera();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        Ray ray = currentCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("Tile")))
        {
            Vector2Int hitPos = LookupTileIndex(hit.transform.gameObject);

            if (currentHover != hitPos)
            {
                ClearHover();
                currentHover = hitPos;
                SetHover(hitPos);
            }
        }
        else
        {
            ClearHover();
        }
    }

    // -------- Hover --------
    private void SetHover(Vector2Int pos)
    {
        MeshRenderer mr = tiles[pos.x, pos.y].GetComponent<MeshRenderer>();
        mr.material = hoverMat;
    }

    private void ClearHover()
    {
        if (currentHover == -Vector2Int.one) return;

        MeshRenderer mr = tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>();
        mr.material = originalMats[currentHover.x, currentHover.y];
        currentHover = -Vector2Int.one;
    }

    // -------- Board Generation --------
    private void GenerateAllTiles(float tileSize)
    {
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        originalMats = new Material[TILE_COUNT_X, TILE_COUNT_Y];

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tile = Instantiate(tilePrefab, transform);
        tile.name = $"Tile {x},{y}";
        tile.transform.position = new Vector3(x * tileSize, 0, y * tileSize);

        bool isWhite = (x + y) % 2 == 0;
        Material baseMat = isWhite ? whiteMat : blackMat;

        MeshRenderer mr = tile.GetComponent<MeshRenderer>();
        mr.material = baseMat;
        originalMats[x, y] = baseMat;

        tile.layer = LayerMask.NameToLayer("Tile");
        return tile;
    }

    // -------- Utils --------
    private Vector2Int LookupTileIndex(GameObject hitTile)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitTile)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
    }
    // -------- Camera Positioning --------
    private void PositionCamera()
    {
        if (!Camera.main) return;

        Vector3 boardCenter = new Vector3(
            (TILE_COUNT_X - 1) * 0.5f,
            0,
            (TILE_COUNT_Y - 1) * 0.5f
        );

        Camera.main.transform.position = boardCenter
            + new Vector3(0, cameraHeight, -cameraDistance);

        Camera.main.transform.LookAt(boardCenter);
    }

}
