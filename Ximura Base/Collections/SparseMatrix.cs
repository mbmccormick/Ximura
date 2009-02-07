#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Collections;

using XIMS;
#endregion // using
namespace XIMS.Helper.Collections
{
	///// <summary>
	///// Summary description for SparseMatrix. This has been remmed out as noone is using it.
	///// </summary>
	//public class SparseMatrix : Matrix, IList
	//{
	//    protected int dimensions = 1;
	//    protected Hashtable hashtable;
	//    protected int[] lowerBounds, upperBounds;

	//    public SparseMatrix()
	//    {
	//        hashtable = new Hashtable();
	//        lowerBounds = new int[dimensions];
	//        upperBounds = new int[dimensions];
	//    }
		
	//    public SparseMatrix(int dimensions)
	//    {
	//        this.dimensions = dimensions;
	//        hashtable = new Hashtable();
	//        lowerBounds = new int[dimensions];
	//        upperBounds = new int[dimensions];
	//    }

	//    protected string IndexToHash(int[] indices)
	//    {
	//        if (indices.Length != dimensions)
	//            throw new ArgumentException("The number of indices must match the number of dimensions");

	//        System.Text.StringBuilder sb = new System.Text.StringBuilder();
	//        for (int i = 0; i < indices.Length; i++)
	//        {
	//            sb.Append(indices[i].ToString());
	//            if (i < (indices.Length-1))
	//                sb.Append(',');
	//        }
	//        return sb.ToString();
	//    }

	//    protected int[] HashToIndex(string hash)
	//    {
	//        string[] subs = hash.Split(',');
	//        if (subs.Length != dimensions)
	//            throw new ArgumentException("The number of indices must match the number of dimensions");

	//        int[] ret = new int[dimensions];
	//        for (int i = 0; i < dimensions; i++)
	//            ret[i] = int.Parse(subs[i]);

	//        return ret;
	//    }

	//    public bool IsFixedSize { get { return false; } }
	//    public bool IsReadOnly { get { return false; } }
	//    public bool IsSynchronized { get { return false; } }
	//    public int Count { get { return hashtable.Count; } }
	//    public int Rank { get { return dimensions; } }
	//    public object SyncRoot { get { return null; } }

	//    public void CopyTo(Array array, int index)
	//    {
	//        throw new NotImplementedException();
	//    }

	//    public int GetLowerBound(int dimension)
	//    {
	//        if (dimension > dimensions)
	//            throw new ArgumentOutOfRangeException("dimension");
	//        return lowerBounds[dimension];
	//    }

	//    public int GetUpperBound(int dimension)
	//    {
	//        if (dimension > dimensions)
	//            throw new ArgumentOutOfRangeException("dimension");
	//        return upperBounds[dimension];
	//    }

	//    public object GetValue(int[] indices)
	//    {
	//        string key = IndexToHash(indices);
	//        if (hashtable.Contains(key))
	//            return hashtable[key];
	//        return null;
	//    }

	//    public object GetValue(int index)
	//    {
	//        return GetValue(new int[] { index });
	//    }

	//    public object GetValue(int index1, int index2)
	//    {
	//        return GetValue(new int[] { index1, index2 });
	//    }

	//    public void SetValue(object value, int[] indices)
	//    {
	//        hashtable.Add(IndexToHash(indices), value);
	//        for (int i = 0; i < dimensions; i++)
	//        {
	//            if (lowerBounds[i] > indices[i])
	//                lowerBounds[i] = indices[i];
	//            if (upperBounds[i] < indices[i])
	//                upperBounds[i] = indices[i];
	//        }
	//    }

	//    public void SetValue(object value, int index)
	//    {
	//        SetValue(value, new int[] { index });
	//    }

	//    public void SetValue(object value, int index1, int index2)
	//    {
	//        SetValue(value, new int[] { index1, index2 });
	//    }

	//    private class SparseMatrixEnumerator : IEnumerator
	//    {
	//        private IDictionaryEnumerator dict;
	//        private SparseMatrix parent;
	//        public SparseMatrixEnumerator(SparseMatrix array) { parent = array; dict = array.hashtable.GetEnumerator(); }
	//        public void Reset() { dict.Reset(); }
	//        public bool MoveNext() { return dict.MoveNext(); }
	//        public object Current { get { return dict.Value; } }
	//        public int[] Index { get { return parent.HashToIndex((string)dict.Key); } }
	//    }

	//    public System.Collections.IEnumerator GetEnumerator()
	//    {
	//        return new SparseMatrixEnumerator(this);
	//    }

	//    public void RemoveAt(int index)
	//    {
	//        throw new NotImplementedException();
	//    }

	//    public void Insert(int index, object value)
	//    {
	//        throw new NotImplementedException();
	//    }

	//    public void Remove(object value)
	//    {
	//        throw new NotImplementedException();
	//    }

	//    public bool Contains(object value)
	//    {
	//        return hashtable.ContainsValue(value);
	//    }

