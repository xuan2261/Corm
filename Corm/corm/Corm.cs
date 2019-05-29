using System;
using System.Data.SqlClient;
using CORM.utils;

namespace CORM
{
    /*
     * CORM -- A Simple ORM Framework for C# and SqlServer
     * Author : Ericwyn
     * Github: https://github.com/Ericwyn/Corm
     * CreateDate: 2019/05/16
     */
    public class Corm
    {
        public SqlConnection _sqlConnection
        {
            get
            {
                SqlConnection conn = new SqlConnection(this.ConnectStr);
                conn.Open();
                return conn;
            }
        }

        public string ConnectStr { get; }
        public CormLogUtils LogUtils { get; }

        private Corm(string connectionStr, CormLogUtils logUtils)
        {
            this.ConnectStr = connectionStr;
            this.LogUtils = logUtils;
        }

        // ---------------------------------- Build 模式 --------------------------------------
        public class CormBuilder
        {
            private class _defaultSqlPrintCB : CormSqlPrintCB
            {
                public void SqlPrint(string sql)
                {
                    Console.WriteLine("\n[Corm Log] -------------------------------");
                    Console.WriteLine(sql);
                    Console.WriteLine("-------------------------------------------- \n");
                }
            }
        
            private string _connectStr = "";
            private CormSqlPrintCB _sqlPrintCb = new _defaultSqlPrintCB();
//        private bool _allDebugLog = false;
        
            public CormBuilder(){}

            public CormBuilder Server(string connectStr)
            {
                this._connectStr = connectStr;
                return this;
            }

            public CormBuilder SqlPrint(CormSqlPrintCB cb)
            {
                this._sqlPrintCb = cb;
                return this;
            }

//        public CormBuilder DebugLog(bool allDebugLog)
//        {
//            _allDebugLog = allDebugLog;
//            return this;
//        }

            public Corm Build()
            {
                if (_connectStr == null || _connectStr.Trim() == "")
                {
                    throw new CormException("创建 Corm 对象时候，ConnectStr 错误");
                }
                return new Corm(_connectStr, new CormLogUtils(this._sqlPrintCb));
            }
        }
        
    }

    
}