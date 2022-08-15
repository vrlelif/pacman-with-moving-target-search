using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pacman.GameLogic
{
    [Serializable()]
	public class Pacman : Entity, ICloneable
	{
		public const int StartX = 111, StartY = 189;

		private int counter;
		public int Score;
		public int Lives;
		private bool gotExtraLife;		
		private int eatGhostBonus;

		public Pacman(int x, int y, GameState gameState, double Speed)
			: base(x, y, gameState) {
			this.Speed = Speed;
			Reset();
		}

		public void SetPosition(int x, int y, Direction direction) {
			this.x = x;
			this.y = y;
			this.direction = direction;
			// eat all pills on path from last node
			Node curNode = Node;
			Node nextNode = GameState.Map.GetNode(X, Y);			
			if( nextNode.Type == Node.NodeType.Wall ) {
				//nextNode = curNode;
				throw new ApplicationException("You cannot set your destination to a wall");
			}
			if( curNode.ShortestPath[nextNode.X, nextNode.Y] != null ) {
				while( curNode != nextNode ) {
					curNode = curNode.GetNode(curNode.ShortestPath[nextNode.X, nextNode.Y].Direction);
					if( curNode.Type == Node.NodeType.Pill || curNode.Type == Node.NodeType.PowerPill ) {
						GameState.Map.PillsLeft--;
						curNode.Type = Node.NodeType.None;
					}
				}
				// set new node
				Node = nextNode;
			}
		}

		public void SetDirection(Direction direction) {
            NextDirection = direction;
		}

		public void Die() {
			Lives--;
			if( GameState.Controller != null ) {
				GameState.Controller.EatenByGhost();
			}
			ResetPosition();
		}

		public void Reset() {
            Lives = 0;
			Score = 0;
			gotExtraLife = false;
			counter = 0;
			ResetPosition();
		}

		public void ResetPosition() {
			x = StartX;
			y = StartY;
			Node = GameState.Map.GetNode(X, Y);
			direction = Direction.Left;
			NextDirection = Direction.Left;
		}

		public void EatGhost() {		
			Score += eatGhostBonus;

            GameState.m_GhostsEaten++;

			eatGhostBonus *= 2;
			if( GameState.Controller != null ) {
				GameState.Controller.EatGhost();
			}
		}

        // This utilizes the likes of the 2D array enum to determine whether or not
        // a pill has been eaten at a given co-ordinate
        protected override void ProcessNodeSimulated() {
            if (GameState.Map.NodeData[Node.X,Node.Y] == Node.NodeType.Pill || GameState.Map.NodeData[Node.X,Node.Y] == Node.NodeType.PowerPill)
            {
                if (GameState.Map.NodeData[Node.X,Node.Y] == Node.NodeType.PowerPill)
                {
                    foreach (var g in GameState.Ghosts)
                    {
                        g.Flee();
                    }
                    eatGhostBonus = 200;
                    Score += 50;
                    if (GameState.Controller != null)
                    {
                        GameState.Controller.EatPowerPill();
                        // GameState.PacmanMortal = true;
                    }
                    GameState.ReverseGhosts();
                }
                else
                {
                    Score += 10;
                    if (GameState.Controller != null)
                    {
                        GameState.Controller.EatPill();
                    }
                }
                GameState.m_PillsEaten++;
                if(GameState.Map.NodeData[Node.X,Node.Y] == Node.NodeType.PowerPill)
                {
                    GameState.m_PowerPillsEaten++;
                }
                GameState.Map.NodeData[Node.X,Node.Y] = Node.NodeType.None;
                GameState.Map.Nodes[Node.X, Node.Y].Type = Node.NodeType.None;
                GameState.Map.PillsLeft--;
                if (Score >= 10000 && !gotExtraLife)
                {
                    gotExtraLife = true;
                    Lives++;
                }
            }
        }

		protected override void ProcessNode() {
			if( Node.Type == Node.NodeType.Pill || Node.Type == Node.NodeType.PowerPill ) {
				if( Node.Type == Node.NodeType.PowerPill ) {
					foreach(var g in GameState.Ghosts ) {
						g.Flee();
					}
					eatGhostBonus = 200;
                    Score += 50;
					if( GameState.Controller != null ) {
						GameState.Controller.EatPowerPill();
                       // GameState.PacmanMortal = true;
					}
					GameState.ReverseGhosts();
                    GameState.m_PowerPillsEaten++;
                } else {
                    Score += 10;
					if( GameState.Controller != null ) {
						GameState.Controller.EatPill();
					}
				}
				Node.Type = Node.NodeType.None;
                GameState.m_PillsEaten++;
				GameState.Map.PillsLeft--;				
				if(Score >= 10000 && !gotExtraLife ) {
					gotExtraLife = true;
					Lives++;
				}
			}
		}

		public override void Draw(Graphics g, Image sprites) {
			int offset = 0;
			if( counter % 4 < 2 ) {
				offset = Width;
			}
			switch( Direction ) {				
				case Direction.Down: offset += Width * 2; break;
				case Direction.None:
				case Direction.Left: offset += Width * 4; break;
				case Direction.Right: offset += Width * 6; break;
			}
			g.DrawImage(sprites, new Rectangle(ImgX, ImgY, 14, 14), new Rectangle(offset, 0, 13, 14), GraphicsUnit.Pixel);
			counter++;
		}

        #region ICloneable Members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Pacman _temp = (Pacman)this.MemberwiseClone();
            _temp.Node = this.Node.Clone();
            return _temp;
            //throw new NotImplementedException();
        }

        #endregion
    }
}
