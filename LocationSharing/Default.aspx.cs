using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Data;

public partial class _Default : Page
{
    public static int ValidityTime = 1000 * 60;
    public int UserId = 0;
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]
    public static void LoginUser(string Name, double Latitude, double Longitude)
    {
        MySql.Data.MySqlClient.MySqlConnection mySqlConnection = new MySql.Data.MySqlClient.MySqlConnection();
        mySqlConnection.ConnectionString = "Server=localhost;Database=locationshare;Uid=root;Pwd=root;";
        try
        {
            mySqlConnection.Open();
            switch (mySqlConnection.State)
            {
                case System.Data.ConnectionState.Open:

                    MySqlCommand cmd = new MySqlCommand("LoginUser", mySqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("userNameIn", Name);
                    cmd.Parameters["userNameIn"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("LatitudeIn", Latitude);
                    cmd.Parameters["LatitudeIn"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("LongitudeIn", Longitude);
                    cmd.Parameters["LongitudeIn"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("TokenStartTimeIn", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    cmd.Parameters["TokenStartTimeIn"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("ValidityTime", ValidityTime);
                    cmd.Parameters["ValidityTime"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("userIdOut", MySqlDbType.Int32);
                    cmd.Parameters["userIdOut"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    HttpContext.Current.Session["UserId"] = Convert.ToInt32(cmd.Parameters["userIdOut"].Value);
                    HttpContext.Current.Session["UserName"] = Name;
                    break;
                case System.Data.ConnectionState.Closed:
                    // Connection could not be made, throw an error
                    break;
                default:
                    // Connection is actively doing something else
                    break;
            }
        }
        catch (MySql.Data.MySqlClient.MySqlException mySqlException)
        {
        }
        catch (Exception exception)
        {
        }
        finally
        {
            if (mySqlConnection.State != System.Data.ConnectionState.Closed)
            {
                mySqlConnection.Close();
            }
        }
    }

    [WebMethod]
    public static void CreateGroup(string Name)
    {
        {
            MySql.Data.MySqlClient.MySqlConnection mySqlConnection = new MySql.Data.MySqlClient.MySqlConnection();
            mySqlConnection.ConnectionString = "Server=localhost;Database=locationshare;Uid=root;Pwd=root;";
            try
            {
                mySqlConnection.Open();
                switch (mySqlConnection.State)
                {
                    case System.Data.ConnectionState.Open:

                        MySqlCommand cmd = new MySqlCommand("CreateGroup", mySqlConnection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("userIdIn", HttpContext.Current.Session["UserId"]);
                        cmd.Parameters["userIdIn"].Direction = ParameterDirection.Input;

                        cmd.Parameters.AddWithValue("groupNameIn", Name);
                        cmd.Parameters["groupNameIn"].Direction = ParameterDirection.Input;

                        cmd.Parameters.AddWithValue("userNameIn", HttpContext.Current.Session["UserName"]);
                        cmd.Parameters["userNameIn"].Direction = ParameterDirection.Input;

                        cmd.Parameters.AddWithValue("GroupId", MySqlDbType.Int32);
                        cmd.Parameters["GroupId"].Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();
                        HttpContext.Current.Session["GroupId"] = Convert.ToInt32(cmd.Parameters["GroupId"].Value);
                        break;
                    case System.Data.ConnectionState.Closed:
                        // Connection could not be made, throw an error
                        break;
                    default:
                        // Connection is actively doing something else
                        break;
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException mySqlException)
            {
            }
            catch (Exception exception)
            {
            }
            finally
            {
                if (mySqlConnection.State != System.Data.ConnectionState.Closed)
                {
                    mySqlConnection.Close();
                }
            }
        }
    }

    [WebMethod]
    public static void JoinGroup(string Name)
    {
        MySql.Data.MySqlClient.MySqlConnection mySqlConnection = new MySql.Data.MySqlClient.MySqlConnection();
        mySqlConnection.ConnectionString = "Server=localhost;Database=locationshare;Uid=root;Pwd=root;";
        try
        {
            mySqlConnection.Open();
            switch (mySqlConnection.State)
            {
                case System.Data.ConnectionState.Open:

                    MySqlCommand cmd = new MySqlCommand("JoinGroup", mySqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("userIdIn", HttpContext.Current.Session["UserId"]);
                    cmd.Parameters["userIdIn"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("groupNameIn", Name);
                    cmd.Parameters["groupNameIn"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("userNameIn", HttpContext.Current.Session["UserName"]);
                    cmd.Parameters["userNameIn"].Direction = ParameterDirection.Input;

                    MySqlDataReader reader = null;
                    reader = cmd.ExecuteReader();
                    break;
                case System.Data.ConnectionState.Closed:
                    // Connection could not be made, throw an error
                    break;
                default:
                    // Connection is actively doing something else
                    break;
            }
        }
        catch (MySql.Data.MySqlClient.MySqlException mySqlException)
        {
        }
        catch (Exception exception)
        {
        }
        finally
        {
            if (mySqlConnection.State != System.Data.ConnectionState.Closed)
            {
                mySqlConnection.Close();
            }
        }
    }

    [WebMethod]
    public static List<UserPosition> GetGroupPeople()
    {
        MySql.Data.MySqlClient.MySqlConnection mySqlConnection = new MySql.Data.MySqlClient.MySqlConnection();
        mySqlConnection.ConnectionString = "Server=localhost;Database=locationshare;Uid=root;Pwd=root;";
        List<UserPosition> tUserPosition = new List<UserPosition>();
        try
        {
            mySqlConnection.Open();
            switch (mySqlConnection.State)
            {
                case System.Data.ConnectionState.Open:

                    MySqlCommand cmd = new MySqlCommand("GetGroupPeople", mySqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataReader reader = null;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["RemainingTime"]) > 0 && (string)reader["UserName"] != HttpContext.Current.Session["UserName"].ToString())
                        {
                            UserPosition user = new UserPosition();
                            user.UserName = (string)reader["UserName"];
                            user.Lat = (float)reader["Lat"];
                            user.Lon = (float)reader["Lon"];
                            user.RemainingTime = Convert.ToInt32(reader["RemainingTime"]);
                            tUserPosition.Add(user);
                        }
                    }
                    break;
                case System.Data.ConnectionState.Closed:
                    // Connection could not be made, throw an error
                    break;
                default:
                    // Connection is actively doing something else
                    break;
            }
        }
        catch (MySql.Data.MySqlClient.MySqlException mySqlException)
        {
        }
        catch (Exception exception)
        {
        }
        finally
        {
            if (mySqlConnection.State != System.Data.ConnectionState.Closed)
            {
                mySqlConnection.Close();
            }
        }
        return tUserPosition;
    }

    [WebMethod]
    public static List<string> GetGroupList()
    {
        List<string> tempString = new List<string>();
        MySql.Data.MySqlClient.MySqlConnection mySqlConnection = new MySql.Data.MySqlClient.MySqlConnection();
        mySqlConnection.ConnectionString = "Server=localhost;Database=locationshare;Uid=root;Pwd=root;";
        try
        {
            mySqlConnection.Open();
            switch (mySqlConnection.State)
            {
                case System.Data.ConnectionState.Open:

                    MySqlCommand cmd = new MySqlCommand("GetGroupList", mySqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataReader reader = null;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["GroupName"] != null)
                            tempString.Add((string)reader["GroupName"]);
                    }
                    break;
                case System.Data.ConnectionState.Closed:
                    // Connection could not be made, throw an error
                    break;
                default:
                    // Connection is actively doing something else
                    break;
            }
        }
        catch (Exception exception)
        {
        }
        finally
        {
            if (mySqlConnection.State != System.Data.ConnectionState.Closed)
            {
                mySqlConnection.Close();
            }
        }
        return tempString;
    }
}