using System;
using static TSP.Matrix;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace TSP
{
    class Program
    {
        public const string resultSeparator = "->";
        public static Stopwatch stopWatch = new Stopwatch();
        public static TimeSpan endTime = new TimeSpan(0, 0, 5, 0);

        public class Result
        {
            public bool fullFinish { get; set; }
            public string result { get; set; }
            public int cost { get; set; }
        }

        public static int[] InsertToBuffer(int[] buffer, int newVal) 
        {
            for (int i = 0; i != buffer.Length; i++) 
            {
                int temp = buffer[i];
                buffer[i] = newVal;
                newVal = temp;
            }

            return buffer;
        }

        //check if program should be stopped
        public static bool CheckTimer() 
        {
            if (stopWatch.Elapsed > endTime)
                return false;
            else
                return true;
        }

        public static Result GetLAHCResult(Matrix matrix, int[] buffer) 
        {
            Result returnData = new Result();
            returnData.fullFinish = true;

            string lastElem = "";

            //first element
            returnData.result = resultSeparator + pointTitle[1] + resultSeparator;
            returnData.cost = 0;

            bool toBe = true;
            int i = 0;
            while (toBe)
            {
                //for best choice in row - if no value is less then showed
                MatrixElem tempVal = null;
                //not to the last point
                for (int j = 0; j != matrix.elems.GetLength(1); j++)
                {
                    //check timer
                    if (!CheckTimer())
                    {
                        returnData.fullFinish = false;
                        return returnData;
                    }

                    //ignore if its the same element, as it was previous
                    //also ignore elements, that showed previously
                    if (matrix.elems[i, j].lPoint != lastElem && !returnData.result.Contains(resultSeparator + matrix.elems[i, j].lPoint + resultSeparator))
                    {
                        //add second best option
                        if ((tempVal == null || matrix.elems[i, j].length < tempVal.length))
                            tempVal = matrix.elems[i, j];

                        if (buffer[buffer.Length - 1] > matrix.elems[i, j].length)
                        {
                            tempVal = matrix.elems[i, j];
                            buffer = InsertToBuffer(buffer, matrix.elems[i, j].length);
                            break;
                        }
                        else
                        {
                            //add element to buffer
                            buffer = InsertToBuffer(buffer, matrix.elems[i, j].length);
                        }
                    }
                }

                //if no value found
                if (tempVal == null)
                {
                    break;
                }
                else
                {
                    i = GetColumnByPointTitle(tempVal.lPoint);
                    returnData.result += tempVal.lPoint + resultSeparator;
                    returnData.cost += tempVal.length;
                    lastElem = tempVal.lPoint;
                }
            }

            //for last val add
            returnData.result = returnData.result.Substring(2);
            var preLastElem = lastElem;
            int colIndex = GetColumnByPointTitle(preLastElem);

            for (int j = 0; j != matrix.elems.GetLength(1); j++)
            {
                if (matrix.elems[colIndex, j].fPoint == preLastElem && matrix.elems[colIndex, j].lPoint == "A")
                {
                    if (matrix.elems[colIndex, j].length != 0)
                    {
                        returnData.result += matrix.elems[colIndex, j].lPoint;
                        returnData.cost += matrix.elems[colIndex, j].length;

                        returnData.fullFinish = true;
                        return returnData;
                    }
                    else
                    {
                        returnData.fullFinish = false;
                        return returnData;
                    }
                }
            }

            returnData.fullFinish = false;
            return returnData;
        }

        //starting LAHC algorithm
        public static void StartLAHC(int[,] input)
        {
            Matrix matrix = new Matrix(input);

            int bufferLength = matrix.elems.GetLength(0) / (matrix.elems.GetLength(0) / 2);
            Result finalRes = null;

            //initialize buffer
            while (CheckTimer() && bufferLength <= matrix.elems.GetLength(0)) 
            {
                //upgrade buffer
                int[] buffer = new int[bufferLength];
                Array.Fill(buffer, 0);

                var lahcRes = GetLAHCResult(matrix, buffer);
                if (lahcRes.fullFinish == false) 
                {
                    Console.WriteLine("Unable to find full route");
                    if (finalRes != null) 
                    {
                        Console.WriteLine("Route: " + finalRes.result);
                        Console.WriteLine("Cost: " + finalRes.cost);
                    }
                    return;
                }
                else if (finalRes == null || finalRes.cost > lahcRes.cost)
                {
                    finalRes = lahcRes;
                }

                bufferLength += 1;
            }

            Console.WriteLine("LAHC found optimal route:");
            Console.WriteLine("Route: " + finalRes.result);
            Console.WriteLine("Cost: " + finalRes.cost);

            return;
        }

        static void Main(string[] args)
        {
            //reading text files
            Console.WriteLine("Type path to file:");
            string pathToTxt = Console.ReadLine();

            //to register program time           
            stopWatch.Start();

            if (!string.IsNullOrEmpty(pathToTxt)) 
            {
                string[] lines = File.ReadAllLines(pathToTxt);
                int[,] input = new int[lines[0].Split(' ').Length, lines.Length];

                for (int i = 0; i != lines.Length; i++) 
                {
                    string[] splitLine = lines[i].Split(' ');
                    for (int j = 0; j != splitLine.Length; j++) 
                    {
                        input[i, j] = int.Parse(splitLine[j]);
                    }
                }

                StartLAHC(input);
            }

            stopWatch.Stop();
            Console.WriteLine("Time spent: " + stopWatch.Elapsed.ToString());
        }
    }
}
