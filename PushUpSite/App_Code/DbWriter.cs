using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for DbWriter
/// </summary>
public class DbWriter
{
    public DbWriter ()
    {
    }

    public string UpdateDB ( string prosedur, params SqlParameter[] parameterList )
    {
        string returnValue = "0";
        SqlDataReader dataReader = null;
        SqlConnection connection = null;

        try
        {
            connection = new SqlConnection ();
            connection.ConnectionString = GetConnectionString ();

            SqlCommand command = new SqlCommand ();
            command.Connection = connection;
            command.CommandText = prosedur;
            command.CommandType = CommandType.StoredProcedure;

            if (parameterList.Length > 0)
                for (var i = 0; i < parameterList.Length; i++)
                    command.Parameters.Add (parameterList[i]);

            connection.Open ();
            dataReader = command.ExecuteReader ();

            if (dataReader.Read ())
                return dataReader.GetString (0);
        }
        catch (Exception e)
        {
            returnValue = "error update database : " + e.ToString ();
            Console.WriteLine ("error update database : " + e.ToString ());
        }
        finally
        {
            if (connection != null)
                connection.Close ();
            if (dataReader != null)
                dataReader.Close ();
        }

        return returnValue;
    }

    private string GetConnectionString ()
    {
        return ConfigurationManager.ConnectionStrings["MySqlDataConnection"].ConnectionString;
    }
}