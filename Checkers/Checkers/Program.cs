using System;

namespace CheckersGame
{
    class Program
    {
        static char[,] board = new char[8, 8];
        static void Main(string[] args)
        {
            InitializeBoard();
            bool isPlayerOneTurn = true;
            while (true)
            {
                Console.Clear();
                PrintBoard();
                Console.WriteLine(isPlayerOneTurn ? "Player 1's turn (X)" : "Player 2's turn (O)");
                Console.Write("Enter your move (e.g., A3 B4): ");
                string move = Console.ReadLine();
                if (move.ToLower() == "q") break;

                if (IsValidMove(move, isPlayerOneTurn))
                {
                    MakeMove(move);
                    isPlayerOneTurn = !isPlayerOneTurn;
                }
                else
                {
                    Console.WriteLine("Invalid move. Try again.");
                    Console.ReadKey();
                }
            }
        }

        static void InitializeBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i % 2 != j % 2 && i < 3)
                        board[i, j] = 'X';
                    else if (i % 2 != j % 2 && i > 4)
                        board[i, j] = 'O';
                    else
                        board[i, j] = '.';
                }
            }
        }

        static void PrintBoard()
        {
            Console.WriteLine("  A B C D E F G H");
            for (int i = 0; i < 8; i++)
            {
                Console.Write(i + 1 + " ");
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        static bool IsValidMove(string move, bool isPlayerOneTurn)
        {
            // Basic validation logic for moves
            // This should be expanded to include all checkers rules
            string[] parts = move.Split(' ');
            if (parts.Length != 2) return false;

            int startX = parts[0][0] - 'A';
            int startY = parts[0][1] - '1';
            int endX = parts[1][0] - 'A';
            int endY = parts[1][1] - '1';

            if (startX < 0 || startX >= 8 || startY < 0 || startY >= 8 ||
                endX < 0 || endX >= 8 || endY < 0 || endY >= 8)
                return false;

            char playerPiece = isPlayerOneTurn ? 'X' : 'O';
            if (board[startY, startX] != playerPiece || board[endY, endX] != '.')
                return false;

            // Add more rules for valid moves here

            return true;
        }

        static void MakeMove(string move)
        {
            string[] parts = move.Split(' ');
            int startX = parts[0][0] - 'A';
            int startY = parts[0][1] - '1';
            int endX = parts[1][0] - 'A';
            int endY = parts[1][1] - '1';

            board[endY, endX] = board[startY, startX];
            board[startY, startX] = '.';
        }
    }
}
