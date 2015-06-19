using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.Utilities
{
    // Code is adapted from http://www.devhood.com/Tutorials/tutorial_details.aspx?tutorial_id=574
    public class QuickSort
    {
        private static IComparable[] array;
        public static IComparable[] Sort(Dictionary<string, IComparable> Items)
        {
            array = new IComparable[Items.Count];
            int i = 0;
            foreach (IComparable obj in Items.Values)
            {
                array[i] = obj;
                i++;
            }
            QSort(0, array.Length - 1);
            return array;
        }
        public static IComparable[] Sort(Dictionary<string, SignificanceTestResultItem> Items)
        {
            array = new IComparable[Items.Count];
            int i = 0;
            foreach (IComparable obj in Items.Values)
            {
                array[i] = obj;
                i++;
            }
            QSort(0, array.Length - 1);
            return array;
        }

        private static void QSort(int beg, int end)
        {
            if (end == beg) return;
            if (end - beg < 9) //an arbitrary limit to when we call selectionsort
                SelectionSort(beg, end);
            else
            {
                int l = Pivot(beg, end);
                QSort(beg, l - 1);
                QSort(l + 1, end);
            }
        }

        private static int Pivot(int beg, int end)
        {
            //set pivot to beginning of array
            IComparable p = array[beg];
            //m also starts at beginning
            int m = beg;
            //n starts off end (we'll decrement it before it's used)
            int n = end + 1;
            do
            {
                m = m + 1;
            } while (array[m].CompareTo(p) <= 0 && m < end);//find first larger element
            do
            {
                n = n - 1;
            } while (array[n].CompareTo(p) > 0);//find last smaller element
            //loop until pointers cross
            while (m < n)
            {
                //swap
                IComparable temp = array[m];
                array[m] = array[n];
                array[n] = temp;
                //find next values to swap
                do
                {
                    m = m + 1;
                } while (array[m].CompareTo(p) <= 0);
                do
                {
                    n = n - 1;
                } while (array[n].CompareTo(p) > 0);
            }
            //swap beginning with n
            IComparable temp2 = array[n];
            array[n] = array[beg];
            array[beg] = temp2;
            return n;// n is now the division between two unsorted halves
        }

        private static void SelectionSort(int beg, int end)
        {
            for (int i = beg; i < end; i++)
            {
                int minj = i;
                IComparable minx = array[i];
                for (int j = i + 1; j <= end; j++)
                {
                    if (array[j].CompareTo(minx) < 0)
                    {
                        minj = j;
                        minx = array[j];
                    }
                }
                array[minj] = array[i];
                array[i] = minx;
            }
        }
    }
}