	//    public void Clear()
	//    {
	//        hashtable.Clear();
	//    }

	//    public int IndexOf(object value)
	//    {
	//        if (dimensions != 1)
	//            throw new RankException();
	//        return 0;
	//    }

	//    public int Add(object value)
	//    {
	//        throw new NotImplementedException();
	//    }

	//    public object this[int[] indicies]
	//    {
	//        get { return GetValue(indicies); } 
	//        set { SetValue(value, indicies); }
	//    }

	//    public object this[int index]
	//    {
	//        get { return GetValue(index); } 
	//        set { SetValue(value, index); }
	//    }

	//    public object this[int index1, int index2]
	//    {
	//        get { return GetValue(index1, index2); } 
	//        set { SetValue(value, index1, index2); }
	//    }

	//}


	///// <summary>
	///// This class is used to perform matrix operations.
	///// </summary>
	//public class DenseMatrix : Matrix
	//{
	//    public double[,] Data;

	//    public DenseMatrix(double[,] data)
	//    {
	//        if(data==null)
	//        {
	//            throw new System.Exception("Cannot create a matrix");  
	//        }
	//        Data=data;			
	//    }

	//    public static DenseMatrix operator+ (DenseMatrix M1, DenseMatrix M2)
	//    {
	//        int r1 = M1.Data.GetLength(0);int r2 = M2.Data.GetLength(0);  
	//        int c1 = M1.Data.GetLength(1);int c2 = M2.Data.GetLength(1);  
	//        if ((r1!=r2)||(c1!=c2))
	//        {
	//            throw new System.Exception("Matrix dimensions donot agree");  
	//        }
	//        double[,] res = new double[r1,c1]; 
	//        for (int i=0;i<r1;i++)
	//        {
	//            for (int j=0;j<c1;j++)
	//            {
	//                res[i,j]=M1.Data[i,j]+M2.Data[i,j];				
	//            }		
	//        }
	//        return new DenseMatrix(res);	
	//    }

	//    public static DenseMatrix operator- (DenseMatrix M1, DenseMatrix M2)
	//    {
	//        int r1 = M1.Data.GetLength(0);int r2 = M2.Data.GetLength(0);  
	//        int c1 = M1.Data.GetLength(1);int c2 = M2.Data.GetLength(1);  
	//        if ((r1!=r2)||(c1!=c2))
	//        {
	//            throw new System.Exception("Matrix dimensions donot agree");  
	//        }
	//        double[,] res = new double[r1,c1]; 
	//        for (int i=0;i<r1;i++)
	//        {
	//            for (int j=0;j<c1;j++)
	//            {
	//                res[i,j]=M1.Data[i,j]-M2.Data[i,j];				
	//            }		
	//        }
	//        return new DenseMatrix(res);	
	//    }

	//    public static DenseMatrix operator* (DenseMatrix M1, DenseMatrix M2)
	//    {
	//        int r1 = M1.Data.GetLength(0);int r2 = M2.Data.GetLength(0);  
	//        int c1 = M1.Data.GetLength(1);int c2 = M2.Data.GetLength(1);  
	//        if (c1!=r2)
	//        {
	//            throw new System.Exception("Matrix dimensions donot agree");  
	//        }
	//        double[,] res = new double[r1,c2]; 
	//        for (int i=0;i<r1;i++)
	//        {
	//            for(int j=0;j<c2;j++)
	//            {
	//                for(int k=0;k<r2;k++)						
	//                {
	//                    res[i,j]=  res[i,j] + (M1.Data[i,k]*M2.Data[k,j]);
	//                }
	//            }			
	//        }
	//        return new DenseMatrix(res);				
	//    }

	//    public static DenseMatrix operator/ (double i,DenseMatrix M)
	//    {
	//        return new DenseMatrix(scalmul(i,INV(M.Data)));		
	//    }
		
	//    public static bool operator== (DenseMatrix M1, DenseMatrix M2)
	//    {
	//        bool B=true;
	//        int r1 = M1.Data.GetLength(0);int r2 = M2.Data.GetLength(0);  
	//        int c1 = M1.Data.GetLength(1);int c2 = M2.Data.GetLength(1);  
	//        if ((r1!=r2)||(c1!=c2))
	//        {
	//            return false;
	//        }
	//        else
	//        {
	//            for (int i=0;i<r1;i++)
	//            {
	//                for (int j=0;j<c1;j++)
	//                {
	//                    if(M1.Data[i,j]!=M2.Data[i,j])
	//                        B=false;
	//                }		
	//            }		
	//        }
	//        return B;
	//    }

	//    public static bool operator!= (DenseMatrix M1, DenseMatrix M2)
	//    {
	//        return !(M1==M2);		
	//    }

	//    public override bool Equals(object obj)
	//    {
	//        if (!(obj is Matrix))
	//        {
	//            return false;
	//        }
	//        return this==(Matrix)obj;
	//    }	

