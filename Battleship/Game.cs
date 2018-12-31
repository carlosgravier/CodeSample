using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public enum GameModes
    {
        PlayerFleetPlacement,
        EnemyFleetPlacement,
        Battle
    }

    public class Game
    {


        Random random = new Random();
        public GameModes CurrentGameMode { get; set; }
        public Ship CurrentShipToPlace { get; set; }
        public Player CurrentPlayer { get; set; }

        public Player Player1 = new Player("Player 1");
        public Player Enemy = new Player("Enemy");
        public List<GridSquare> GridSquares = new List<GridSquare>();
        List<Ship> PlacementState = new List<Ship>();
        bool IsTrackingTarget = false;
        Stack<HitTracker> TrackingStack = new Stack<HitTracker>();

        public Game()
        {
            StartGame();
            EnemyShipPlacement();
        }

        public void StartGame()
        {
            CurrentGameMode = GameModes.PlayerFleetPlacement;
            PlacementState = InitializeShipPlacementState(Player1);
            CurrentPlayer = Player1;
            CurrentShipToPlace = Player1.Fleet.Where(x => x.Type == Ship.ShipType.AircraftCarrier).First();
        }

        public List<Ship> InitializeShipPlacementState(Player player)
        {
            var retval = new List<Ship>();
            retval.AddRange(player.Fleet);
            return retval.ToList();
        }

        public Coordinate AcceptClickCoordinate(int x, int y)
        {
            var coordinate = new Coordinate(x, y, new HitState());
            return coordinate;
        }

        public void SetupGameboardClick(int x, int y, Ship.ShipDirection direction)
        {
            var coordinate = new Coordinate();
            Console.WriteLine($"Place ship for {CurrentPlayer.PlayerName}. Ship to place is {CurrentShipToPlace.Type}.");

            coordinate = AcceptClickCoordinate(x, y);

            CurrentShipToPlace.BowCoordinate = coordinate;
            CurrentShipToPlace.Direction = direction;

            List<Coordinate> coordslist;
            bool isValidPlacement;
            ValidateShipPlacement(out coordslist, out isValidPlacement);

            if (isValidPlacement)
            {
                PlaceShipOnGameBoard(coordslist);
            }
            else
            {
                Console.WriteLine($"{Environment.NewLine}Not a valid placement");
            }
        }

        private void ValidateShipPlacement(out List<Coordinate> coordslist, out bool isValidPlacement)
        {
            coordslist = CurrentShipToPlace.ShipCoordinatesList(CurrentShipToPlace);
            isValidPlacement = coordslist.All(x => x.IsValid(CurrentPlayer, x));
        }

        private void PlaceShipOnGameBoard(List<Coordinate> coordslist)
        {
            Console.WriteLine($"{Environment.NewLine}Placing ship {CurrentShipToPlace.Type} for {CurrentPlayer.PlayerName} facing {CurrentShipToPlace.Direction} starting at {CurrentShipToPlace.BowCoordinate.X}, {CurrentShipToPlace.BowCoordinate.Y}");
            CurrentShipToPlace.InitializeCoordinateList(CurrentShipToPlace, coordslist);
            CurrentPlayer.PlayerOccupiedCoordinates.AddRange(coordslist);
            var toRemove = PlacementState.Where(x => x.Type == CurrentShipToPlace.Type).First();
            PlacementState.Remove(toRemove);


            if (PlacementState.Any())
            {
                CurrentShipToPlace = PlacementState[0];
            }
            else
            {
                CurrentGameMode = GameModes.Battle;
            }
        }

        private void TryToRandomlyPlaceShips()
        {
            var randomX = random.Next(0, 10);
            var randomY = random.Next(0, 10);
            var randomCoordinate = new Coordinate(randomX, randomY, new HitState());

            if (random.Next(0, 2) == 0)
                CurrentShipToPlace.Direction = Ship.ShipDirection.Horizontal;
            else
                CurrentShipToPlace.Direction = Ship.ShipDirection.Vertical;

            CurrentShipToPlace.BowCoordinate = randomCoordinate;

            List<Coordinate> coordslist;
            bool isValidPlacement;
            ValidateShipPlacement(out coordslist, out isValidPlacement);


            if (!isValidPlacement)
                TryToRandomlyPlaceShips();
            else
            {
                Console.WriteLine($"{Environment.NewLine}Placing ship {CurrentShipToPlace.Type} for {CurrentPlayer.PlayerName} facing {CurrentShipToPlace.Direction} starting at {CurrentShipToPlace.BowCoordinate.X}, {CurrentShipToPlace.BowCoordinate.Y}");
                CurrentShipToPlace.InitializeCoordinateList(CurrentShipToPlace, coordslist);
                CurrentPlayer.PlayerOccupiedCoordinates.AddRange(coordslist);
                var toRemove = PlacementState.Where(x => x.Type == CurrentShipToPlace.Type).First();
                PlacementState.Remove(toRemove);

                if (PlacementState.Any())
                {
                    CurrentShipToPlace = PlacementState[0];
                    TryToRandomlyPlaceShips();
                }
                else
                {
                    CurrentGameMode = GameModes.Battle;
                    CurrentPlayer = Player1;
                    Console.WriteLine("Starting battle!!");
                }
            }
        }

        private void EnemyShipPlacement()
        {
            CurrentPlayer = Enemy;
            PlacementState = InitializeShipPlacementState(CurrentPlayer);
            CurrentShipToPlace = PlacementState[0];
            Console.WriteLine("Placing ememy ships");
            TryToRandomlyPlaceShips();
        }

        public Tuple<bool, bool, string, bool> CheckFireResult(Player player, int xcoord, int ycoord)
        {
            //Tuple definition:
            //bool was a hit
            //bool was a ship sunk
            //string the ship name that was sunk
            //victory condtion was reached

            var c = new Coordinate(xcoord, ycoord, new HitState());
            foreach (Ship s in player.Fleet)
            {
                if (s.CoordinatesList.Where(x => x.X == c.X && x.Y == c.Y).Any())
                {
                    s.CoordinatesList.Where(x => x.X == c.X && x.Y == c.Y).First().hitState.WasHit = HitState.HitValue.Hit;

                    //player 1s ship was hit
                    if (player.PlayerName == "Player 1")
                    {
                        var gridSquare = GridSquares.Where(x => x._rectangle.Name == $"fleet_{c.X}_{c.Y}").Single();
                        gridSquare.WasHit = HitState.HitValue.Hit;
                        IsTrackingTarget = true;
                        var hitTracker = new HitTracker(gridSquare);
                        TrackingStack.Push(hitTracker);
                    }

                    var wasSunk = WasShipSunk(s);
                    if (wasSunk)
                    {
                        //player 1's ship was sunk
                        if (player.PlayerName == "Player 1")
                        {
                            IsTrackingTarget = false;
                            TrackingStack = new Stack<HitTracker>();
                        }
                        bool victoryState = CheckVictoryState(player);
                        return Tuple.Create(true, true, s.ShipName, victoryState);
                    }
                    else
                    {
                        //was hit but did not sink
                        if (player.PlayerName == "Player 1" && IsTrackingTarget)
                        {
                            if (TrackingStack.Any())
                            {
                                var stackTop = TrackingStack.Peek();
                                stackTop.lastTryWasSuccessful = true;
                            }
                        }
                        return Tuple.Create(true, false, String.Empty, false);
                    }
                }
            }

            //was a miss
            if (TrackingStack.Any())
            {
                var topOfStack = TrackingStack.Peek();
                topOfStack.lastTryWasSuccessful = false;
            }

            return Tuple.Create(false, false, String.Empty, false);
        }

        private bool CheckVictoryState(Player player)
        {
            bool retval = true;
            foreach (Ship s in player.Fleet)
            {
                if (s.CoordinatesList.Any(x => x.hitState.WasHit != HitState.HitValue.Hit))
                    return false;
            }
            return retval;
        }

        public bool WasShipSunk(Ship s)
        {
            if (s.CoordinatesList.All(x => x.hitState.WasHit == HitState.HitValue.Hit))
            {
                return true;
            }
            return false;
        }

        public GridSquare EnemyGuess()
        {
            var availableGuesses = GridSquares.Where(x => x._rectangle.Name.StartsWith("fleet") && x.WasHit == HitState.HitValue.Empty).ToList();

            if (!IsTrackingTarget)
            {
                var r = new Random();
                var index = r.Next(availableGuesses.Count);
                return availableGuesses.ElementAt(index);
            }
            else
            {
                return DetermineEnemyGuess(availableGuesses);
            }
        }

        private GridSquare DetermineEnemyGuess(List<GridSquare> availableGuesses)
        {
            if (TrackingStack.Count > 0)
            {
                var stackTop = TrackingStack.Peek();

                if (TrackingStack.Count == 1)
                {

                    //only one item is in the stack
                    //lets try to exhaust all direction options before giving up
                    if (!stackTop.hasTriedLeft || !stackTop.hasTriedUp || !stackTop.hasTriedRight || !stackTop.hasTriedDown)
                        return TryGridDirection(stackTop, availableGuesses);

                    //if the current items options have been exhausted and there was only one item in the stack just make a new random guess
                    return ResetTrackingState(availableGuesses);
                }
                else
                {
                    //tracking stack has more than 1 item so try to keep tracking in the same direction

                    if (stackTop.hasTriedLeft || stackTop.hasTriedUp || stackTop.hasTriedRight || stackTop.hasTriedDown)
                        TrackingStack.Pop();
                    stackTop = TrackingStack.Peek();

                    if (!stackTop.hasTriedLeft || !stackTop.hasTriedUp || !stackTop.hasTriedRight || !stackTop.hasTriedDown)
                        return TryGridDirection(stackTop, availableGuesses);

                    //fell all the way through
                    return ResetTrackingState(availableGuesses);
                }
            }
            else
            {
                //there are no items in the stack and the tracking state is on
                //this shouldnt be possible but lets bail out by turning off tracking and making another random guess
                return ResetTrackingState(availableGuesses);
            }
        }

        private GridSquare ResetTrackingState(List<GridSquare> availableGuesses)
        {
            TrackingStack = new Stack<HitTracker>();
            IsTrackingTarget = false;
            var r = new Random();
            var index = r.Next(availableGuesses.Count);
            return availableGuesses.ElementAt(index);
        }

        private GridSquare TryGridDirection(HitTracker currentlyTracked, List<GridSquare> availableGuesses)
        {
            var nameArray = currentlyTracked.gridSquare._rectangle.Name.Split('_');
            var xcoord = Int32.Parse(nameArray[1]);
            var ycoord = Int32.Parse(nameArray[2]);

            //find adjacent gridsquares
            var left = availableGuesses.Where(x => x._rectangle.Name == $"fleet_{xcoord - 1}_{ycoord}");
            var right = availableGuesses.Where(x => x._rectangle.Name == $"fleet_{xcoord + 1}_{ycoord}");
            var up = availableGuesses.Where(x => x._rectangle.Name == $"fleet_{xcoord}_{ycoord - 1}");
            var down = availableGuesses.Where(x => x._rectangle.Name == $"fleet_{xcoord}_{ycoord + 1}");

            //first pass
            if (currentlyTracked.direction == HitTracker.TrackingDirection.Neutral)
            {
                //try new directions if the last try didnt succeed
                if (left.Any() && !currentlyTracked.hasTriedLeft)
                {
                    var retval = left.First();
                    currentlyTracked.direction = HitTracker.TrackingDirection.Left;
                    currentlyTracked.hasTriedLeft = true;
                    return retval;
                }
                if (up.Any() && !currentlyTracked.hasTriedUp)
                {
                    var retval = up.First();
                    currentlyTracked.direction = HitTracker.TrackingDirection.Up;
                    currentlyTracked.hasTriedUp = true;
                    return retval;
                }
                if (right.Any() && !currentlyTracked.hasTriedRight)
                {
                    var retval = right.First();
                    currentlyTracked.direction = HitTracker.TrackingDirection.Right;
                    currentlyTracked.hasTriedRight = true;
                    return retval;
                }
                if (down.Any() && !currentlyTracked.hasTriedDown)
                {
                    var retval = down.First();
                    currentlyTracked.direction = HitTracker.TrackingDirection.Down;
                    currentlyTracked.hasTriedDown = true;
                    return retval;
                }
            }
            else
            {
                //is currently tracking a direction

                //if the last try was successful keep tracking that direction
                if (left.Any() && currentlyTracked.direction == HitTracker.TrackingDirection.Left && currentlyTracked.lastTryWasSuccessful)
                {
                    return left.First();
                }
                if (up.Any() && currentlyTracked.direction == HitTracker.TrackingDirection.Up && currentlyTracked.lastTryWasSuccessful)
                {
                    return up.First();
                }
                if (right.Any() && currentlyTracked.direction == HitTracker.TrackingDirection.Right && currentlyTracked.lastTryWasSuccessful)
                {
                    return right.First();
                }
                if (down.Any() && currentlyTracked.direction == HitTracker.TrackingDirection.Down && currentlyTracked.lastTryWasSuccessful)
                {
                    return down.First();
                }

                //if the last try wasn't successful guess in a clockwise direction
                if (up.Any() && currentlyTracked.direction == HitTracker.TrackingDirection.Left && !currentlyTracked.lastTryWasSuccessful)
                {
                    currentlyTracked.direction = HitTracker.TrackingDirection.Up;
                    return up.First();
                }
                if (right.Any() && currentlyTracked.direction == HitTracker.TrackingDirection.Up && !currentlyTracked.lastTryWasSuccessful)
                {
                    currentlyTracked.direction = HitTracker.TrackingDirection.Right;
                    return right.First();
                }
                if (down.Any() && currentlyTracked.direction == HitTracker.TrackingDirection.Right && !currentlyTracked.lastTryWasSuccessful)
                {
                    currentlyTracked.direction = HitTracker.TrackingDirection.Down;
                    return down.First();
                }
                if (left.Any() && currentlyTracked.direction == HitTracker.TrackingDirection.Down && !currentlyTracked.lastTryWasSuccessful)
                {
                    currentlyTracked.direction = HitTracker.TrackingDirection.Left;
                    return left.First();
                }
            }

            //if we fell all the way through just make a new random guess
            return ResetTrackingState(availableGuesses);

        }
    }
}
