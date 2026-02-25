using System.IO;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace AgTrackersJson2Sqlite
{
    class Program
    {
        static void Main(string[] args)
        {
            string jsonFilePath = "trackers.json";
            string dbFilePath = "trackers.db";

            string jsonString = File.ReadAllText(jsonFilePath);
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;
            JsonElement trackersObj = root.GetProperty("trackers");
            JsonElement trackerDomainsObj = root.GetProperty("trackerDomains");

            using var connection = new SqliteConnection($"Data Source={dbFilePath}");
            connection.Open();

            CreateTables(connection);
            InsertTrackers(connection, trackersObj);
            InsertTrackerDomains(connection, trackerDomainsObj);

            Console.WriteLine("数据导入完成！");
        }

        static void CreateTables(SqliteConnection connection)
        {
            // 创建 trackers 表
            string createTrackersTable = @"
            CREATE TABLE IF NOT EXISTS trackers (
                id TEXT PRIMARY KEY,
                name TEXT,
                category_id INTEGER,
                website_url TEXT,
                company_id TEXT,
                ghostery_id TEXT,
                notes TEXT
            );";

            using var cmd1 = new SqliteCommand(createTrackersTable, connection);
            cmd1.ExecuteNonQuery();

            // 创建 tracker_domains 表
            string createDomainsTable = @"
            CREATE TABLE IF NOT EXISTS tracker_domains (
                tracker TEXT,
                domain TEXT PRIMARY KEY,
                notes TEXT
            );";

            using var cmd2 = new SqliteCommand(createDomainsTable, connection);
            cmd2.ExecuteNonQuery();
        }

        static void InsertTrackers(SqliteConnection connection, JsonElement trackersObj)
        {
            foreach (var trackerProperty in trackersObj.EnumerateObject())
            {
                string id = trackerProperty.Name;
                JsonElement tracker = trackerProperty.Value;

                string name = tracker.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
                int? categoryId = tracker.TryGetProperty("categoryId", out var catProp) ? catProp.GetInt32() : (int?)null;
                string websiteUrl = tracker.TryGetProperty("url", out var urlProp) ? urlProp.GetString() : null;
                string companyId = tracker.TryGetProperty("companyId", out var companyProp) ? companyProp.GetString() : null;

                string ghosteryId = companyId;

                // 如果存在 source 字段，可以将其存入 notes
                string notes = null;
                if (tracker.TryGetProperty("source", out var sourceProp))
                {
                    notes = sourceProp.GetString();
                }

                string insertSql = @"
                INSERT OR REPLACE INTO trackers (id, name, category_id, website_url, company_id, ghostery_id, notes)
                VALUES ($id, $name, $categoryId, $websiteUrl, $companyId, $ghosteryId, $notes);";

                using var cmd = new SqliteCommand(insertSql, connection);
                cmd.Parameters.AddWithValue("$id", id);
                cmd.Parameters.AddWithValue("$name", (object)name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$categoryId", (object)categoryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$websiteUrl", (object)websiteUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$companyId", (object)companyId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$ghosteryId", (object)ghosteryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$notes", (object)notes ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        static void InsertTrackerDomains(SqliteConnection connection, JsonElement trackerDomainsObj)
        {
            foreach (var domainProperty in trackerDomainsObj.EnumerateObject())
            {
                string domain = domainProperty.Name;
                string trackerId = domainProperty.Value.GetString();

                string insertSql = @"
                INSERT OR REPLACE INTO tracker_domains (tracker, domain, notes)
                VALUES ($tracker, $domain, $notes);";

                using var cmd = new SqliteCommand(insertSql, connection);
                cmd.Parameters.AddWithValue("$tracker", trackerId);
                cmd.Parameters.AddWithValue("$domain", domain);
                cmd.Parameters.AddWithValue("$notes", DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
