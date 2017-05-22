using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.ComputerOpponent {
   /// <summary>
   /// A pair of an IGameMove that was the best move to apply for a given board state,
   /// and the Weight of the board that resulted.
   /// </summary>
   internal struct MinimaxBestMove {
      public int Weight { get; set; }
      public IGameMove Move { get; set; }
   }

   /// <summary>
   /// A minimax with alpha-beta pruning implementation of IGameAi.
   /// </summary>
   public class MinimaxAi : IGameAi {
      private int mMaxDepth;
      public MinimaxAi(int maxDepth) {
         mMaxDepth = maxDepth;
      }

      // The public calls this function, which kicks off the minimax search.
      public IGameMove FindBestMove(IGameBoard b) {
         // TODO: call the private FindBestMove with appropriate values for the parameters.
         // mMaxDepth is what the depthLeft should start at.
         // You are maximizing iff the board's current player is 1.

         var move = FindBestMove(b, mMaxDepth, (b.CurrentPlayer == 1));

         return null;
      }

      private static MinimaxBestMove FindBestMove(IGameBoard b, int depthLeft, bool maximize) {
         // Implement the minimax algorithm. 
         // Your first attempt will not use alpha-beta pruning. Once that works, 
         // implement the pruning as discussed in the project notes.
         MinimaxBestMove move = new MinimaxBestMove();
         move.Weight = b.Weight;
         move.Move = null;

         if (depthLeft == 0 || b.IsFinished) {
            return move;
         }

         int bestWeight = int.MaxValue;
         IGameMove bestMove = null;
         if (maximize)
            bestWeight *= -1;

         var possMoves = b.GetPossibleMoves();
         foreach (var possMove in possMoves) {
            b.ApplyMove(possMove);
            var w = FindBestMove(b, depthLeft - 1, !maximize);
            b.UndoLastMove();

            if (maximize && w.Weight < bestWeight) {
               bestWeight = w.Weight;
               bestMove = possMove;
            }

            else if (!maximize && w.Weight < bestWeight) {
               bestWeight = w.Weight;
               bestMove = possMove;
            }

            move.Weight = bestWeight;
            move.Move = bestMove;

            return move;
         }




         throw new NotImplementedException();
      }

   }
}
