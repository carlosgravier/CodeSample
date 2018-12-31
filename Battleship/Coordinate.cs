using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public HitState hitState { get; set; }

        public Coordinate()
        {

        }

        public Coordinate(int x, int y, HitState hitState)
        {
            this.X = x;
            this.Y = y;
            this.hitState = hitState;
        }

        public bool IsOffGrid(int value)
        {
            if (value < 0 || value > 9)
                return true;
            return false;
        }

        public bool IsOverlappingAnyShips(Player player, Coordinate coordinate)
        {
            if (player.PlayerOccupiedCoordinates.Where(x=> x.X == coordinate.X && x.Y == coordinate.Y).Any())
                return true;

            return false;
        }

        public bool IsValid(Player player, Coordinate coordinate)
        {
            if (IsOffGrid(coordinate.X) || IsOffGrid(coordinate.Y) || IsOverlappingAnyShips(player, coordinate))
                return false;

            return true;
        }
    }
}
