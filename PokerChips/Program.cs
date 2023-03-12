using System;
using System.Collections;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static PokerChips.Program;

namespace PokerChips
{
    internal class Program
    {
        internal static int Total;
        public class Chips
        {
            
            internal static int[] MaximumFlags(int[] Array, int ChipsPerPlayer)
            {
                int[] Maximums = new int[Array.Count()];
                int Index = 0;
                foreach (int Node in Array)
                {
                    Maximums[Index] = (Node > ChipsPerPlayer) ? 1 : 0;
                    Index++;
                }
                return Maximums;
            }
            internal static int[] MinimumFlags(int[] Array, int ChipsPerPlayer)
            {
                int[] Minimums = new int[Array.Count()];
                int Index = 0;
                foreach (int Node in Array)
                {
                    Minimums[Index] = (Node < ChipsPerPlayer) ? 1 : 0;
                    Index++;
                }
                return Minimums;
            }
            internal static int FindMaximum(int[] Array, int ChipsPerPlayer)
            {
                int Maximum = Array[0];
                foreach (int Node in Array)
                {
                    if (Node > Maximum)
                    {
                        Maximum = Node;
                    }
                }
                return Maximum;
            }
            internal static int FindMaximumIndex(int[] Array, int ChipsPerPlayer)
            {
                int Maximum = Array[0];
                int MaximumIndex = 0;
                int Index = 0;
                foreach (int Node in Array)
                {
                    if (Node > Maximum)
                    {
                        Maximum = Node;
                        MaximumIndex = Index;
                    }
                    Index++;
                }
                return MaximumIndex;
            }
            internal static int[,] FindMaximumFlow(int[] ring, int balanceChipsNumber)
            {
            int[] Minimums = Chips.MinimumFlags(ring, balanceChipsNumber);
            int[] Maximums = Chips.MaximumFlags(ring, balanceChipsNumber);
            var Rows = new List<int>();
            var Columns = new List<int>();
            int count = 0;
            foreach (int min in Minimums)
            {
                if (min == 1)
                {
                    Columns.Add(count);
                }
                count++;
            }
            count = 0;
            foreach (int max in Maximums)
            {
                if (max == 1)
                {
                    Rows.Add(count);
                }
                count++;
            }
            int[,] MaxMoves = new int[Rows.Count(), Columns.Count()];

            for (int i = 0; i < Rows.Count(); i++)
            {
                for (int j = 0; j < Columns.Count(); j++)
                {
                    MaxMoves[i, j] = Math.Min(ring[Rows[i]] - balanceChipsNumber, balanceChipsNumber - ring[Columns[j]]);

                }
            }
            return MaxMoves;

        }
            internal static int[,] FindMoves(int[] ring, int balanceChipsNumber)
        {
            int[] Minimums = Chips.MinimumFlags(ring, balanceChipsNumber);
            int[] Maximums = Chips.MaximumFlags(ring, balanceChipsNumber);
            var Rows = new List<int>();
            var Columns = new List<int>();
            int count = 0;
            foreach (int min in Minimums)
            {
                if (min == 1)
                {
                    Columns.Add(count);
                }
                count++;
            }
            count = 0;
            foreach (int max in Maximums)
            {
                if (max == 1)
                {
                    Rows.Add(count);
                }
                count++;
            }
            int[,] TotalMoves = new int[Rows.Count(), Columns.Count()];

            for (int i = 0; i < Rows.Count(); i++)
            {
                for (int j = 0; j < Columns.Count(); j++)
                {
                    int Right = Math.Abs(Rows[i] - Columns[j]);
                    int Left = ring.Count() - Right;
                    TotalMoves[i, j] = Math.Min(Left, Right);
                }
            }
            return TotalMoves;
        }

