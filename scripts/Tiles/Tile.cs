using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	GameObject PREFAB;

	public GameObject visual;

	public TileType type = TileType.Normal1;

	public Vector2 gridPosition = Vector2.zero;
	
	public int movementCost = 1;
	public bool impassible = false;
	
	public List<Tile> neighbors = new List<Tile>();

	// Start
	void Start () {
		if (Application.loadedLevelName == "gameScene") generateNeighbors();
	}
	
	public void generateNeighbors() {		
		neighbors = new List<Tile>();
		
		//up
		if (gridPosition.y > 0) {
			Vector2 n = new Vector2(gridPosition.x, gridPosition.y - 1);
			neighbors.Add(GameManager.instance.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
		}
		//down
		if (gridPosition.y < GameManager.instance.mapSize - 1) {
			Vector2 n = new Vector2(gridPosition.x, gridPosition.y + 1);
			neighbors.Add(GameManager.instance.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
		}		
		
		//left
		if (gridPosition.x > 0) {
			Vector2 n = new Vector2(gridPosition.x - 1, gridPosition.y);
			neighbors.Add(GameManager.instance.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
		}
		//right
		if (gridPosition.x < GameManager.instance.mapSize - 1) {
			Vector2 n = new Vector2(gridPosition.x + 1, gridPosition.y);
			neighbors.Add(GameManager.instance.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
		}
	}

	//==========
	// 滑鼠事件
	//==========
	void OnMouseEnter() {
		if (Application.loadedLevelName == "MapCreatorScene" && Input.GetMouseButton(0)) {
			setType(MapCreatorManager.instance.palletSelection);
		}
	}
	
	void OnMouseDown() {
		
		//if(GameManager.instance.currentPlayerIndex != 0){return ;}	// 如果目前行動者不是玩家自己則不做任何事
		
		if (Application.loadedLevelName == "gameScene") {
			if (GameManager.instance.players[GameManager.instance.currentPlayerIndex].attacking) {
				GameManager.instance.attackWithCurrentPlayer(this);
			}else{
				// 取得玩家目前所在位置
				Player p = GameManager.instance.players[0] ;
				Tile pTile = GameManager.instance.getCurrentPlayerTile() ;
				List<Tile> path = TilePathFinder.FindPath(pTile,this, GameManager.instance.getPlayersGrid(this)) ;
				Debug.Log(path.Count) ;
				// 如果旁邊都不能走  就回合結束
				if(!path.Contains(pTile.neighbors[0]) && !path.Contains(pTile.neighbors[1]) && !path.Contains(pTile.neighbors[2]) && !path.Contains(pTile.neighbors[3])){
					p.actionPoints -- ;
					return ;
				}
				Tile dest = path.Count >= p.baseMovementPerActionPoint ? path[p.baseMovementPerActionPoint-1] : this ; 
				if(dest == pTile){return;}
				// 如果敵人在此格上 則按下格子不會移動
				if(!enemyOnTile(dest)){
					GameManager.instance.moveCurrentPlayer(dest);
				}
			}
		} else if (Application.loadedLevelName == "MapCreatorScene") {
			setType(MapCreatorManager.instance.palletSelection);
		}
	}
	bool enemyOnTile(Tile destTile){
		foreach (Player p in GameManager.instance.players) {
			if (p.gridPosition == destTile.gridPosition) {
				return true ;
			}
		}
		return false ;
	}
	//==========================
	// 設定網格類型，是否是障礙物
	//==========================
	public void setType(TileType t) {
		type = t;
		switch(t)
		{
		case TileType.Normal1:
			movementCost = 1;
			impassible = false;
			PREFAB = PrefabHolder.instance.Normal1;
			break;

		case TileType.Normal2:
			movementCost = 1;
			impassible = false;
			PREFAB = PrefabHolder.instance.Normal2;
			break;

		case TileType.Normal3:
			movementCost = 1;
			impassible = false;
			PREFAB = PrefabHolder.instance.Normal3;
			break;

		case TileType.Normal4:
			movementCost = 1;
			impassible = false;
			PREFAB = PrefabHolder.instance.Normal4;
			break;

		case TileType.Bridge:
			movementCost = 1;
			impassible = false;
			PREFAB = PrefabHolder.instance.Bridge;
			break;
			//-------以下不可走-------//
		case TileType.Impassible_Empty:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_Empty;
			break;

		case TileType.Impassible_FakeNomal:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_FakeNomal;
			break;

		case TileType.Impassible_pillar:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_pillar;
			break;

		case TileType.Impassible_matelWall_col:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_matelWall_col;
			break;

		case TileType.Impassible_matelWall_row:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_matelWall_row;
			break;

		case TileType.Impassible_RockWall_col:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_RockWall_col;
			break;
		case TileType.Impassible_RockWall_row:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_RockWall_row;
			break;

		case TileType.Impassible_RockWall_row_light:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_RockWall_row_light;
			break;

		case TileType.Impassible_RockWall_col_light:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_RockWall_col_light;
			break;

		case TileType.Impassible_Door_col:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_Door_col;
			break;

		case TileType.Impassible_Door_row:
			movementCost = 999999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible_Door_row;
			break;


		case TileType.Impassible:
			movementCost = 9999;
			impassible = true;
			PREFAB = PrefabHolder.instance.Impassible;
			break;



			default:
			movementCost = 1;
			impassible = false;
			PREFAB = PrefabHolder.instance.Normal1;
			break;
		}

		generateVisuals();
	}

	public void generateVisuals() {
		GameObject container = transform.FindChild("Visuals").gameObject;
		//initially remove all children
		for(int i = 0; i < container.transform.childCount; i++) {
			Destroy (container.transform.GetChild(i).gameObject);
		}

		GameObject newVisual = (GameObject)Instantiate(PREFAB, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
		newVisual.transform.parent = container.transform;

		visual = newVisual;
	}
}
