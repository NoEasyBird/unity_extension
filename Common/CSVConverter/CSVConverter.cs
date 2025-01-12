using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Utility
{
    public static class CSVConverter
    {
        public static List<T> LoadTextAsset<T>(string path)
        {
            TextAsset textAsset = Resources.Load(path) as TextAsset;
            if (textAsset)
            {
                return Read<T>(textAsset);
            }
            
            LogManager.LogError("Read Text Asset Error - Path : " + path);

            return null;
        }

        public static List<T> Read<T>(TextAsset textAsset)
        {
            string[,] parseData = CSVParse(textAsset.text);
            List<T> objList = new List<T>();

            for (var i = 1; i < parseData.GetLength(0); i++)
            {
                T obj = Activator.CreateInstance<T>();

                for (var j = 0; j < parseData.GetLength(1); j++)
                {
                    string fieldName = parseData[0, j];
                    string value = parseData[i, j];

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    PropertyInfo propertyInfo = obj.GetType().GetProperty(fieldName);
                    if (propertyInfo != null)
                    {
                        try
                        {
                            propertyInfo.SetValue(obj,
                                Convert.ChangeType(value, propertyInfo.PropertyType, CultureInfo.InvariantCulture),
                                null);
                        }
                        catch (InvalidCastException e)
                        {
                            LogManager.LogError(string.Format("[{0}][{1}][{2}]{3}", typeof(T), fieldName, i, e.Message));
                        }
                        catch (NullReferenceException e)
                        {
                            LogManager.LogError(string.Format("[{0}][{1}][{2}]{3}", typeof(T), fieldName, i, e.Message));
                        }
                        catch
                        {
                            LogManager.LogError(string.Format("[{0}][{1}][{2}] Exception", typeof(T), fieldName, i));
                        }
                    }
                }
                objList.Add(obj);
            }

            return objList;
        }

        private static string[,] CSVParse(string csvStr)
        {
            var SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
            var LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
            //Line 나누기
            string[] lines = Regex.Split(csvStr, LINE_SPLIT_RE);
            var header = Regex.Split(lines[0], SPLIT_RE);
            string[,] grid = new string[lines.Length, header.Length];
            //Line 에서 property 나누기
            for (var i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    continue;
                }
                string[] lineSplits = Regex.Split(lines[i], SPLIT_RE);
                for (var j = 0; j < lineSplits.Length; j++)
                {
                    grid[i, j] = lineSplits[j];
                }
            }

            return grid;
        }
        public static DataTable LoadCSV(string fileName)
        {
            DataTable table = new DataTable();
            
            string[] buff = File.ReadAllLines(Application.dataPath + "/Resources/Text/" + fileName + ".csv", Encoding.UTF8);
            
            for (var i = 0; i < buff.Length; i++)
            {
                if (string.IsNullOrEmpty(buff[i]))
                {
                    continue;
                }

                string[] split = buff[i].Split(',');

                if (split.Length <= 0)
                {
                    continue;
                }

                if (i == 0)
                {
                    foreach (var columnField in split)
                    {
                        table.Columns.Add(columnField);
                    }
                }
                else
                {
                    table.Rows.Add(split);
                }
            }

            return table;
        }

        
    
        public static bool SaveCSV(string fileName, DataTable table)
        {
            List<string> buff = new List<string>();
 
            string[] columnNames = table.Columns.Cast<DataColumn>().
                Select(column => column.ColumnName).
                ToArray();
 
            string line = string.Join(",", columnNames);
            buff.Add(line);
 
            foreach (DataRow row in table.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                    ToArray();
 
                line = string.Join(",", fields);
                buff.Add(line);
            }
 
            File.WriteAllLines(Application.dataPath + "/Resources/Text/" + fileName + ".csv", buff.ToArray(), Encoding.UTF8);
 
 
            return true;
        }
    }
}
