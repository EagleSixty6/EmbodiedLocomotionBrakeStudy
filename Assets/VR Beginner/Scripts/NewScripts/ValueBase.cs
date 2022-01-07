using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//script to save logging data

[System.Serializable]
public class ValueBase
{
    SortedDictionary<string, string> valuemap;
    int key = 9532;
    public ValueBase()
    {
        valuemap = new SortedDictionary<string, string>();

    }
    private string Hash(string data)
    {
        byte[] textToBytes = Encoding.UTF8.GetBytes(data);
        SHA256Managed mySHA = new SHA256Managed();
        byte[] hashValue = mySHA.ComputeHash(textToBytes);

        return GetHexStringFromHash(hashValue);
    }

    private string GetHexStringFromHash(byte[] hash)
    {
        string hexStr = String.Empty;

        foreach (byte b in hash)
        {
            hexStr += b.ToString("x2");
        }

        return hexStr;
    }

    private string Encrypt(string data, int key)
    {
        StringBuilder input = new StringBuilder(data);
        StringBuilder output = new StringBuilder(data.Length);

        char character;
        for (int i = 0; i < data.Length; i++)
        {
            character = input[i];
            character = (char)(character ^ key);
            output.Append(character);

        }

        return output.ToString();
    }

    public void saveToFile(string filedir)
    {
        string buffer = "";

        foreach (var pair in valuemap)
        {
            buffer += pair.Key + ":" + pair.Value + "\n";
        }


        // Write each directory name to a file.
        using (StreamWriter sw = new StreamWriter(filedir))
        {
            //foreach (var pair in valuemap)
            //    sw.WriteLine(pair.Key + ":" + pair.Value);
            sw.Write(Encrypt(buffer, key));
        }
    }

    public void loadFromFile(string filedir)
    {
        string buffer = "";

        // Write each directory name to a file.
        using (StreamReader sr = new StreamReader(filedir))
        {
            buffer = sr.ReadToEnd();
        }

        buffer = Encrypt(buffer, key);

        string[] mapvalues = buffer.Split('\n');


        foreach (var element in mapvalues)
        {
            string[] split = element.Split(':');

            if (split.Length == 2)
            {
                if (!valuemap.ContainsKey(split[0]))
                    valuemap.Add(split[0], split[1]);
                else
                    valuemap[split[0]] = split[1];

            }
        }
    }

    //new saves new filte 
    public void saveToFileAsValues(string filedir)
    {
        string buffer = "";

        using (StreamWriter sw = new StreamWriter(filedir))
        {
            foreach (var pair in valuemap)
            {
                buffer += pair.Value + ";";
                //sw.WriteLine(pair.Value + ";");

            }

            sw.Write(Encrypt(buffer, key));

        }

    }

    public void sortSavetoFile(string filedir)
    {
        string buffer = "";

        for (int index = 1;true;index++)
        {

            string strIndex = index.ToString();

            if (valuemap.ContainsKey(strIndex))
            {
                buffer += strIndex + ":" + valuemap[strIndex] + "\n";
            }
            else
                break;
        }

        // Write each directory name to a file.
        using (StreamWriter sw = new StreamWriter(filedir))
        {
            //foreach (var pair in valuemap)
            //    sw.WriteLine(pair.Key + ":" + pair.Value);
            sw.Write(Encrypt(buffer, key));
        }

    }


    

    public void createIntValue(string key, int value)
    {
        if (!valuemap.ContainsKey(key))
            valuemap.Add(key, value.ToString());
        else
            valuemap[key] = value.ToString();
    }
    public void createFloatValue(string key, float value)
    {
        if (!valuemap.ContainsKey(key))
            valuemap.Add(key, value.ToString());
        else
            valuemap[key] = value.ToString();
    }

    public void createStringValue(string key, string value)
    {
        if (!valuemap.ContainsKey(key))
            valuemap.Add(key, value);
        else
            valuemap[key] = value;
    }

    public void createBoolValue(string key, bool value)
    {
        if (!valuemap.ContainsKey(key))
            valuemap.Add(key, value.ToString());
        else
            valuemap[key] = value.ToString();
    }

    public int toIntValue(string key)
    {
        if (valuemap.ContainsKey(key))
            return Int32.Parse(valuemap[key]);

        return -1;
    }
    public float toFloatValue(string key)
    {
        if (valuemap.ContainsKey(key))
            return (float)Convert.ToDouble(valuemap[key]);

        return -1.0f;
    }
    public bool toBolValue(string key)
    {
        if (valuemap.ContainsKey(key))
            return Convert.ToBoolean(valuemap[key]);

        return false;
    }
    public string toStringValue(string key)
    {
        if (valuemap.ContainsKey(key))
            return valuemap[key];

        return "";
    }
}
