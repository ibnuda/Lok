using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

/// <summary>
/// Summary description for DbClientReader
/// </summary>
public class DbClientReader
{
    public DbClientReader ()
    {
    }

    // TODO: Create method to read data and parameters from android clients.
    public string WusDat (string prosedur, params SqlParameter[] parameterList)
    {
        var returnValue = "Wus Dat?!";
        // Ingatkan saya nulis dimari.
        return returnValue;
    }
}