            internal static int[] MoveLeft(int[] Array, int Home, int Destination, int Moving)
            {
                int Moves = 0;
                int ChipsCount = 0;
                int CW = Math.Abs(Home - Destination);
                int CCW = Array.Count() - CW;
                int Way = Math.Min(CW, CCW);

                while (ChipsCount < Moving)
                {
                    ChipsCount++;
                    Array[Destination]++;
                    Array[Home]--;
                    Moves += Way;
                    Total += Way;
                }
                //Раскомментировать следующие строки для отображения процесса:
                //    Console.WriteLine("\t Из позиции " + Home + " (" + ChipsCount + " ф.)  <-  на " + Way + " за " + Moves + " Перемещений");
                //    Console.WriteLine("\tРезультат передвижений: " + string.Join(", ", Array) + "\n");
                return Array;
            }
            internal static int[] MoveRight(int[] Array, int Home, int Destination, int Moving)
            {
                int Moves = 0;
                int ChipsCount = 0;
                int CW = Math.Abs(Home - Destination);
                int CCW = Array.Count() - CW;
                int Way = Math.Min(CW, CCW);
                while (ChipsCount < Moving)
                {
                    ChipsCount++;
                    Array[Destination]++;
                    Array[Home]--;
                    Moves += Way;
                    Total += Way;
                }
                //Раскомментировать следующие строки для отображения процесса:
                //    Console.WriteLine("\t Из позиции " + Home + " (" + ChipsCount + " ф.)  ->  на " + Way + " за " + Moves + " Перемещений");
                //    Console.WriteLine("\tРезультат передвижений: " + string.Join(", ", Array) + "\n");
                return Array;
            }
        }

        
        static void Main(string[] args)
        {
           
            #region Ввод данных в консоль
            Console.WriteLine("Введите последовательность данных через запятую и нажмите клавишу Enter:");
            int[] Ring = { };
            int BalanceChipsNumber = 0;

            while (Ring.Count() < 1)
            {
                try
                {
                    Ring = Console.ReadLine().Replace(" ", "").Split(',').Select(element => int.Parse(element)).ToArray();
                    if (Ring.Sum() % Ring.Count() != 0)
                    {
                        Console.WriteLine("Сумма фишек указанных при вводе не делится нацело на игроков. Введите корректные значения.");
                        Ring = new int[0];
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Данные на входе имели не корректный формат, повторите ввод.");
                }
            }
            int N = Ring.Count();
            BalanceChipsNumber = Ring.Sum() / N;
            #endregion
            int[] StartRing = (int[])Ring.Clone();
            int[] Minimums = new int[N];
            int[] Maximums = new int[N];
            int[,] MaxFlowArray = new int[Maximums.Sum(), Minimums.Sum()];
            int[,] TotalCosts = new int[Maximums.Sum(), Minimums.Sum()];

            int MaxIndex = 0;
            int MaxChips = Ring[0];
            int FlowChip = BalanceChipsNumber;
            var CurrentFlows = new List<int>();
            var Rows = new List<int>();
            var Columns = new List<int>();
            int count = 0;
            void Iniciate(int[] ring, int balanceChipsNumber)
            {
                Minimums = Chips.MinimumFlags(ring, balanceChipsNumber);
                Maximums = Chips.MaximumFlags(ring, balanceChipsNumber);
                MaxChips = Chips.FindMaximum(ring, balanceChipsNumber);
                MaxIndex = Chips.FindMaximumIndex(ring, balanceChipsNumber);

                MaxFlowArray = Chips.FindMaximumFlow(ring, balanceChipsNumber);
                TotalCosts = Chips.FindMoves(ring, balanceChipsNumber);
            }
            void MiniMax(int[] min, int[] max)
            {
                foreach (int Min in Minimums)
                {
                    if (Min == 1)
                    {
                        Columns.Add(count);
                    }
                    count++;
                }
                count = 0;
                foreach (int Max in Maximums)
                {
                    if (Max == 1)
                    {
                        Rows.Add(count);
                    }
                    count++;
                }
            }

            Iniciate(Ring, BalanceChipsNumber);
            Console.WriteLine("\tPlayers - " + N);
            Console.WriteLine("\tBalanceChips - " + BalanceChipsNumber);
            MiniMax(Minimums, Maximums);

            Console.WriteLine("\n\n");
            Console.WriteLine("\tИдёт подсчёт передвижений...\n");
            int Cost = 1;
            int MinMoves = int.MaxValue;
            int MinFlow = int.MaxValue;
            while (Cost <= Ring.Count() / 2)
            {
                Console.WriteLine("\tFlow\n");
                for (int i = 0; i < Maximums.Sum(); i++)
                {
                    for (int j = 0; j < Minimums.Sum(); j++)
                    {
                        Console.Write("{0,3}", (MaxFlowArray.GetValue(i, j)).ToString());
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("\tCost\n");
                for (int i = 0; i < Maximums.Sum(); i++)
                {
                    for (int j = 0; j < Minimums.Sum(); j++)
                    {
                        Console.Write("{0,3}", (TotalCosts[i, j]).ToString());
                    }
                    Console.WriteLine();
                }
                int FromCell = int.MaxValue, ToCell = int.MaxValue;
                int From = 0, To = 0;
                int CW = 0;
                int CCW = 0;
                for (int i = 0; i < Rows.Count(); i++)
                {
                    for (int j = 0; j < Columns.Count(); j++)
                    {
                        if ((MaxFlowArray[i, j] * TotalCosts[i, j] <= MinMoves))
                        {
                            MinFlow= MaxFlowArray[i, j];
                            MinMoves = MaxFlowArray[i, j] * TotalCosts[i, j];
                            FromCell = Rows[i];
                            ToCell = Columns[j];
                            From = i;
                            To= j;
                        }
                        if ((MaxFlowArray[i, j] * TotalCosts[i, j] <= MinMoves) && Rows[From] - Columns[To] > Rows[i] - Columns[j])
                        {
                            MinFlow = MaxFlowArray[i, j];
                            MinMoves = MaxFlowArray[i, j] * TotalCosts[i, j];
                            FromCell = Rows[i];
                            ToCell = Columns[j];
                        }
                    }
                }
                //вычисляем кратчайшее расстояние
                if (FromCell > ToCell)
                {
                    CCW = Math.Abs(FromCell - ToCell);
                    CW = Ring.Count() - CCW;
                }
                else
                {
                    CW = Math.Abs(FromCell - ToCell);
                    CCW = Ring.Count() - CW;
                }
                if (FromCell == int.MaxValue && ToCell == int.MaxValue) { Cost++; } // следующая величина потока
                // сдвигаем фишки
                if (CW != 0 && CCW != 0)
                {
                    if (CCW < CW)
                    {
                        Ring = Chips.MoveLeft(Ring, FromCell, ToCell, MinFlow);
                    }
                    else
                    {
                        Ring = Chips.MoveRight(Ring, FromCell, ToCell, MinFlow);
                    }
                }
                #region Refresh
                From = 0; To = 0;
                MinFlow = int.MaxValue;
                MinMoves = int.MaxValue;
                Columns.Clear();
                Rows.Clear();
                count = 0;
                Iniciate(Ring, BalanceChipsNumber);
                MiniMax(Minimums, Maximums);
                #endregion
            }
            //Console.Clear();
            Console.WriteLine("\tИсходный набор: " + string.Join(", ", StartRing) + "\n");
            Console.WriteLine("\tРезультат передвижений: " + string.Join(", ", Ring) + "\n");
            Console.WriteLine("\tВсего передвижений - " + Total);
            Console.ReadKey();
        }
    }
}
    
    