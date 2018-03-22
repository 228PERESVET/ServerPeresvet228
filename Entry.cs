using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// Запить в Json формате
[Serializable]
public class Entry
{
    public DateTime DT { get; set; }    // Дата-время: datatime
    public bool A { get; set; }         // Доступ: access
    public string N { get; set; }       // Еще какая то байда

    public Entry() { }

    public Entry(DateTime datatime, bool access, string name)
    {
        DT = datatime;
        A = access;
        N = name;
    }
}