using System;
using System.Collections.Generic;
using System.Text;

namespace TSP
{
    public class Matrix
    {
        public Matrix(int[,] input) 
        {
            if (input != null) 
            {
                this.elems = new MatrixElem[input.GetLength(0), input.GetLength(1)];
                for (int i = 0; i != input.GetLength(0); i++) 
                {
                    for (int j = 0; j != input.GetLength(1); j++) 
                    {
                        this.elems[i, j] = new MatrixElem
                        {
                            fPoint = GetColumnPointTitle(i + 1),
                            lPoint = GetColumnPointTitle(j + 1),
                            length = input[i, j]
                        };
                    }
                }
            }
        }

        //matrix constants
        public static string pointTitle = "JABCDEFGHI";

        public MatrixElem[,] elems { get; set; }

        public class MatrixElem
        {
            public string fPoint { get; set; }
            public string lPoint { get; set; }
            public int length { get; set; }
        }

        public static int GetColumnByPointTitle(string title) 
        {
            int num = 0;
            foreach (char item in title) 
            {
                num = num * 10 + pointTitle.IndexOf(item);
            }
            return num - 1;
        }

        private static string GetColumnPointTitle(int num)
        {
            string res = "";
            while (num != 0)
            {
                res = pointTitle[num % 10] + res;
                num /= 10;
            }
            return res;
        }
    }
}
