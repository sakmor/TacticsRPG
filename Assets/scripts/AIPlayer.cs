using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIPlayer : Player {
	
	// Update
	public override void Update () {
		/*
		if (GameManager.instance.players[GameManager.instance.currentPlayerIndex] == this) {
			transform.GetComponent<Renderer>().material.color = Color.green;
		} else {
			transform.GetComponent<Renderer>().material.color = Color.white;
		}
		*/
		base.Update();
	}
	
	// 
	void OnMouseDown(){
		if(GameManager.instance.currentPlayerIndex != 0){return ;}	// 如果目前行動者不是玩家自己則不做任何事
		Tile pTile = GameManager.instance.getCurrentPlayerTile() ;
		if(TilePathFinder.FindPath(pTile,GameManager.instance.getPlayerTile(this)).Count > GameManager.instance.players[0].baseAttackRange){return ;}
		// 點擊攻擊
		GameManager.instance.attackTo(this) ;
		
	}
	
	public override void TurnUpdate ()
	{
		if (positionQueue.Count > 0) {
			transform.position += (positionQueue[0] - transform.position).normalized * moveSpeed * Time.deltaTime;
			
			if (Vector3.Distance(positionQueue[0], transform.position) <= 0.1f) {
				transform.position = positionQueue[0];
				positionQueue.RemoveAt(0);
				if (positionQueue.Count == 0) {
					actionPoints--;
				}
			}
			
		} else {
			//priority queue
			List<Tile> attacktilesInRange = TileHighlight.FindHighlight(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], attackRange, true);
			List<Tile> movementTilesInRange = TileHighlight.FindHighlight(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], movementPerActionPoint + 1000);
			//attack if in range and with lowest HP
			if (attacktilesInRange.Where(x => GameManager.instance.players.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count () > 0) {
				var opponentsInRange = attacktilesInRange.Select(x => GameManager.instance.players.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? GameManager.instance.players.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
				Player opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).First ();

				GameManager.instance.removeTileHighlights();
				moving = false;
				attacking = true;
				GameManager.instance.highlightTilesAt(gridPosition, Color.red, attackRange);

				GameManager.instance.attackWithCurrentPlayer(GameManager.instance.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y]);
			}
			// 若超過指定距離則不行動
			else if(GameManager.instance.getDistanceFromPlayer(GameManager.instance.getPlayerTile(this)) > 8.0f){
				actionPoints = 0 ;
			// 若在距離內則往玩家移動
			}else if(!moving && movementTilesInRange.Where(x => GameManager.instance.players.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count () > 0) {
				var opponentsInRange = movementTilesInRange.Select(x => GameManager.instance.players.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? GameManager.instance.players.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
				Player opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y],GameManager.instance.map[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First ();

				GameManager.instance.removeTileHighlights();
				moving = true;
				attacking = false;
				GameManager.instance.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint+2, false);
				
				List<Tile> path = TilePathFinder.FindPath (GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y],GameManager.instance.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], GameManager.instance.players.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
				if (path.Count() > 1) {
					List<Tile> actualMovement = TileHighlight.FindHighlight(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], movementPerActionPoint+2, GameManager.instance.players.Where(x => x.gridPosition != gridPosition).Select(x => x.gridPosition).ToArray());
					path.Reverse();
					if (path.Where(x => actualMovement.Contains(x)).Count() > 0) GameManager.instance.moveCurrentPlayer(path.Where (x => actualMovement.Contains(x)).First());
				}
			}
		}

		if (actionPoints <= 1 && (attacking || moving)) {
			moving = false;
			attacking = false;			
		}
		base.TurnUpdate ();
	}
	
	public override void TurnOnGUI () {
		base.TurnOnGUI ();
	}
}
