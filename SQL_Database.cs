using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Data;

namespace Microsoft.BotBuilderSamples
{
  class SQL_Database
  {

    private SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder
    {
      DataSource = "viplabcareerhackserver.database.windows.net",
      UserID = "viplab",
      Password = "Careerhack12345",
      InitialCatalog = "VIPLABCAREERHACKDB",
    };
    private SqlConnection connection;
    public SQL_Database()
    {
      using (connection = new SqlConnection(cb.ConnectionString))
      {
        connection.Open();
        Submit_Tsql_NonQuery(sql_cmd_CreateTables);

      }

    }

    public void Submit_Tsql_NonQuery(string tsqlSourceCode)
    {
      using (connection = new SqlConnection(cb.ConnectionString))
      {
        connection.Open();
        using (var command = new SqlCommand(tsqlSourceCode, connection))
        {
          int rowsAffected = command.ExecuteNonQuery();
          Console.WriteLine(rowsAffected + " = rows affected.");
        }
      }
    }
    public void createTable()
    {
      Submit_Tsql_NonQuery(sql_cmd_CreateTables);
    }

    private const string sql_cmd_CreateTables = @"
                DROP TABLE IF EXISTS tabBought_List;
                DROP TABLE IF EXISTS tabItem;
                DROP TABLE IF EXISTS tabUser;

                CREATE TABLE tabUser
                (
                    id  nvarchar(40) PRIMARY KEY,
                    Location    nvarchar(128),
                    InterestType    nvarchar(128)
                );

                CREATE TABLE tabItem
                (
                    id  nvarchar(40) PRIMARY KEY,
                    --Time    DATETIME,
                    Time    VARCHAR(20),
                    Type    VARCHAR(20),
                    --img     IMAGE,  
                    img     VARCHAR(20),
                    status  VARCHAR(20),
                    Count   INT NOT NULL,
                    Description     VARCHAR(120),
                    Location    VARCHAR(120),
                    User_id nvarchar(40),
                    piece INT
                );
                
                CREATE TABLE tabBought_List
                (
                    User_id   nvarchar(40) NOT NULL,
                    Item_id  nvarchar(40) NOT NULL,
                    Bought_quantity  INT NOT NULL,
                    --order_price DECIMAL   NOT NULL,
                    PRIMARY KEY(User_id, Item_id),
                    FOREIGN KEY(User_id) REFERENCES tabUser(id),
                    FOREIGN KEY(Item_id) REFERENCES tabItem(id)
                );
            ";


    public void Insert_tabUser(
        string user_id,
        string Loaction,
        string InterestType,
        string tsqlSourceCode = sql_cmd_Insert_tabUser)
    {
      using (connection = new SqlConnection(cb.ConnectionString))
      {
        connection.Open();
        using (var command = new SqlCommand(tsqlSourceCode, connection))
        {
          command.Parameters.AddWithValue("@value1", user_id);
          command.Parameters.AddWithValue("@value2", Loaction);
          command.Parameters.AddWithValue("@value3", InterestType);
          int rowsAffected = command.ExecuteNonQuery();
          Console.WriteLine(rowsAffected + " = rows affected.");
        }
      }
    }

    public void Insert_tabItem(
        string Item_id,
       string Time,
       string Type,
       string img,
       string status,
       int count,
       string Description,
       string Location,
       string User_id,
       int piece,
       string tsqlSourceCode = sql_cmd_Insert_tabItem)
    {

      using (connection = new SqlConnection(cb.ConnectionString))
      {
        connection.Open();
        using (var command = new SqlCommand(tsqlSourceCode, connection))
        {
          command.Parameters.AddWithValue("@value1", Item_id);
          command.Parameters.AddWithValue("@value2", Time);
          command.Parameters.AddWithValue("@value3", Type);
          command.Parameters.AddWithValue("@value4", img);
          command.Parameters.AddWithValue("@value5", status);
          command.Parameters.AddWithValue("@value6", count);
          command.Parameters.AddWithValue("@value7", Description);
          command.Parameters.AddWithValue("@value8", Location);
          command.Parameters.AddWithValue("@value9", User_id);
          command.Parameters.AddWithValue("@value10", piece);
          int rowsAffected = command.ExecuteNonQuery();
          Console.WriteLine(rowsAffected + " = rows affected.");
        }
      }
    }


    public void Insert_tabBought_List(
        string User_id,
        string Item_id,
        int Bought_quantity,
        string tsqlSourceCode = sql_cmd_Insert_tabBought_List)
    {

      using (connection = new SqlConnection(cb.ConnectionString))
      {
        connection.Open();
        using (var command = new SqlCommand(tsqlSourceCode, connection))
        {
          command.Parameters.AddWithValue("@value1", User_id);
          command.Parameters.AddWithValue("@value2", Item_id);
          command.Parameters.AddWithValue("@value3", Bought_quantity);
          int rowsAffected = command.ExecuteNonQuery();
          Console.WriteLine(rowsAffected + " = rows affected.");
        }
      }
    }

