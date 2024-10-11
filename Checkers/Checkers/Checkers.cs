using System;
using System.Collections.Generic;

namespace Checkers
{
    public class Square
    {
        public string Color { get; set; }
        public bool IsEmpty { get; set; }
        public Piece TypePiece { get; set; }

        public void PlacePiece(Piece piece)
        {
            TypePiece = piece;
            IsEmpty = false;
        }

        public void RemovePiece()
        {
            TypePiece = null;
            IsEmpty = true;
        }
    }

    public class Piece
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public string Color { get; set; }
        public bool IsKing { get; set; }
        public bool IsAlive { get; set; }

        public Piece(int x, int y, string color)
        {
            PosX = x;
            PosY = y;
            Color = color;
            IsKing = false;
            IsAlive = true;
        }

        public void Move(int newX, int newY)
        {
            PosX = newX;
            PosY = newY;
        }

        public void Remove(Player player)
        {
            IsAlive = false; // Mark the piece as not alive
            player.Pieces.Remove(this); // Remove from the player's list
        }
    }

    public class Player
    {
        public string Color { get; set; }
        public List<Piece> Pieces { get; set; }

        public Player(string color)
        {
            Color = color;
            Pieces = new List<Piece>();
        }

        public void MakeMove(Piece piece, int newX, int newY)
        {
            piece.Move(newX, newY);
        }
    }

    public class Board
    {
        public Square[,] BoardArr { get; set; }

        public Board()
        {
            BoardArr = new Square[8, 8];
            InitBoard();
        }

        public void InitBoard()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    BoardArr[x, y] = new Square
                    {
                        Color = (x + y) % 2 == 0 ? "White" : "Black",
                        IsEmpty = true
                    };
                }
            }
        }

        public void ShowBoard()
        {
            // Display column letters
            Console.WriteLine("   a   b   c   d   e   f   g   h");
            for (int y = 0; y < 8; y++)
            {
                Console.Write($"{8 - y} "); // Display row numbers
                for (int x = 0; x < 8; x++)
                {
                    var square = BoardArr[x, y];
                    if (square.IsEmpty)
                        Console.Write(" .  ");
                    else
                        Console.Write(square.TypePiece.Color == "Black" ? " B  " : " W  ");
                }
                Console.WriteLine();
            }
        }

        public bool IsMoveOk(Piece piece, int newX, int newY)
        {
            if (newX < 0 || newX >= 8 || newY < 0 || newY >= 8)
                return false;

            if (!BoardArr[newX, newY].IsEmpty)
                return false;

            int xDiff = Math.Abs(newX - piece.PosX);
            int yDiff = Math.Abs(newY - piece.PosY);

            // Check for a simple move
            if (xDiff == 1 && yDiff == 1)
            {
                return true;
            }

            // Check for a capture
            if (xDiff == 2 && yDiff == 2)
            {
                int midX = (piece.PosX + newX) / 2;
                int midY = (piece.PosY + newY) / 2;

                // Check if there is an opponent's piece to capture
                var midSquare = BoardArr[midX, midY];
                if (!midSquare.IsEmpty && midSquare.TypePiece.Color != piece.Color)
                {
                    return true; // Valid capture move
                }
            }

            return false;
        }

        public void UpdateBoard(Piece piece, int newX, int newY, Player PlayerOne, Player PlayerTwo)
        {
            // Check if we are capturing a piece
            if (Math.Abs(newX - piece.PosX) == 2 && Math.Abs(newY - piece.PosY) == 2)
            {
                int midX = (piece.PosX + newX) / 2;
                int midY = (piece.PosY + newY) / 2;

                // Get the middle square
                var midSquare = BoardArr[midX, midY];

                // Remove the captured piece using its Remove method
                midSquare.TypePiece.Remove(midSquare.TypePiece.Color == "White" ? PlayerOne : PlayerTwo);

                // Remove the piece from the board
                midSquare.RemovePiece();
            }

            // Move the current piece to the new position
            BoardArr[piece.PosX, piece.PosY].RemovePiece();
            piece.Move(newX, newY);
            BoardArr[newX, newY].PlacePiece(piece);
        }

    }

    public class Checkers
    {
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public Board Board { get; set; }
        public Player CurrentPlayer { get; set; }

        public Checkers()
        {
            PlayerOne = new Player("White");
            PlayerTwo = new Player("Black");
            Board = new Board();
            CurrentPlayer = PlayerOne; // White goes first
        }

        public void StartGame()
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = (y % 2 == 0) ? 0 : 1; x < 8; x += 2)
                {
                    Piece blackPiece = new Piece(x, y, "Black");
                    PlayerTwo.Pieces.Add(blackPiece);
                    Board.BoardArr[x, y].PlacePiece(blackPiece);
                }
            }

            for (int y = 5; y < 8; y++)
            {
                for (int x = (y % 2 == 0) ? 0 : 1; x < 8; x += 2)
                {
                    Piece whitePiece = new Piece(x, y, "White");
                    PlayerOne.Pieces.Add(whitePiece);
                    Board.BoardArr[x, y].PlacePiece(whitePiece);
                }
            }

            Board.ShowBoard();
            GameLoop();
        }

        public void GameLoop()
        {
            while (true)
            {
                Console.WriteLine($"{CurrentPlayer.Color}'s turn. Choose a piece (e.g., a1):");
                string input = Console.ReadLine();

                if (input.Length != 2 || !IsValidInput(input, out int x, out int y))
                {
                    Console.WriteLine("Invalid piece. Try again.");
                    continue;
                }

                Piece selectedPiece = CurrentPlayer.Pieces.Find(p => p.PosX == x && p.PosY == y);

                if (selectedPiece == null)
                {
                    Console.WriteLine("Invalid piece. Try again.");
                    continue;
                }

                Console.WriteLine("Choose destination (e.g., b2):");
                string destInput = Console.ReadLine();

                if (!IsValidInput(destInput, out int newX, out int newY))
                {
                    Console.WriteLine("Invalid destination. Try again.");
                    continue;
                }

                if (Board.IsMoveOk(selectedPiece, newX, newY))
                {
                    Board.UpdateBoard(selectedPiece, newX, newY, PlayerOne, PlayerTwo);
                    Board.ShowBoard();
                    if (CheckGameResult())
                    {
                        Console.WriteLine($"Game over! {CurrentPlayer.Color} wins!");
                        break;
                    }
                    SwitchPlayer();
                }
                else
                {
                    Console.WriteLine("Invalid move. Try again.");
                }
            }
        }

        private bool IsValidInput(string input, out int x, out int y)
        {
            x = -1;
            y = -1;

            if (input.Length != 2)
                return false;

            char col = input[0];
            char row = input[1];

            if (col < 'a' || col > 'h' || row < '1' || row > '8')
                return false;

            x = col - 'a'; // Convert letter to index (0-7)
            y = 8 - (row - '0'); // Convert number to index (0-7)

            return true;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == PlayerOne ? PlayerTwo : PlayerOne;
        }

        public bool CheckGameResult()
        {
            // Check for win
            if (PlayerOne.Pieces.Count == 0)
            {
                Console.WriteLine("Black wins!");
                return true;
            }
            if (PlayerTwo.Pieces.Count == 0)
            {
                Console.WriteLine("White wins!");
                return true;
            }
            // Check for draw (no valid moves)
            if (!HasValidMoves(PlayerOne) && !HasValidMoves(PlayerTwo))
            {
                Console.WriteLine("It's a draw!");
                return true;
            }
            return false;
        }

        private bool HasValidMoves(Player player)
        {
            foreach (Piece piece in player.Pieces)
            {
                for (int dx = -1; dx <= 1; dx += 2) // Check for diagonal moves
                {
                    for (int dy = -1; dy <= 1; dy += 2)
                    {
                        int newX = piece.PosX + dx;
                        int newY = piece.PosY + dy;
                        if (Board.IsMoveOk(piece, newX, newY))
                        {
                            return true; // Found a valid move
                        }
                    }
                }
            }
            return false; // No valid moves found
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Checkers checkersGame = new Checkers();
            checkersGame.StartGame();
        }
    }
}
