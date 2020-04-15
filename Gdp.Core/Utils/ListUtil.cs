//2015.06.06, czs, create in namu, 

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Linq;


namespace Gdp.Utils
{
    /// <summary>
    /// �б�
    /// </summary>
    public static class ListUtil
    {

        /// <summary>
        /// ���������б�ά����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="dimension"></param>
        public static List<T> SetDimension<T>(List<T> list, int dimension)
        {
            if (list == null)
            {
                var array = new T[dimension];
                list = new List<T>(array);
            }
            if (list.Count < dimension)
            {
                var adding = new T[dimension - list.Count];
                list.AddRange(adding);
            }
            return list;
        }
        /// <summary>
        /// ���ظ�����ӣ����ǰ�ж�һ�Ρ�
        /// </summary>
        /// <param name="siteNames"></param>
        /// <param name="siteName"></param>
        public static void AddNoRepeat<T>(List<T> siteNames, T siteName)
        {
            if (!siteNames.Contains(siteName))
            {
                siteNames.Add(siteName);
            }
        }

        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <returns></returns>
        public static bool IsEqual(List<string> listA, List<string> listB)
        {
            //�ж�δ֪�����Ƿ�һ��
            if (listA.Count != listB.Count) return false;
            return GetDifferences(listA, listB).Count == 0; 
        }
        /// <summary>
        /// ��ȡ��ͬ�Ĳ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <returns></returns>
        static public List<T> GetDifferences<T>(IEnumerable<T> listA, IEnumerable<T> listB)
        {
            List<T> list = new List<T>();
            foreach (var item in listA)
            {
                if (!listB.Contains(item)) list.Add(item);
            }

            foreach (var item in listB)
            {
                if (!listA.Contains(item)) list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// �������б��е�δ֪����Ϊ�ӽ����ɡ���С��������С�
        /// ���õݹ���ַ�ʵ�֡�
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="orderedKeys"></param>
        /// <param name="key"></param>
        /// <param name="keyToDouble">������ߵľ��룬���ж�Զ��</param>
        /// <param name="lowIndex"></param>
        /// <param name="highIndex"></param>
        /// <param name="prevIndex"></param>
        /// <param name="prevDistance"></param>
        /// <returns></returns>
        internal static int ClosestToIndexOf<TKey>(List<TKey> orderedKeys, TKey key, Func<TKey,double> keyToDouble = null, int lowIndex = 0, int highIndex = int.MaxValue, int prevIndex = int.MaxValue, double prevDistance = int.MaxValue)
            where TKey : IComparable<TKey>
        {
            if(keyToDouble == null)
            {
                Gdp.Utils.DoubleUtil.AutoSetKeyToDouble(out keyToDouble);//= new Func<TKey, double>((m) =>  Convert.ToDouble(m));
            }

            //orderedKeys.Min(new Func<TKey, double>(m => m.CompareTo(key)));
            if(lowIndex == highIndex) { //���ҵ��˱߽�
                return lowIndex;
            }

            if (highIndex == int.MaxValue || highIndex == -1 )  {   highIndex = orderedKeys.Count - 1;  }

            int midIndex = lowIndex + (highIndex - lowIndex) / 2;
            if(midIndex == lowIndex || midIndex == highIndex) { return midIndex; }

            double differ = keyToDouble(key)- keyToDouble(orderedKeys[midIndex]);  // orderedKeys[mid]  .CompareTo(key);
            double currentDistance = Math.Abs(differ);

            //if (prevDistance == int.MaxValue)//��ʼ��ֵ
            //{
            //    prevDistance = currentDistance;
            //}
            //else if (currentDistance > prevDistance)//������ϴδ��򷵻��ϴν����������ϴ�С����������
            //{
            //    return prevIndex;
            //}

            //�жϵ�ǰ���м��λ��
            if (differ == 0)
            {
                return midIndex;
            }
            else  if (differ > 0)//��ǰֵ���м�Ĵ����ڴ���������
            {
                return ClosestToIndexOf(orderedKeys, key, keyToDouble, midIndex + 1, highIndex, midIndex, currentDistance);
            }
            else //������С���������
            {
                return ClosestToIndexOf(orderedKeys, key, keyToDouble, lowIndex, midIndex - 1, midIndex, currentDistance);
              
            }
        }
        /// <summary>
        /// �������У����ظ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <param name="prns"></param>
        /// <returns></returns>
        public static List<T> GetAll<T>(IEnumerable<T> keys, IEnumerable<T> prns)
        {
            List<T> all = new List<T>(keys);
            foreach (var item in prns)
            {
                if (!all.Contains(item))
                {
                    all.Add(item);
                }
            }

            return all;
        }

        /// <summary>
        /// ��ȡ��ͬ�Ĳ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <returns></returns>
        static public List<T> GetCommonsOfTwo<T>(IEnumerable<T> listA, IEnumerable<T> listB)
        {
            List<T> list = new List<T>();
            foreach (var item in listA)
            {
                if (listB.Contains(item)) list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// ��ȡ��ͬ�Ĳ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists"></param> 
        /// <returns></returns>
        static public List<T> GetCommons<T>( params  IEnumerable<T> [] lists)
        {
            int length = lists.Length;
            var first = lists[0];

            List<T> commons =new List<T>( first);
            for (int i = 1; i < length; i++)
            {
                commons = GetCommonsOfTwo<T>(commons, lists[i]); 
            } 
            return commons;
        }

        /// <summary>
        /// ��ȡǰ���У�����û�еĲ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listhas"></param>
        /// <param name="listNot"></param>
        /// <returns></returns>
        static public List<T> GetExcept<T>(IEnumerable<T> listhas, IEnumerable<T> listNot)
        {
            List<T> list = new List<T>();
            foreach (var item in listhas)
            {
                if (!listNot.Contains(item)) list.Add(item);
            }
            return list;
        }


        /// <summary>
        /// �ϲ�
        /// </summary> 
        /// <param name="indexes"></param>
        /// <param name="oneIndexe"></param>
        /// <returns></returns>
        public static List<T> Emerge<T>(IEnumerable<T> indexes, IEnumerable<T> oneIndexe)
        {
            var differ = GetExcept<T>(oneIndexe, indexes);
            List<T> result = new List<T>();
            result.AddRange(indexes);
            result.AddRange(differ);
            return result;

        }

        /// <summary>
        /// ��ȡ��ǰ�����һ��
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static T GetNext<T>(List<T> list, T current)
        {
            var index = list.LastIndexOf(current);
            if (index < list.Count - 1)
            {
                return list[index + 1];
            }
            return default(T);
        }

        /// <summary>
        /// �������ظ�Ԫ�ص��б�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> GetNoRepeatList<T>(IEnumerable<T> list)
        {
            List<T> result = new List<T>();
            foreach (var item in list)
            {
                if (result.Contains(item)) { continue; }
                result.Add(item);                
            }

            return result;
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="list"></param>
        /// <param name="spliters"></param>
        /// <returns></returns>
        public static List<string> Parse(string list, params char[] spliters)
        {
            if(spliters == null || spliters.Length == 0)
            {
                spliters = new char[] {',',';','\t','\n','��','��' };

            }
            return new List<string>(list.Split(spliters, StringSplitOptions.RemoveEmptyEntries));
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="list"></param> 
        /// <returns></returns>
        public static List<int> ParseToInts(List<string> list)
        {
            var ints = new List<int>();
            foreach (var item in list)
            {
                ints.Add(int.Parse(item));
            }
            return ints;
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="list"></param>
        /// <param name="spliters"></param>
        /// <returns></returns>
        public static List<int> ParseToInts(string list, params char[] spliters)
        {
            return ParseToInts(Parse(list, spliters));
        }

        /// <summary>
        /// �����Ƴ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="tobeRemoves"></param>
        public static void RemoveAt<T>(List<T> values, List<int> tobeRemoves)
        {
            if(tobeRemoves.Count == 0) { return; }

            tobeRemoves.Sort();
            tobeRemoves.Reverse();//����ɾ��
            foreach (var item in tobeRemoves)
            {
                values.RemoveAt(item);
            }
        }
    }
}
