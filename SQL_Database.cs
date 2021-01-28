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
    //class Program
    //{

    //    static void Main(string[] args)
    //    {
    //        try
    //        {
    //            SQL_Database sQL_Database = new SQL_Database();
    //            var cb = new SqlConnectionStringBuilder
    //            {
    //                DataSource = "viplabcareerhackserver.database.windows.net",
    //                UserID = "viplab",
    //                Password = "Careerhack12345",
    //                InitialCatalog = "VIPLABCAREERHACKDB",
    //            };
                
    //            using (var connection = new SqlConnection(cb.ConnectionString))
    //            {
    //                connection.Open();

    //                sQL_Database.Submit_Tsql_NonQuery(connection, "Create-Tables", sQL_Database.sql_cmd_CreateTables());
    //                //Submit_Tsql_NonQuery(connection, "Create-Tables", sql_cmd_CreateTables());

    //                sQL_Database.Submit_Tsql_Insert_tabUser(connection, "Insert into table user", 
    //                    sQL_Database.sql_cmd_Insert_tabUser(), "Taiwan", "dog");
    //                //Submit_Tsql_Insert_tabUser(connection, "Insert into table user",
    //                //    sql_cmd_Insert_tabUser(), "Taiwan", "dog");

    //                //Submit_Tsql_Insert_tabUser(connection, "Insert into table user",
    //                //   sql_cmd_Insert_tabUser(), "US", "FUCK");

    //                sQL_Database.Submit_Tsql_Insert_tabItem(connection, "Insert into table item", sQL_Database.sql_cmd_Insert_tabItem(),
    //                    "2021/01/27/01:20:00", "food", "", "on sell", 3, "second-hand", "Taiwan", 1, 87);

    //                sQL_Database.Submit_Tsql_Insert_tabBought_List(connection, "Insert into table Bought list",
    //                    sQL_Database.sql_cmd_Insert_tabBought_List(), 1, 1, 1);

    //                //Submit_Tsql_Insert_tabItem(connection, "Insert into table item", sql_cmd_Insert_tabItem(),
    //                //      "2021/01/27/01:20:00", "food", "", "sold", 3, "second-hand", "Taiwan", 1, 100);

    //                int count = sQL_Database.Submit_Tsql_Select(connection, sQL_Database.sql_cmd_select_tabItem());

    //                int bought_quantity = sQL_Database.Submit_Tsql_Select(connection, sQL_Database.sql_cmd_select_tabBought_List());

    //                string status;
    //                if (count - bought_quantity > 0)
    //                    status = "on sell";
    //                else
    //                    status = "sold";

    //                sQL_Database.Submit_Tsql_update_tabItem(connection, "Update table item", sQL_Database.sql_cmd_Update_tabItem(), status);

    //                //Submit_Tsql_NonQuery(connection, "5 - Delete-Join", Build_5_Tsql_DeleteJoin(),
    //                //    "@csharpParmDepartmentName", "Legal");

    //                //Submit_6_Tsql_Select_Buyer(connection);

    //                //Submit_Tsql_NonQuery(connection, "Delete-Tables", sql_cmd_DeleteTables());


    //                connection.Close();

    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.ToString());
    //        }

    //        Console.WriteLine("View the report output here, then press any key to end the program...");
    //        Console.ReadKey();
    //    }

    //    //static void Submit_Tsql_NonQuery(
    //    //    SqlConnection connection,
    //    //    string tsqlPurpose,
    //    //    string tsqlSourceCode
    //    //    )
    //    //{
    //    //    Console.WriteLine();
    //    //    Console.WriteLine("=================================");
    //    //    Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

    //    //    using (var command = new SqlCommand(tsqlSourceCode, connection))
    //    //    {
    //    //        int rowsAffected = command.ExecuteNonQuery();
    //    //        Console.WriteLine(rowsAffected + " = rows affected.");
    //    //    }
    //    //}

    //    //static string sql_cmd_CreateTables()
    //    //{
    //    //    return @"
    //    //        DROP TABLE IF EXISTS tabBought_List;
    //    //        DROP TABLE IF EXISTS tabItem;
    //    //        DROP TABLE IF EXISTS tabUser;

    //    //        CREATE TABLE tabUser
    //    //        (
    //    //            id  INT IDENTITY PRIMARY KEY,
    //    //            Location    nvarchar(128),
    //    //            InterestType    nvarchar(128)
    //    //        );

    //    //        CREATE TABLE tabItem
    //    //        (
    //    //            id  INT IDENTITY PRIMARY KEY,
    //    //            --Time    DATETIME,
    //    //            Time    VARCHAR(20),
    //    //            Type    VARCHAR(20),
    //    //            --img     IMAGE,  
    //    //            img     VARCHAR(20),
    //    //            status  VARCHAR(20),
    //    //            Count   INT NOT NULL,
    //    //            Description     VARCHAR(120),
    //    //            Location    VARCHAR(120),
    //    //            User_id INT,
    //    //            piece INT
    //    //        );
                
    //    //        CREATE TABLE tabBought_List
    //    //        (
    //    //            User_id   INT NOT NULL,
    //    //            Item_id  INT NOT NULL,
    //    //            Bought_quantity  INT NOT NULL,
    //    //            --order_price DECIMAL   NOT NULL,
    //    //            PRIMARY KEY(User_id, Item_id),
    //    //            FOREIGN KEY(User_id) REFERENCES tabUser(id),
    //    //            FOREIGN KEY(Item_id) REFERENCES tabItem(id)
    //    //        );
                
    //    //    ";
    //    //}

    //    //static void Submit_Tsql_Insert_tabUser(
    //    //    SqlConnection connection,
    //    //    string tsqlPurpose,
    //    //    string tsqlSourceCode,
    //    //    string Loaction,
    //    //    string InterestType)
    //    //{
    //    //    Console.WriteLine();
    //    //    Console.WriteLine("=================================");
    //    //    Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

    //    //    using (var command = new SqlCommand(tsqlSourceCode, connection))
    //    //    {
    //    //        //command.Parameters.AddWithValue("@value1", id);
    //    //        command.Parameters.AddWithValue("@value2", Loaction);
    //    //        command.Parameters.AddWithValue("@value3", InterestType);
    //    //        int rowsAffected = command.ExecuteNonQuery();
    //    //        Console.WriteLine(rowsAffected + " = rows affected.");
    //    //    }
    //    //}

    //    //static void Submit_Tsql_Insert_tabItem(
    //    //    SqlConnection connection,
    //    //    string tsqlPurpose,
    //    //    string tsqlSourceCode,
    //    //    string Time,
    //    //    string Type,
    //    //    string img,
    //    //    string status,
    //    //    int count,
    //    //    string Description,
    //    //    string Location,
    //    //    int User_id,
    //    //    int piece)
    //    //{
    //    //    Console.WriteLine();
    //    //    Console.WriteLine("=================================");
    //    //    Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

    //    //    using (var command = new SqlCommand(tsqlSourceCode, connection))
    //    //    {
    //    //        //command.Parameters.AddWithValue("@value1", id);
    //    //        command.Parameters.AddWithValue("@value2", Time);
    //    //        command.Parameters.AddWithValue("@value3", Type);
    //    //        command.Parameters.AddWithValue("@value4", img);
    //    //        command.Parameters.AddWithValue("@value5", status);
    //    //        command.Parameters.AddWithValue("@value6", count);
    //    //        command.Parameters.AddWithValue("@value7", Description);
    //    //        command.Parameters.AddWithValue("@value8", Location);
    //    //        command.Parameters.AddWithValue("@value9", User_id);
    //    //        command.Parameters.AddWithValue("@value10", piece);
    //    //        int rowsAffected = command.ExecuteNonQuery();
    //    //        Console.WriteLine(rowsAffected + " = rows affected.");
    //    //    }
    //    //}


    //    //static void Submit_Tsql_Insert_tabBought_List(
    //    //    SqlConnection connection,
    //    //    string tsqlPurpose,
    //    //    string tsqlSourceCode,
    //    //    int User_id,
    //    //    int Item_id,
    //    //    int Bought_quantity)
    //    //{
    //    //    Console.WriteLine();
    //    //    Console.WriteLine("=================================");
    //    //    Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

    //    //    using (var command = new SqlCommand(tsqlSourceCode, connection))
    //    //    {
    //    //        command.Parameters.AddWithValue("@value1", User_id);
    //    //        command.Parameters.AddWithValue("@value2", Item_id);
    //    //        command.Parameters.AddWithValue("@value3", Bought_quantity);
    //    //        int rowsAffected = command.ExecuteNonQuery();
    //    //        Console.WriteLine(rowsAffected + " = rows affected.");
    //    //    }
    //    //}

    //    //static void Submit_Tsql_update_tabItem(
    //    // SqlConnection connection,
    //    // string tsqlPurpose,
    //    // string tsqlSourceCode,
    //    // string status)
    //    //{
    //    //    Console.WriteLine();
    //    //    Console.WriteLine("=================================");
    //    //    Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

    //    //    using (var command = new SqlCommand(tsqlSourceCode, connection))
    //    //    {
    //    //        command.Parameters.AddWithValue("@value1", status);
    //    //        int rowsAffected = command.ExecuteNonQuery();
    //    //        Console.WriteLine(rowsAffected + " = rows affected.");
    //    //    }
    //    //}


    //    //static string sql_cmd_DeleteTables()
    //    //{
    //    //    return @" 
    //    //        DROP TABLE IF EXISTS tabBought_List;
    //    //        DROP TABLE IF EXISTS tabItem;
    //    //        DROP TABLE IF EXISTS tabUser;
    //    //    ";
    //    //}



    //    //static string sql_cmd_Insert_tabUser()
    //    //{
    //    //    return @"
    //    //        INSERT INTO tabUser (Location, InterestType)
    //    //        VALUES
    //    //            (@value2, @value3)
    //    //    ";
    //    //}

    //    //static string sql_cmd_Insert_tabItem()
    //    //{
    //    //    //('A', '', 'a', SWITCHOFFSET(SYSDATETIMEOFFSET(), '+08:00'), GETDATE()),
    //    //    return @"
    //    //        INSERT INTO tabItem (Time, Type, img, status, count, Description, Location, User_id, piece)
    //    //        VALUES
    //    //            (@value2, @value3,@value4, @value5, @value6, @value7, @value8, @value9, @value10)
    //    //    ";
    //    //}

    //    //static string sql_cmd_Insert_tabBought_List()
    //    //{
    //    //    return @"
    //    //        INSERT INTO tabBought_List
    //    //        VALUES
    //    //            (@value1, @value2, @value3)
    //    //    ";
    //    //}

    //    //static string sql_cmd_select_tabItem()
    //    //{
    //    //    return @"
    //    //        SELECT
    //    //            Count
    //    //        FROM tabItem;
    //    //    ";
    //    //}

    //    //static string sql_cmd_select_tabBought_List()
    //    //{
    //    //    return @"
    //    //        SELECT
    //    //            Bought_quantity
    //    //        FROM tabBought_List;
    //    //    ";
    //    //}


    //    //public byte[] GetPictureData(string imagepath)
    //    //{
    //    //    FileStream file = new FileStream(imagepath, FileMode.Open);
    //    //    byte[] by = new byte[file.Length];
    //    //    file.Read(by, 0, by.Length);
    //    //    file.Close();
    //    //    return by;
    //    //}

        

    //    //static string sql_cmd_Update_tabItem()
    //    //{
    //    //    return @"
    //    //        DECLARE @status  nvarchar(32) = @value1;

    //    //        UPDATE tabItem
    //    //        SET
    //    //            Count -= tabBought_List.Bought_quantity,
    //    //            status = @status
    //    //        FROM
    //    //            tabBought_List;
    //    //    ";
    //    //}

    //    //static string Build_5_Tsql_DeleteJoin()
    //    //{
    //    //    return @"
    //    //        DECLARE @DName2  nvarchar(128);
    //    //        SET @DName2 = @csharpParmDepartmentName;  --'Legal';

    //    //        -- Right size the Legal department.
    //    //        DELETE empl
    //    //        FROM
    //    //            tabEmployee   as empl
    //    //        INNER JOIN
    //    //            tabDepartment as dept ON dept.DepartmentCode = empl.DepartmentCode
    //    //        WHERE
    //    //            dept.DepartmentName = @DName2

    //    //        -- Disband the Legal department.
    //    //        DELETE tabDepartment
    //    //            WHERE DepartmentName = @DName2;
    //    //    ";
    //    //}


    //    //static int Submit_Tsql_Select(SqlConnection connection, string tsql)
    //    //{
    //    //    Console.WriteLine();
    //    //    Console.WriteLine("=================================");
    //    //    Console.WriteLine("Select value from table");

    //    //    int value = 0;
            
    //    //    using (var command = new SqlCommand(tsql, connection))
    //    //    {

    //    //        using (SqlDataReader reader = command.ExecuteReader())
    //    //        {
    //    //            while (reader.Read())
    //    //            {
    //    //                Console.WriteLine(reader.GetInt32(0));
    //    //                value = reader.GetInt32(0);
    //    //            }
    //    //        }
    //    //    }
    //    //    return value;
    //    //}
    //} // EndOfClass

    class SQL_Database
    {

        public SQL_Database()
        {

        }

        public void Submit_Tsql_NonQuery(
          SqlConnection connection,
          string tsqlPurpose,
          string tsqlSourceCode
          )
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("T-SQL to {0}...", tsqlPurpose);


            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " = rows affected.");
            }
        }

        public string sql_cmd_CreateTables()
        {
            return @"
                DROP TABLE IF EXISTS tabBought_List;
                DROP TABLE IF EXISTS tabItem;
                DROP TABLE IF EXISTS tabUser;

                CREATE TABLE tabUser
                (
                    id  INT IDENTITY PRIMARY KEY,
                    Location    nvarchar(128),
                    InterestType    nvarchar(128)
                );

                CREATE TABLE tabItem
                (
                    id  INT IDENTITY PRIMARY KEY,
                    --Time    DATETIME,
                    Time    VARCHAR(20),
                    Type    VARCHAR(20),
                    --img     IMAGE,  
                    img     VARCHAR(20),
                    status  VARCHAR(20),
                    Count   INT NOT NULL,
                    Description     VARCHAR(120),
                    Location    VARCHAR(120),
                    User_id INT,
                    piece INT
                );
                
                CREATE TABLE tabBought_List
                (
                    User_id   INT NOT NULL,
                    Item_id  INT NOT NULL,
                    Bought_quantity  INT NOT NULL,
                    --order_price DECIMAL   NOT NULL,
                    PRIMARY KEY(User_id, Item_id),
                    FOREIGN KEY(User_id) REFERENCES tabUser(id),
                    FOREIGN KEY(Item_id) REFERENCES tabItem(id)
                );
                
            ";
        }

        public void Submit_Tsql_Insert_tabUser(
            SqlConnection connection,
            string tsqlPurpose,
            string tsqlSourceCode,
            string Loaction,
            string InterestType)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                //command.Parameters.AddWithValue("@value1", id);
                command.Parameters.AddWithValue("@value2", Loaction);
                command.Parameters.AddWithValue("@value3", InterestType);
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " = rows affected.");
            }
        }

         public void Submit_Tsql_Insert_tabItem(
            SqlConnection connection,
            string tsqlPurpose,
            string tsqlSourceCode,
            string Time,
            string Type,
            string img,
            string status,
            int count,
            string Description,
            string Location,
            int User_id,
            int piece)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                //command.Parameters.AddWithValue("@value1", id);
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


        public void Submit_Tsql_Insert_tabBought_List(
            SqlConnection connection,
            string tsqlPurpose,
            string tsqlSourceCode,
            int User_id,
            int Item_id,
            int Bought_quantity)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                command.Parameters.AddWithValue("@value1", User_id);
                command.Parameters.AddWithValue("@value2", Item_id);
                command.Parameters.AddWithValue("@value3", Bought_quantity);
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " = rows affected.");
            }
        }

        public void Submit_Tsql_update_tabItem(
         SqlConnection connection,
         string tsqlPurpose,
         string tsqlSourceCode,
         string status)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                command.Parameters.AddWithValue("@value1", status);
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " = rows affected.");
            }
        }


        public string sql_cmd_DeleteTables()
        {
            return @" 
                DROP TABLE IF EXISTS tabBought_List;
                DROP TABLE IF EXISTS tabItem;
                DROP TABLE IF EXISTS tabUser;
            ";
        }

        public string sql_cmd_Insert_tabUser()
        {
            return @"
                INSERT INTO tabUser (Location, InterestType)
                VALUES
                    (@value2, @value3)
            ";
        }

        public string sql_cmd_Insert_tabItem()
        {
            //('A', '', 'a', SWITCHOFFSET(SYSDATETIMEOFFSET(), '+08:00'), GETDATE()),
            return @"
                INSERT INTO tabItem (Time, Type, img, status, count, Description, Location, User_id, piece)
                VALUES
                    (@value2, @value3,@value4, @value5, @value6, @value7, @value8, @value9, @value10)
            ";
        }

        public string sql_cmd_Insert_tabBought_List()
        {
            return @"
                INSERT INTO tabBought_List
                VALUES
                    (@value1, @value2, @value3)
            ";
        }

        public string sql_cmd_select_tabItem()
        {
            return @"
                SELECT
                    Count
                FROM tabItem;
            ";
        }

        public string sql_cmd_select_tabBought_List()
        {
            return @"
                SELECT
                    Bought_quantity
                FROM tabBought_List;
            ";
        }


        public byte[] GetPictureData(string imagepath)
        {
            FileStream file = new FileStream(imagepath, FileMode.Open);
            byte[] by = new byte[file.Length];
            file.Read(by, 0, by.Length);
            file.Close();
            return by;
        }



        public string sql_cmd_Update_tabItem()
        {
            return @"
                DECLARE @status  nvarchar(32) = @value1;

                UPDATE tabItem
                SET
                    Count -= tabBought_List.Bought_quantity,
                    status = @status
                FROM
                    tabBought_List;
            ";
        }

        public string Build_5_Tsql_DeleteJoin()
        {
            return @"
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
        }

        public int Submit_Tsql_Select(SqlConnection connection, string tsql)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("Select value from table");

            int value = 0;

            using (var command = new SqlCommand(tsql, connection))
            {

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

