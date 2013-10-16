using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace GetData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string connecttionString =
                @"Data Source=(localdb)\Projects;Initial Catalog=DappaerSample;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
            using (var con = new SqlConnection(connecttionString))
            {
                con.Open();
                //データ追加
                con.Execute("INSERT Log(TimeStamp, Description) VALUES(@tm, @desc)",
                    new {tm = DateTime.Now, desc = "Description"});
                //データ抽出
                var logs = con.Query<Log>("SELECT Id, TimeStamp, Description FROM Log");
                logs.ToList().ForEach(l => Console.WriteLine("{0}, {1}, {2}", l.Id, l.TimeStamp, l.Description));
                //ストアードプロシージャの実行
                Console.WriteLine("ストアードプロシージャの実行");
                var logs2 = con.Query<Log>("GetLogs", new { param1 = DateTime.Now.AddHours(1.0) },
                    commandType: CommandType.StoredProcedure);
                logs2.ToList().ForEach(l => Console.WriteLine("{0}, {1}, {2}", l.Id, l.TimeStamp, l.Description));

                Console.WriteLine("終了するにはEnterキーを押して下さい。");
                Console.ReadLine();
            }
        }
    }

    public class Log
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Description { get; set; }
    }
}