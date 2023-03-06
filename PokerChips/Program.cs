using System;
using System.Collections;
using System.Collections.Generic;
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
        public class Chips
        {
            public static int[] MaximumFlags(int[] Array, int ChipsPerPlayer)
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
            public static int[] MinimumFlags(int[] Array, int ChipsPerPlayer)
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
            public static int FindMaximum (int[] Array, int ChipsPerPlayer)
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
            public static int FindMaximumIndex(int[] Array, int ChipsPerPlayer)
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

            public static (int[] NewRing, int Moves) MoveLeft(int[] Array, int Index, int maxIndex, int Radius, int ChipsPerPlayer)
            {
                int Moves = 0;
                int ChipsNeeded = ChipsPerPlayer - Array[Index];
                int ChipsAvaliable = Array[maxIndex] - ChipsPerPlayer;
                int MovingChipsNumber = (ChipsNeeded > ChipsAvaliable) ? ChipsAvaliable : ChipsNeeded;
                if (MovingChipsNumber==0) { return (Array, Moves); } 
                else 
                {
                    while (Array[Index] < ChipsPerPlayer && Array[maxIndex] > ChipsPerPlayer)
                    {
                        Array[maxIndex]--;
                        Array[Index]++;
                        Moves += Radius;
                    }
                    //Раскомментировать следующие строки для отображения процесса:
                    //Console.WriteLine("\t Из позиции " + maxIndex + " (" + MovingChipsNumber + " ф.)  <-  на " + Radius + " за " + Moves + " Перемещений");
                    //Console.WriteLine("\tРезультат передвижений: " + string.Join(", ", Array) + "\n");
                    return (Array, Moves);
                }
                
            }
            public static (int[] NewRing, int Moves) MoveRight(int[] Array,int Index,int maxIndex, int Radius, int ChipsPerPlayer)
            {
                int Moves = 0;
                int ChipsNeeded = ChipsPerPlayer - Array[Index];
                int ChipsAvaliable = Array[maxIndex] - ChipsPerPlayer;
                int MovingChipsNumber = (ChipsNeeded > ChipsAvaliable) ? ChipsAvaliable : ChipsNeeded;
                if (MovingChipsNumber == 0) { return (Array, Moves); }
                else
                {
                    while (Array[Index] < ChipsPerPlayer && Array[maxIndex] > ChipsPerPlayer)
                    {
                        Array[maxIndex]--;
                        Array[Index]++;
                        Moves += Radius;
                    }
                    //Раскомментировать следующие строки для отображения процесса:
                    //Console.WriteLine("\t Из позиции " + maxIndex + " (" + MovingChipsNumber + " ф.)  ->  на " + Radius + " за " + Moves + " Перемещений");
                    //Console.WriteLine("\tРезультат передвижений: " + string.Join(", ", Array) + "\n");
                    return (Array, Moves);
                }
            }
        }
    
    static void Main(string[] args)
        {
            int TotalMoves=0;
            int SearchRadius=1;
            int LeftIndex=0, RightIndex=0;

            #region Ввод данных в консоль
            Console.WriteLine("Введите последовательность данных через запятую и нажмите клавишу Enter:");
            int[] Ring= { };
            int BalanceChipsNumber=0;

            while (Ring.Count()<1)
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

            Console.WriteLine("\tN - " + N);
            Console.WriteLine("\tChips - " + BalanceChipsNumber);
            int[] StartRing = (int[])Ring.Clone();
            int[] Minimums= new int[N];
            int[] Maximums = new int[N];
            int MaxIndex = 0;
            int MaxChips = Ring[0];

            Minimums = Chips.MinimumFlags(Ring, BalanceChipsNumber);
            Maximums = Chips.MaximumFlags(Ring, BalanceChipsNumber);
            MaxChips = Chips.FindMaximum(Ring, BalanceChipsNumber);
            MaxIndex = Chips.FindMaximumIndex(Ring, BalanceChipsNumber);

            Console.WriteLine("\tИдёт подсчёт передвижений...\n");
            Thread thread = new Thread(() =>
            {
                while (MaxChips != BalanceChipsNumber)
                {

                    LeftIndex = (MaxIndex - SearchRadius < 0) ? Ring.Count() - SearchRadius : MaxIndex - SearchRadius;
                    RightIndex = (MaxIndex + SearchRadius > Ring.Count() - 1) ? 0 : MaxIndex + SearchRadius;

                    if (Minimums[LeftIndex] == 1)
                    {
                        TotalMoves += Chips.MoveLeft(Ring, LeftIndex, MaxIndex, SearchRadius, BalanceChipsNumber).Moves;
                        Ring = Chips.MoveLeft(Ring, LeftIndex, MaxIndex, SearchRadius, BalanceChipsNumber).NewRing;
                        MaxChips = 0;
                        MaxIndex = 0;
                        SearchRadius = 1;
                    }
                    else if (Minimums[RightIndex] == 1)
                    {
                        TotalMoves += Chips.MoveRight(Ring, RightIndex, MaxIndex, SearchRadius, BalanceChipsNumber).Moves;
                        Ring = Chips.MoveRight(Ring, RightIndex, MaxIndex, SearchRadius, BalanceChipsNumber).NewRing;
                        MaxChips = 0;
                        MaxIndex = 0;
                        SearchRadius = 1;
                    }
                    else
                    {
                        SearchRadius++;
                    }
                    Minimums = Chips.MinimumFlags(Ring, BalanceChipsNumber);
                    Maximums = Chips.MaximumFlags(Ring, BalanceChipsNumber);
                    MaxChips = Chips.FindMaximum(Ring, BalanceChipsNumber);
                    MaxIndex = Chips.FindMaximumIndex(Ring, BalanceChipsNumber);
                }
            });
            thread.Start();
            thread.Join();
            Console.Clear();
            Console.WriteLine("\tИсходный набор: " + string.Join(", ", StartRing) + "\n");
            Console.WriteLine("\tРезультат передвижений: " + string.Join(", ", Ring) + "\n");
            Console.WriteLine("\tВсего передвижений - " + TotalMoves);
            Console.ReadKey();
        }
       
    }
}

