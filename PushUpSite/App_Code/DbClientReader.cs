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
    public string WusDat ( string prosedur, params SqlParameter[] parameterList )
    {
        var returnValue = "Wus Dat?!";
        // Ingatkan saya nulis dimari.
        return returnValue;
    }

    public string GetJsonString ( string prosedur, string jsonName, params SqlParameter[] parameterList )
    {
        SqlConnection connection = null;
        SqlDataReader dataReader = null;
        var stringBuilder = new StringBuilder ("");

        try
        {
            connection = new SqlConnection ();
            connection.ConnectionString = GetConnectionString ();

            var command = new SqlCommand ();
            command.Connection = connection;
            command.CommandText = prosedur;
            command.CommandType = CommandType.StoredProcedure;

            if (parameterList.Length > 0)
                foreach (var item in parameterList)
                    command.Parameters.Add (item);

            connection.Open ();
            dataReader = command.ExecuteReader ();

            stringBuilder.Append ("{ \"");
            stringBuilder.Append (jsonName);
            stringBuilder.Append ("\": [");

            if (dataReader.HasRows)
                while (dataReader.Read ())
                    stringBuilder.Append (dataReader.GetString (0)).Append (",");
            if (stringBuilder.ToString ().EndsWith (","))
                stringBuilder = stringBuilder.Remove (stringBuilder.Length - 1, 1);
        }
        catch (Exception up)
        {
            //throw up;
            Console.WriteLine ("Update error: " + up.Message);
        }
        finally
        {
            if (connection != null)
                connection.Close ();
            if (dataReader != null)
                dataReader.Close ();
        }

        stringBuilder.Append ("] }");
        return stringBuilder.ToString ();
    }

    private string GetConnectionString ()
    {
        return ConfigurationManager.ConnectionStrings["MySqlDataConnection"].ConnectionString;
    }
}