using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	
	public GameObject TilePrefab;
	public GameObject UserPlayerPrefab;
	public GameObject AIPlayerPrefab;
	
	public int mapSize = 22;
	Transform mapTransform;
	
	public List <List<Tile>> map = new List<List<Tile>>();	// 全地圖的網格清單
	public List <Player> players = new List<Player>();		// 所有角色(包含敵人)
	public int currentPlayerIndex = 0;
	
	void Awake() {
		instance = this;

		mapTransform = transform.FindChild("Map");
	}
	
	// Start
	void Start () {		
		generateMap();
		generatePlayers();
	}
	
	// Update
	void Update () {
		
		if (players[currentPlayerIndex].HP > 0) players[currentPlayerIndex].TurnUpdate();
		else nextTurn();
	}
	
	void OnGUI () {
		if (players[currentPlayerIndex].HP > 0) players[currentPlayerIndex].TurnOnGUI();
	}
	// 取得目前玩家
	public Player getCurrentPlayer(){
		return players[currentPlayerIndex] ;
	}
	// 下一回合
	public void nextTurn() {
		if (currentPlayerIndex + 1 < players.Count) {
			currentPlayerIndex++;
		} else {
			currentPlayerIndex = 0;
		}
	}
	// 加亮網格
	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers=true) {

		List <Tile> highlightedTiles = new List<Tile>();	// 需要加亮的網格
		// 加亮方式有兩種，忽略或不忽略玩家
		if (ignorePlayers) highlightedTiles = TileHighlight.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance, highlightColor == Color.red);
		else highlightedTiles = TileHighlight.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance, players.Where(x => x.gridPosition != originLocation).Select(x => x.gridPosition).ToArray(), highlightColor == Color.red);
		// 改變材質顏色
		foreach (Tile t in highlightedTiles) {
			t.visual.transform.GetComponent<Renderer>().materials[0].color = highlightColor;
		}
	}
	// 移除加亮網格
	public void removeTileHighlights() {
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				if (!map[i][j].impassible) map[i][j].visual.transform.GetComponent<Renderer>().materials[0].color = Color.white;
			}
		}
	}
 	// 移動目前行動的角色
	public void moveCurrentPlayer(Tile destTile) {
		if (!destTile.impassible && players[currentPlayerIndex].positionQueue.Count == 0) 
		{
			removeTileHighlights();
			players[currentPlayerIndex].moving = false;
			foreach(Tile t in TilePathFinder.FindPath(map[(int)players[currentPlayerIndex].gridPosition.x][(int)players[currentPlayerIndex].gridPosition.y],destTile, players.Where(x => x.gridPosition != destTile.gridPosition && x.gridPosition != players[currentPlayerIndex].gridPosition).Select(x => x.gridPosition).ToArray()))
			{
				players[currentPlayerIndex].positionQueue.Add(map[(int)t.gridPosition.x][(int)t.gridPosition.y].transform.position + 0.5f * Vector3.up);
			}			
			players[currentPlayerIndex].gridPosition = destTile.gridPosition;

		} 
	}
	// 取得目前玩家所在的網格
	public Tile getCurrentPlayerTile(){
		return map[(int)players[currentPlayerIndex].gridPosition.x][(int)players[currentPlayerIndex].gridPosition.y] ;
	}
	public Vector2[] getPlayersGrid(Tile destTile){
		return players.Where(x => x.gridPosition != destTile.gridPosition && x.gridPosition != players[currentPlayerIndex].gridPosition).Select(x => x.gridPosition).ToArray() ;
	}
	// 取得特定玩家所在網格
	public Tile getPlayerTile(Player p){
		return map[(int)p.gridPosition.x][(int)p.gridPosition.y] ;
	}
	// 取得網格與角色的距離
	public float getDistanceFromPlayer(Tile tile){
		return Vector3.Distance(tile.gridPosition, players[0].gridPosition) ;
	}
	// 目前角色攻擊敵人
	public void attackWithCurrentPlayer(Tile destTile) {
		if (!destTile.impassible) {
			
			Player target = null;
			foreach (Player p in players) {
				if (p.gridPosition == destTile.gridPosition) {
					target = p;
				}
			}
			
			if (target != null) {
								
				
				players[currentPlayerIndex].actionPoints--;
				
				removeTileHighlights();
				players[currentPlayerIndex].attacking = false;			
				
				//attack logic
				//roll to hit
				bool hit = Random.Range(0.0f, 1.0f) <= players[currentPlayerIndex].attackChance - target.evade;
				
				if (hit) {
					//damage logic
					int amountOfDamage = Mathf.Max(0, (int)Mathf.Floor(players[currentPlayerIndex].damageBase + Random.Range(0, players[currentPlayerIndex].damageRollSides)) - target.damageReduction);
					
					target.HP -= amountOfDamage;
					
				} else {
					Debug.Log(players[currentPlayerIndex].playerName + " missed " + target.playerName + "!");
				}
			}
		} else {
			Debug.Log ("destination invalid");
		}
	}
	// 玩家攻擊敵人
	public void attackTo(Player tar){
		// 扣減行動點數
		players[0].actionPoints -- ;
		players[0].attacking = false;
		// 命中率
		bool hit = Random.Range(0.0f, 1.0f) <= players[0].attackChance - tar.evade;
				
		if (hit) {
			// 傷害計算
			int amountOfDamage = Mathf.Max(0, (int)Mathf.Floor(players[0].damageBase + Random.Range(0, players[0].damageRollSides)) - tar.damageReduction);
					
			tar.HP -= amountOfDamage;
					
		} else {
			Debug.Log(players[0].playerName + " missed " + tar.playerName + "!");
		}
	}
	//========================
	// 生成地圖，讀取外部xml檔案
	//========================
	void generateMap() {
		loadMapFromXml();
	}

	void loadMapFromXml() {
		MapXmlContainer container = MapSaveLoad.Load("map.xml");
		
		mapSize = container.size;
		
		//initially remove all children
		for(int i = 0; i < mapTransform.childCount; i++) {
			Destroy (mapTransform.GetChild(i).gameObject);
		}
		
		map = new List<List<Tile>>();
		for (int i = 0; i < mapSize; i++) {
			List <Tile> row = new List<Tile>();
			for (int j = 0; j < mapSize; j++) {
				Tile tile = ((GameObject)Instantiate(PrefabHolder.instance.BASE_TILE_PREFAB, new Vector3(i - Mathf.Floor(mapSize/2),0, -j + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
				tile.transform.parent = mapTransform;
				tile.gridPosition = new Vector2(i, j);
				tile.setType((TileType)container.tiles.Where(x => x.locX == i && x.locY == j).First().id);
				row.Add (tile);
			}
			map.Add(row);
		}
	}
	
	//==========
	// 生成角色
	//==========
	void generatePlayers() {
		UserPlayer player;	// 角色物件
		
		// 生成角色 定義屬性 名字、位置等等
		player = ((GameObject)Instantiate(UserPlayerPrefab, new Vector3(0 - Mathf.Floor(mapSize/2),0.5f, -0 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<UserPlayer>();
		player.gridPosition = new Vector2(0,0);
		player.playerName = "Bob";
		player.headArmor = Armor.FromKey(ArmorKey.LeatherCap);
		player.chestArmor = Armor.FromKey(ArmorKey.MagicianCloak);
		player.handWeapons.Add(Weapon.FromKey(WeaponKey.LongSword));
		// 加入角色到清單中
		players.Add(player);
		
		
		AIPlayer aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(6 - Mathf.Floor(mapSize/2),0.5f, -4 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
		aiplayer.gridPosition = new Vector2(6,4);
		aiplayer.playerName = "Bot1";
		aiplayer.chestArmor = Armor.FromKey(ArmorKey.IronHelmet);
		aiplayer.handWeapons.Add(Weapon.FromKey(WeaponKey.LongSword));
		
		players.Add(aiplayer);

		aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(8 - Mathf.Floor(mapSize/2),0.5f, -4 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
		aiplayer.gridPosition = new Vector2(8,4);
		aiplayer.playerName = "Bot2";
		aiplayer.handWeapons.Add(Weapon.FromKey(WeaponKey.LongSword));
		
		players.Add(aiplayer);

		aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(12 - Mathf.Floor(mapSize/2),0.5f, -1 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
		aiplayer.gridPosition = new Vector2(12,1);
		aiplayer.playerName = "Bot3";
		aiplayer.chestArmor = Armor.FromKey(ArmorKey.LeatherVest);
		aiplayer.handWeapons.Add(Weapon.FromKey(WeaponKey.ShortBow));
		
		players.Add(aiplayer);

		aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(18 - Mathf.Floor(mapSize/2),0.5f, -8 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
		aiplayer.gridPosition = new Vector2(18,8);
		aiplayer.playerName = "Bot4";
		aiplayer.handWeapons.Add(Weapon.FromKey(WeaponKey.LongSword));

		players.Add(aiplayer);
	}
}
