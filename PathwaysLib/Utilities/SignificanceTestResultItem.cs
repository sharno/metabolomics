using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.Utilities
{
    public class SignificanceTestResultItem : IComparable
    {
        public string ItemName;
        public string ItemID;
        public double pValue;
        public int SampleSize = 0;
        public List<string> AssociatedInputItemIds;

        public SignificanceTestResultItem(string name, string id)
        {
            ItemName = name;
            ItemID = id;
            pValue = 0;
            AssociatedInputItemIds = new List<string>();
        }

        public int CompareTo(object obj) 
        {
            if(obj is SignificanceTestResultItem) 
            {
                SignificanceTestResultItem temp = (SignificanceTestResultItem) obj;
                if (pValue > temp.pValue)
                    return 1;
                else if (pValue == temp.pValue)
                    return 0;
                else
                    return -1;                
            }
            throw new ArgumentException("object is not a SignificanceTestResultItem");    
        }
    }
}
