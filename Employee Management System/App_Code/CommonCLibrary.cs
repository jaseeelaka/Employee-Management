using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;


namespace Employee_Management_System
{
    public class CommonCLibrary
    {
        public static String SqlConnectionString = "";
        public static String SecretKey = "";

        public class DAL : IDisposable
        {
            public DAL(IConfiguration configuration)
            {
                SqlConnectionString = configuration.GetConnectionString("SqlConnectionString");
                SecretKey = configuration.GetValue<String>("SecretKey");
            }
            public SqlConnection? ClientSqlCon { get; set; }
            void IDisposable.Dispose()
            {
                if (ClientSqlCon != null)
                    ClientSqlCon.Dispose();
            }
            public String GetHomeBaseURL(HttpContext context)
            {
                var request = context.Request;
                string _baseURL = $"{request.Scheme}://{request.Host}";
                return _baseURL;
            }

            public UserRoleClass? GetSessionInfo(HttpContext? httpContext)
            {
                UserRoleClass? userRoleClass = null;
                ISession Session = httpContext.Session;
                if (httpContext.Session.IsAvailable)
                {
                    if (String.IsNullOrEmpty(Session.GetString("UserId")))
                    {
                    }
                    else
                    {
                        userRoleClass = new UserRoleClass();
                        userRoleClass.UserId = Session.GetString("UserId");
                        userRoleClass.RoleId = Session.GetString("RoleId");
                        userRoleClass.Roletype = Session.GetString("Roletype");
                        userRoleClass.Name = Session.GetString("Name");
                    }
                }
                return userRoleClass;
            }
            public Boolean IsUserLoggedIn(HttpContext? httpContext)
            {
                ISession Session = httpContext.Session;
                if (httpContext.Session.Keys.Count() > 0)
                {

                    if (String.IsNullOrEmpty(Session.GetString("UserId")))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    try
                    {
                        String cookieKey = "_egl";
                        var CookieLogin = httpContext.Request.Cookies[cookieKey];
                        if (CookieLogin != null)
                        {
                            String? encCookieString = CookieLogin.ToString();
                            String? decryptedCookieQueryString = Decrypt(encCookieString);

                            String? UserId = HttpUtility.ParseQueryString(decryptedCookieQueryString).Get("userid");
                            String? RoleId = HttpUtility.ParseQueryString(decryptedCookieQueryString).Get("roleid");
                            String? Roletype = HttpUtility.ParseQueryString(decryptedCookieQueryString).Get("Roletype");
                            String? Name = HttpUtility.ParseQueryString(decryptedCookieQueryString).Get("name");


                            httpContext.Session.SetString("UserId", UserId);
                            httpContext.Session.SetString("RoleId", RoleId);
                            httpContext.Session.SetString("Roletype", Roletype);
                            httpContext.Session.SetString("Name", Name);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch { return false; }
                }
            }


            public String CheckLogin(String email, String Password, HttpContext httpContext, bool keeplogin)
            {
                List<System.Data.SqlClient.SqlParameter> sqlParams = new List<System.Data.SqlClient.SqlParameter>();
                sqlParams.Add(new System.Data.SqlClient.SqlParameter("@email", email));
                sqlParams.Add(new System.Data.SqlClient.SqlParameter("@Password", Password));
                //sqlParams.Add(new System.Data.SqlClient.SqlParameter("@SecretKey", SecretKey));
                DataSet ds = SqlDataSetResult("USP_CheckLogin", sqlParams);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    UserMaster account = ConvertDataTableToClass<UserMaster>(ds.Tables[0]);
                    httpContext.Session.SetString("UserId", account.CON_DocNo.ToString());
                    httpContext.Session.SetString("Roletype", "counsellor");

                    httpContext.Session.SetString("Name", account.CON_Name.ToString());

                    if (keeplogin)
                    {
                        String cookieKey = "_egl";
                        var CookieLogin = httpContext.Request.Cookies[cookieKey];
                        if (CookieLogin == null)
                        {
                            String cookieQueryString = "userid=" + account.CON_DocNo + "&name=" + account.CON_Name + "&Roletype=" + "counsellor";
                            CookieOptions newCookieLogin = new CookieOptions();
                            newCookieLogin.Expires = DateTime.Now.AddYears(1);
                            String encryptedCookieQueryString = Encrypt(cookieQueryString);
                            httpContext.Response.Cookies.Append(cookieKey, encryptedCookieQueryString, newCookieLogin);
                        }
                    }

                    return "Userexist";

                }
                else
                {
                    return "NoUser";
                }

            }
            public String EncryptText(String text)
            {
                return Encrypt(text);
            }
            public String DecryptText(String text)
            {
                return Decrypt(text);
            }

            public DataSet SqlDataSetResult(String storedProcedureName, List<SqlParameter> sqlParams, Boolean isStoredProcedure = true)
            {
                DataSet ds = new DataSet();
                using (ClientSqlCon = new SqlConnection(SqlConnectionString))
                {
                    SqlTransaction transaction;
                    ClientSqlCon.Open();
                    transaction = ClientSqlCon.BeginTransaction();
                    try
                    {
                        SqlCommand clientSqlCmd = new SqlCommand(storedProcedureName, ClientSqlCon, transaction)
                        {
                            CommandType = (isStoredProcedure) ? CommandType.StoredProcedure : CommandType.Text,
                            CommandTimeout = 0
                        };

                        if (sqlParams.Count > 0)
                        {
                            clientSqlCmd.Parameters.AddRange(sqlParams.ToArray());
                        }

                        SqlDataAdapter da = new SqlDataAdapter(clientSqlCmd);
                        da.Fill(ds);
                        transaction.Commit();
                        ClientSqlCon.Close();
                    }
                    catch (Exception ex)
                    {
                        String? ACM_DOCNO = "";
                        try
                        {
                            SqlParameter? logID = sqlParams.Where(t => t.ParameterName == "USR_DocNo").FirstOrDefault();
                            ACM_DOCNO = (logID != null) ? logID.Value.ToString() : "";
                        }
                        catch { }
                        SaveERRORLogs(ACM_DOCNO, storedProcedureName, ex.Message, (ex.InnerException == null) ? "" : ex.InnerException.ToString());

                        transaction.Rollback();
                        ClientSqlCon.Close();
                    }
                }
                return ds;
            }
            private void SaveERRORLogs(String? ACM_DOCNO, String ErrorPage, String ErrorDetails, String? InnerException)
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@ACM_DOCNO", ACM_DOCNO));
                sqlParameters.Add(new SqlParameter("@ErrorPage", ErrorPage));
                sqlParameters.Add(new SqlParameter("@ErrorDetails", ErrorDetails));
                sqlParameters.Add(new SqlParameter("@InnerException", InnerException));

                DataSet dataSet = SqlDataSetResult("USP_SaveErrorLogs", sqlParameters);
            }