	//    public void display()
	//    {
	//        int r1 = this.Data.GetLength(0);int c1 = this.Data.GetLength(1);
	//        for (int i=0;i<r1;i++)
	//        {
	//            for (int j=0;j<c1;j++)
	//            {
	//                Console.Write(this.Data[i,j].ToString("N2")+"   " );				
	//            }
	//            Console.WriteLine(); 
	//        }
	//        Console.WriteLine(); 
	//    }

		
	//    static double[,] INV (double[,] a )
	//    {
	//        int ro = a.GetLength(0);
	//        int co = a.GetLength(1);
	//        try
	//        {
	//            if (ro!=co)	{throw new System.Exception();}
	//        }
	//        catch{Console.WriteLine("Cannot find inverse for an non square matrix");}
			
	//        int q;double[,] b = new double[ro,co];double[,] I = eyes(ro);
	//        for(int p=0;p<ro;p++){for(q=0;q<co;q++){b[p,q]=a[p,q];}}			
	//        int i;double det=1;	
	//        if (a[0,0]==0)
	//        {
	//            i=1;
	//            while (i<ro)
	//            {
	//                if (a[i,0]!=0)
	//                {
	//                    DenseMatrix.interrow(a,0,i);		
	//                    DenseMatrix.interrow(I,0,i);
	//                    det *= -1;
	//                    break;
	//                }
	//                i++;
	//            }			
	//        }
	//        det*= a[0,0];
	//        DenseMatrix.rowdiv(I,0,a[0,0]);
	//        DenseMatrix.rowdiv(a,0,a[0,0]);
	//        for (int p=1;p<ro;p++)
	//        {
	//            q=0;
	//            while(q<p)
	//            {
	//                DenseMatrix.rowsub(I,p,q,a[p,q]);
	//                DenseMatrix.rowsub(a,p,q,a[p,q]);
	//                q++;
	//            }
	//            if(a[p,p]!=0)
	//            {
	//                det*=a[p,p];
	//                DenseMatrix.rowdiv (I,p,a[p,p]); 
	//                DenseMatrix.rowdiv (a,p,a[p,p]); 
	//            }
	//            if(a[p,p]==0)
	//            {
	//                for(int j=p+1;j<co;j++)
	//                {
	//                    if(a[p,j]!=0)
	//                    {
	//                        throw new System.Exception("Unable to deteremine the Inverse");  							
	//                    }
	//                }
		
	//            }
	//        }

	//        for (int p=ro-1;p>0;p--)
	//        {
	//            for(q=p-1;q>=0;q--)
	//            {
	//                DenseMatrix.rowsub (I,q,p,a[q,p]);
	//                DenseMatrix.rowsub (a,q,p,a[q,p]);
	//            }
	//        }
						
	//        for(int p=0;p<ro;p++)
	//        {
	//            for(q=0;q<co;q++)
	//            {
	//                a[p,q]=b[p,q];
	//            }
	//        }
			
	//        return(I);			
	//    }

	//    static void rowdiv(double[,] a,int r, double s )
	//    {
	//        int co=a.GetLength(1);
	//        for(int q=0;q<co;q++)
	//        {
	//            a[r,q]=a[r,q]/s;
	//        }
	//    }
	//    static void rowsub(double[,] a, int i, int j,double s)
	//    {
	//        int co=a.GetLength(1);
	//        for (int q=0;q<co;q++)
	//        {
	//            a[i,q]=a[i,q]-(s*a[j,q]);
	//        }
	//    }

	//    static double[,] interrow (double[,]a ,int i , int j)
	//    {
	//        int ro = a.GetLength(0);
	//        int co = a.GetLength(1);
	//        double temp =0;
	//        for (int q=0;q<co;q++)
	//        {
	//            temp=a[i,q];
	//            a[i,q]=a[j,q];
	//            a[j,q]=temp;
	//        }
	//        return(a);
	//    }

	//    static double[,] eyes (int n)
	//    {
	//        double[,] a= new double[n,n];
	//        for (int p=0;p<n;p++)
	//        {
	//            for (int q=0;q<n;q++)
	//            {
	//                if(p==q)
	//                {
	//                    a[p,q]=1;
	//                }
	//                else
	//                {
	//                    a[p,q]=0;
	//                }
					
	//            }
	//        }
	//        return(a);
	//    }
		
	//    static double[,] scalmul(double scalar,double[,] A)
	//    {
	//        int ro = A.GetLength(0);
	//        int co = A.GetLength(1);
	//        double[,] B = new double[ro,co];
	//        for(int p=0;p<ro;p++)
	//        {
	//            for(int q=0;q<co;q++)
	//            {
	//                B[p,q]= scalar*A[p,q];
	//            }
	//        }
	//        return(B);	
	//    }

	//}


	/// <summary>
	/// This is the abstract matrix class.
	/// </summary>
	public abstract class Matrix
	{
	}
}