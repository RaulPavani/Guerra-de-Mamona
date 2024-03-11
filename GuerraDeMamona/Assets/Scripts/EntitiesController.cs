using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class EntitiesController : Singleton<EntitiesController>
{
    [Header("Tilemap")]
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap interactiveMap;
    [SerializeField] private Tilemap baseMap;
    [SerializeField] private Tile hoverTile;
    [SerializeField] private Tile selectedTile;

    private List<Vector2Int> currentRangeTiles = new List<Vector2Int>();

    private Vector3 mousePosition;
    private Vector3Int tileMousePos;
    private Vector3Int previousTileMousePos;
    private Vector3Int selectedTilePos;

    [Header("Entities")]
    [SerializeField] private LayerMask entitiesLayer;
    private EntityBase currentHighlightedEntity;
    private EntityBase currentSelectedEntity;
    private EntityBase currentSelectedEnemy;

    private List<PlayerOneEntity> playerOneEntities = new List<PlayerOneEntity>();
    private List<PlayerTwoEntity> playerTwoEntities = new List<PlayerTwoEntity>();

    public event Action OnAllEntitiesDieEvent;

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        TileMapInteract();
        EntityInteract();
    }

    private void TileMapInteract()
    {
        tileMousePos = grid.WorldToCell(mousePosition);

        //Show tile indicator on mouse pos
        if (!tileMousePos.Equals(previousTileMousePos))
        {
            interactiveMap.SetTile(previousTileMousePos, null);
            interactiveMap.SetTile(tileMousePos, hoverTile);
            previousTileMousePos = tileMousePos;
        }

        //Clicking on a tile with a selected character -> creates a move action
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && currentSelectedEntity != null)
        {
            if (currentHighlightedEntity == null && currentRangeTiles.Contains((Vector2Int)tileMousePos))
            {
                selectedTilePos = tileMousePos;
                interactiveMap.SetTile(selectedTilePos, selectedTile);

                Vector3 destinationPos = grid.CellToWorld(tileMousePos + Vector3Int.one);
                destinationPos.z = 0;
                ClearRangeTiles();

                CommandManager.Instance.EnqueueCommand(new MoveCommand(currentSelectedEntity, currentSelectedEntity.transform.position, destinationPos));
                currentSelectedEntity.DeselectEntity();
                currentSelectedEntity = null;
            }
        }
    }

    public Vector3Int GetTileCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    public void ShowSelectedTile(Vector3Int? tileCell, bool tileActive = true)
    {
        interactiveMap.SetTile((Vector3Int)tileCell, tileActive ? selectedTile : null);
    }

    private void EntityInteract()
    {
        HandleHighlightEntities();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleSelectionEntities();
        }
    }

    private void HandleHighlightEntities()
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 100f, entitiesLayer);

        if (hit.collider != null)
        {
            currentHighlightedEntity = hit.transform.GetComponent<EntityBase>();

            //Highlight character to select
            if (currentSelectedEntity == null && currentHighlightedEntity.CanSelect)
            {
                currentHighlightedEntity.Highlight();
            }

            //With selected character, highlight enemy
            if (currentSelectedEntity != null && !currentHighlightedEntity.CanSelect)
            {
                currentHighlightedEntity.Highlight();
            }
        }
        else
        {
            //Remove last highlightd character
            currentHighlightedEntity?.RemoveHighlight();
            currentHighlightedEntity = null;
        }
    }

    private void HandleSelectionEntities()
    {
        if (currentHighlightedEntity != null && currentSelectedEntity == null && currentHighlightedEntity.CanSelect)
        {
            ClearRangeTiles();
            SelectEntity(currentHighlightedEntity);
            ShowEntityActionRange(currentHighlightedEntity.transform.position, currentHighlightedEntity.GetRange());
        }

        if (currentSelectedEntity != null && currentSelectedEntity != currentHighlightedEntity)
        {
            if (currentHighlightedEntity != null)
            {
                Vector2Int enemyPosInGrid = (Vector2Int)grid.WorldToCell(currentHighlightedEntity.transform.position);
                enemyPosInGrid -= Vector2Int.one;

                if (currentRangeTiles.Contains(enemyPosInGrid) && !currentHighlightedEntity.CanSelect)
                {
                    ClearRangeTiles();
                    AttackSelectedEnemy();
                }
            }
        }
    }

    private void SelectEntity(EntityBase entity)
    {
        currentSelectedEntity = entity;
        currentSelectedEntity.SelectEntity();
    }

    private void AttackSelectedEnemy()
    {
        currentSelectedEnemy = currentHighlightedEntity;
        CommandManager.Instance.EnqueueCommand(new AttackCommand(currentSelectedEntity, currentSelectedEnemy));
        currentSelectedEntity.DeselectEntity();

        currentSelectedEnemy = null;
        currentSelectedEntity = null;
    }

    public void AddEntityToList<T>(T entity)
    {
        if (entity is PlayerOneEntity)
        {
            playerOneEntities.Add(entity as PlayerOneEntity);
        }

        if (entity is PlayerTwoEntity)
        {
            playerTwoEntities.Add(entity as PlayerTwoEntity);
        }
    }

    public void RemoveEntityFromList<T>(T entity)
    {
        if (entity is PlayerOneEntity)
        {
            playerOneEntities.Remove(entity as PlayerOneEntity);
        }

        if (entity is PlayerTwoEntity)
        {
            playerTwoEntities.Remove(entity as PlayerTwoEntity);
        }

        if (playerTwoEntities.Count == 0 || playerOneEntities.Count == 0)
        {
            OnAllEntitiesDieEvent?.Invoke();
        }
    }

    private void ShowEntityActionRange(Vector3 entityPos, int range)
    {
        Vector3Int entityGridPos = grid.WorldToCell(entityPos);
        entityGridPos -= Vector3Int.one;

        for (int i = entityGridPos.x - range; i < entityGridPos.x + range; i++)
        {
            for (int j = entityGridPos.y - range; j < entityGridPos.y + range; j++)
            {
                if (Mathf.Abs(entityGridPos.x - i) + Mathf.Abs(entityGridPos.y - j) < range)
                {
                    Vector2Int tilePos = new Vector2Int(i, j);
                    interactiveMap.SetTile((Vector3Int)tilePos, selectedTile);
                    currentRangeTiles.Add(tilePos);
                }
            }
        }

    }

    private void ClearRangeTiles()
    {
        for (int i = 0; i < currentRangeTiles.Count; i++)
        {
            interactiveMap.SetTile((Vector3Int)currentRangeTiles[i], null);
        }

        currentRangeTiles.Clear();
    }
}