            public List<T> ConvertDataTableToListClass<T>(DataTable dt)
            {
                List<T> data = new List<T>();
                try
                {
                    data = dt.AsEnumerable().Select(row => GetItem<T>(row)).ToList();
                }
                catch { data = new List<T>(); }
                return data;
            }
            public T ConvertDataTableToClass<T>(DataTable dt) where T : new()
            {
                T item = new T();
                try
                {
                    DataRow row = dt.Rows[0];
                    // set the item
                    SetItemFromRow(item, row);
                }
                catch { }
                return item;
            }


            public string GetUniqueID()
            {
                string ID = "";
                ID = "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + DateTime.Now.Hour + DateTime.Now.Minute + "" + DateTime.Now.Second + "" + DateTime.Now.Millisecond;
                return ID;
            }
            public DateTime DateTimeNow()
            {
                DateTime todaysDate = DateTime.Now;
                //String DateTimeString = todaysDate.ToString("dd-MMM-yyy HH:mm:ss");
                //DateTime ResultDateTime = Convert.ToDateTime(DateTimeString);
                return todaysDate;
            }

            #region Private Methods
            private string Encrypt(string encryptString)
            {
                String MyEncryptionKey = SecretKey;
                byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(MyEncryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        encryptString = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return encryptString;
            }
            private string Decrypt(string cipherText)
            {
                String MyEncryptionKey = SecretKey;
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(MyEncryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            private void SetItemFromRow<T>(T item, DataRow row) where T : new()
            {
                foreach (DataColumn c in row.Table.Columns)
                {
                    PropertyInfo? p = item.GetType().GetProperty(c.ColumnName);
                    if (p != null && row[c] != DBNull.Value)
                    {
                        p.SetValue(item, row[c], null);
                    }
                }
            }
            private T GetItem<T>(DataRow dr)
            {
                Type temp = typeof(T);
                T obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in dr.Table.Columns)
                {
                    PropertyInfo? pro = temp.GetProperties().Where(t => t.Name == column.ColumnName).FirstOrDefault();
                    if (pro != null)
                    {
                        var cellValue = dr[column.ColumnName];
                        cellValue = (cellValue == DBNull.Value) ? "" : cellValue;
                        pro.SetValue(obj, cellValue, null);
                    }
                }
                return obj;
            }
            #endregion Private Methods


            public String DataTableToJSON(Object table)
            {
                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(table);
                return JSONString;
            }

        }

     
        private class UserMaster
        {
            public string? CON_DocNo { get; set; }
            public string? CON_DocType { get; set; }
            public string? CON_Name { get; set; }
            public string? CON_Email { get; set; }
            public string CON_Password { get; set; }
            public string CON_Status { get; set; }

        }

        

        public class UserRoleClass
        {
            public String? UserId { get; set; }
            // public String? UserIdadmin { get; set; }
            public String? RoleId { get; set; }
            public String? Roletype { get; set; }
            // public String? Roletypeadmin { get; set; }
            public String? Name { get; set; }
        }






       
       
        


    }
}
