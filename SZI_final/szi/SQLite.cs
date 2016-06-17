using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;


namespace SZI
{
    class SQLite
    {
            string database;
            SQLiteCommand query;
            SQLiteConnection connection;
            SQLiteDataReader reader;
            //КОНСТРУКТОР
            public SQLite()
            {
                database = Environment.CurrentDirectory + "\\database.sqlite";
                connection = new SQLiteConnection(string.Format("Data Source={0}", database));
                connection.Open();

            }
            //чтение данных
            public SQLiteDataReader ReadData(string query)
            {

                this.query = new SQLiteCommand(query, connection);
                reader = this.query.ExecuteReader();
                return reader;

            }
            //запись данных
            public void WriteData(string query)
            {
                this.query = new SQLiteCommand(query, connection);
                this.query.ExecuteNonQuery();
                Close();
            }
            //закрытие подключения
            public void Close()
            {
                connection.Close();
            }
       
}
}
