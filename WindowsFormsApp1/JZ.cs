using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class JZ
    {
        private int numColumns = 0;// 矩阵列数         
        private int numRows = 0;// 矩阵行数         
        private double eps = 0.0;
        private double[] elements = null;

        public int Rows //矩阵行数 
        {
            get
            {
                return numRows;
            }
        }
        public int Columns //矩阵列数       
        {
            get
            {
                return numColumns;
            }
        }

        public double Eps
        {
            get
            {
                return eps;
            }
            set
            {
                eps = value;
            }
        }



        //基本构造函数    
        public JZ()
        {
            numColumns = 1;
            numRows = 1;
            Init(numRows, numColumns);
        }

        public JZ(int row, int col)
        {
            numColumns = col;
            numRows = row;
            Init(numRows, numColumns);
        }

        public JZ(JZ other)
        {
            numColumns = other.GetNumColumns();
            numRows = other.GetNumRows();
            Init(numRows, numColumns);
        }


        //索引器
        public double this[int row, int col]
        {
            get
            {
                return elements[col + row * numColumns];
            }
            set
            {
                elements[col + row * numColumns] = value;
            }
        }


        //初始化  
        public bool Init(int nRows, int nCols)
        {
            numRows = nRows;
            numColumns = nCols;
            int nSize = nCols * nRows;
            if (nSize < 0)
                return false;
            elements = new double[nSize];
            return true;
        }


        public int GetNumRows() //获得行
        {
            return numRows;
        }

        public int GetNumColumns() //获得列
        {
            return numColumns;
        }

        public void SetElement(int row, int col, double val) //设置元素
        {
            elements[col + row * numColumns] = val;
        }

        public double GetElement(int row, int col) //获得元素
        {
            return elements[col + row * numColumns];
        }


        public static JZ operator +(JZ m1, JZ m2)//加号重载
        {
            return m1.Add(m2);
        }

        public static JZ operator -(JZ m1, JZ m2)//减号重载
        {
            return m1.Subtract(m2);
        }


        public static JZ operator *(JZ m1, JZ m2)//矩阵乘矩阵重载
        {
            return m1.Multiply(m2);
        }


        public JZ Add(JZ other)//矩阵加法
        {    // 检查行列数是否相等    
            if (numColumns != other.GetNumColumns() || numRows != other.GetNumRows())
                throw new Exception("矩阵的行/列数不匹配。");
            // 构造结果矩阵    
            JZ result = new JZ(this);
            // 进行加法 
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numColumns; ++j)
                    result.SetElement(i, j, GetElement(i, j) + other.GetElement(i, j));
            }
            return result;
        }



        public JZ Subtract(JZ other)//矩阵减法
        {
            if (numColumns != other.GetNumColumns() || numRows != other.GetNumRows())
                throw new Exception("矩阵的行/列数不匹配。");
            // 构造结果矩阵    
            JZ result = new JZ(this);
            // 进行减法    
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numColumns; ++j)
                    result.SetElement(i, j, GetElement(i, j) - other.GetElement(i, j));
            }
            return result;
        }


        public JZ Transpose()   //转制
        {    // 构造目标矩阵    
            JZ Trans = new JZ(numColumns, numRows);

            // 转置各元素    
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numColumns; ++j)
                    Trans.SetElement(j, i, GetElement(i, j));
            }

            return Trans;
        }

        public JZ Multiply(double val)     //数乘矩阵
        {    // 构造目标矩阵    
            JZ result = new JZ(this);  // copy ourselves       
            // 进行数乘    
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numColumns; ++j)
                    result.SetElement(i, j, GetElement(i, j) * val);
            }
            return result;
        }

        public JZ Multiply(JZ other)   //矩阵乘法
        {    // 检查行列数是否符合要求    
            if (numColumns != other.GetNumRows())
                throw new Exception("矩阵的行/列数不匹配。");

            JZ result = new JZ(numRows, other.GetNumColumns());
            double value;
            for (int i = 0; i < result.GetNumRows(); ++i)
            {
                for (int j = 0; j < other.GetNumColumns(); ++j)
                {
                    value = 0.0;
                    for (int k = 0; k < numColumns; ++k)
                    {
                        value += GetElement(i, k) * other.GetElement(k, j);
                    }

                    result.SetElement(i, j, value);
                }
            }

            return result;
        }


        public JZ InvertGaussJordan()  //矩阵的求逆
        {
            int i, j, k, l, u, v;
            double d = 0.0, p = 0.0;
            int[] pnRow = new int[numColumns];
            int[] pnCol = new int[numColumns];
            {
                for (k = 0; k <= numColumns - 1; k++)
                {
                    d = 0.0;
                    for (i = k; i <= numColumns - 1; i++)
                    {
                        for (j = k; j <= numColumns - 1; j++)
                        {
                            l = i * numColumns + j; p = Math.Abs(elements[l]);
                            if (p > d)
                            {
                                d = p;
                                pnRow[k] = i;
                                pnCol[k] = j;
                            }
                        }
                    }
                    if (d == 0.0)
                    {
                        throw new Exception("矩阵不可逆。");
                    }
                    if (pnRow[k] != k)
                    {
                        for (j = 0; j <= numColumns - 1; j++)
                        {
                            u = k * numColumns + j;
                            v = pnRow[k] * numColumns + j;
                            p = elements[u];
                            elements[u] = elements[v];
                            elements[v] = p;
                        }
                    }
                    if (pnCol[k] != k)
                    {
                        for (i = 0; i <= numColumns - 1; i++)
                        {
                            u = i * numColumns + k;
                            v = i * numColumns + pnCol[k];
                            p = elements[u];
                            elements[u] = elements[v];
                            elements[v] = p;
                        }
                    }
                    l = k * numColumns + k;
                    elements[l] = 1.0 / elements[l];
                    for (j = 0; j <= numColumns - 1; j++)
                    {
                        if (j != k)
                        {
                            u = k * numColumns + j;
                            elements[u] = elements[u] * elements[l];
                        }
                    }
                    for (i = 0; i <= numColumns - 1; i++)
                    {
                        if (i != k)
                        {
                            for (j = 0; j <= numColumns - 1; j++)
                            {
                                if (j != k)
                                {
                                    u = i * numColumns + j;
                                    elements[u] = elements[u] - elements[i * numColumns + k] * elements[k * numColumns + j];
                                }
                            }
                        }
                    }
                    for (i = 0; i <= numColumns - 1; i++)
                    {
                        if (i != k)
                        {
                            u = i * numColumns + k;
                            elements[u] = -elements[u] * elements[l];
                        }
                    }
                }
                for (k = numColumns - 1; k >= 0; k--)
                {
                    if (pnCol[k] != k)
                    {
                        for (j = 0; j <= numColumns - 1; j++)
                        {
                            u = k * numColumns + j;
                            v = pnCol[k] * numColumns + j;
                            p = elements[u];
                            elements[u] = elements[v];
                            elements[v] = p;
                        }
                    }
                    if (pnRow[k] != k)
                    {
                        for (i = 0; i <= numColumns - 1; i++)
                        {
                            u = i * numColumns + k;
                            v = i * numColumns + pnRow[k];
                            p = elements[u];
                            elements[u] = elements[v];
                            elements[v] = p;
                        }
                    }
                }
            }
            JZ result = new JZ(numRows, numColumns);
            for (int a = 0; a < numRows; ++a)
            {
                for (int b = 0; b < numColumns; ++b)
                {
                    result.SetElement(a, b, GetElement(a, b));
                }

            }
            return result;
        }
    }
}
