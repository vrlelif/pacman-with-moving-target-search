using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pacman.GameLogic.Ghosts
{
    [Serializable()]
	public class Red : Ghost, ICloneable
	{
		public const int StartX = 111, StartY = 93;

		public int WaitToEnter //for test
        {
			get { return waitToEnter; }
        }

		public int getStartX //for test
		{
			get { return StartX; }
		}

		public int getStartY //for test
		{
			get { return StartY; }
		}


		public Red(int x, int y, GameState gameState, double Speed, double FleeSpeed)
			: base(x, y, gameState) {
			this.name = "Red";
			ResetPosition();
            this.Speed = Speed;
            this.FleeSpeed = FleeSpeed;
        }

		public override void PacmanDead() {
			waitToEnter = 0;
			ResetPosition();			
		}

		public override void ResetPosition() {
			x = StartX;
			y = StartY;
			waitToEnter = 0;
			direction = Direction.Left;
			base.ResetPosition();
			entered = true;
		}

		public override void Move() {
			
			bool condition = Distance(GameState.Pacman) > randomMoveDist && GameState.Random.Next(0, randomMove) == 0;

			if (condition) {
				MoveRandom();
			} else {
				MoveAsRed();
			}
			base.Move();
		}

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public new Red Clone()
        {
            Red _temp = (Red)this.MemberwiseClone();
            _temp.Node = Node.Clone();

            return _temp;
        }

        #endregion
    }
}