    public void update_tabItem(
     string status,
     string item_id,
     int count,
     string tsqlSourceCode = sql_cmd_Update_tabItem)
    {
      using (connection = new SqlConnection(cb.ConnectionString))
      {
        connection.Open();
        using (var command = new SqlCommand(tsqlSourceCode, connection))
        {
          command.Parameters.AddWithValue("@value1", status);
          command.Parameters.AddWithValue("@value2", item_id);
          command.Parameters.AddWithValue("@value3", count);
          int rowsAffected = command.ExecuteNonQuery();
          Console.WriteLine(rowsAffected + " = rows affected.");
        }
      }
    }


    private const string sql_cmd_DeleteTables = @" 
                DROP TABLE IF EXISTS tabBought_List;
                DROP TABLE IF EXISTS tabItem;
                DROP TABLE IF EXISTS tabUser;
            ";


    private const string sql_cmd_Insert_tabUser = @"
                INSERT INTO tabUser (id, Location, InterestType)
                VALUES
                    (@value1, @value2, @value3)
            ";


    private const string sql_cmd_Insert_tabItem = @"
                INSERT INTO tabItem (id, Time, Type, img, status, count, Description, Location, User_id, piece)
                VALUES
                    (@value1, @value2, @value3,@value4, @value5, @value6, @value7, @value8, @value9, @value10)
            ";


    private const string sql_cmd_Insert_tabBought_List = @"
                INSERT INTO tabBought_List
                VALUES
                    (@value1, @value2, @value3)
            ";


    public const string sql_cmd_select_tabItem = @"
                DECLARE @Item_id  nvarchar(40) = @value1;
                SELECT
                    Count
                FROM tabItem
                WHERE id = @Item_id;
            ";

    public const string sql_cmd_select_tabBought_List = @"
                DECLARE @User_id  nvarchar(40) = @value1;
                DECLARE @Item_id  nvarchar(40) = @value2;
                SELECT
                    Bought_quantity
                FROM tabBought_List
                WHERE User_id=@User_id AND Item_id=@Item_id;
            ";


    public byte[] GetPictureData(string imagepath)
    {
      FileStream file = new FileStream(imagepath, FileMode.Open);
      byte[] by = new byte[file.Length];
      file.Read(by, 0, by.Length);
      file.Close();
      return by;
    }



    private const string sql_cmd_Update_tabItem = @"
                DECLARE @status  nvarchar(32) = @value1;
                DECLARE @item_id  nvarchar(32) = @value2;
                DECLARE @count  INT = @value3;
                UPDATE tabItem
                SET
                    Count = @count,
                    status = @status
                FROM
                    tabBought_List
                WHERE id=@item_id;
                
            ";


    static private string Build_5_Tsql_DeleteJoin = @"
                DECLARE @DName2  nvarchar(128);
                SET @DName2 = @csharpParmDepartmentName;  --'Legal';

                -- Right size the Legal department.
                DELETE empl
                FROM
                    tabEmployee   as empl
                INNER JOIN
                    tabDepartment as dept ON dept.DepartmentCode = empl.DepartmentCode
                WHERE
                    dept.DepartmentName = @DName2

                -- Disband the Legal department.
                DELETE tabDepartment
                    WHERE DepartmentName = @DName2;
            ";


    public int Select_tabBought_List(string user_id, string item_id, string tsql = sql_cmd_select_tabBought_List)
    {
      using (connection = new SqlConnection(cb.ConnectionString))
      {
        connection.Open();
        Console.WriteLine();
        Console.WriteLine("=================================");
        Console.WriteLine("Select value from table");

        int value = 0;

        using (var command = new SqlCommand(tsql, connection))
        {
          command.Parameters.AddWithValue("@value1", user_id);
          command.Parameters.AddWithValue("@value2", item_id);
          int rowsAffected = command.ExecuteNonQuery();
          Console.WriteLine(rowsAffected + " = rows affected.");

          using (SqlDataReader reader = command.ExecuteReader())
          {

            while (reader.Read())
            {
              Console.WriteLine(reader.GetInt32(0));
              value = reader.GetInt32(0);
            }
          }
        }
        return value;
      }
    }

    public int Select_tabItem(string item_id, string tsql = sql_cmd_select_tabItem)
    {
      using (connection = new SqlConnection(cb.ConnectionString))
      {
        connection.Open();
        Console.WriteLine();
        Console.WriteLine("=================================");
        Console.WriteLine("Select value from table");
        int value = 0;

        using (var command = new SqlCommand(tsql, connection))
        {
          command.Parameters.AddWithValue("@value1", item_id);
          int rowsAffected = command.ExecuteNonQuery();
          Console.WriteLine(rowsAffected + " = rows affected.");

          using (SqlDataReader reader = command.ExecuteReader())
          {

            while (reader.Read())
            {
              Console.WriteLine(reader.GetInt32(0));
              value = reader.GetInt32(0);
            }
          }
        }
        return value;
      }
    }
  }
}

