using GalaxyZooTouchTable.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GalaxyZooTouchTable.Services
{
    public static class LocalDBService
    {
        public static List<TableSubject> GetLocalSubjects(double minRa = 0, double maxRa = 360, double minDec = -90, double maxDec = 90)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subjects where dec > {minDec} and dec < {maxDec} and ra > {minRa} and ra < {maxRa}";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                List<TableSubject> SubjectsWithinBounds = new List<TableSubject>();

                while (reader.Read())
                {
                    string id = reader["subject_id"] as string;
                    string image = reader["image"] as string;
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    TableSubject subjectFromDatabase = new TableSubject(id, image, ra, dec);
                    SubjectsWithinBounds.Add(subjectFromDatabase);
                }

                connection.Close();
                return SubjectsWithinBounds;
            }
        }

        public static TableSubject GetLocalSubject(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subjects where subject_id = {id}";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                TableSubject RetrievedSubject = null;

                while (reader.Read())
                {
                    string image = reader["image"] as string;
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    RetrievedSubject = new TableSubject(id, image, ra, dec);
                }

                connection.Close();
                return RetrievedSubject;
            }
        }

        public static List<TableSubject> GetQueuedSubjects()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subjects order by classifications_count asc limit 10";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                List<TableSubject> Subjects = new List<TableSubject>();

                while (reader.Read())
                {
                    string id = reader["subject_id"] as string;
                    string image = reader["image"] as string;
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    TableSubject RetrievedSubject = new TableSubject(id, image, ra, dec);
                    Subjects.Add(RetrievedSubject);
                }

                connection.Close();
                return Subjects;
            }
        }

        public static SpacePoint GetRandomPoint()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subjects order by random() limit 1";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                SpacePoint point = null;

                while (reader.Read())
                {
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    point = new SpacePoint(ra, dec);
                }

                connection.Close();
                return point;
            }
        }

        public static SpacePoint FindNextAscendingRa(double RaLowerBounds)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subjects where ra > {RaLowerBounds} limit 1";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                SpacePoint point = null;

                while (reader.Read())
                {
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    point = new SpacePoint(ra, dec);
                }

                connection.Close();
                return point;
            }
        }

        public static SpacePoint FindNextDescendingRa(double RaUpperBounds)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subjects where ra < {RaUpperBounds} limit 1";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                SpacePoint point = null;

                while (reader.Read())
                {
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    point = new SpacePoint(ra, dec);
                }

                connection.Close();
                return point;
            }
        }

        public static SpacePoint FindNextAscendingDec(double DecLowerBounds)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subjects where dec > {DecLowerBounds} limit 1";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                SpacePoint point = null;

                while (reader.Read())
                {
                    double dec = (double)reader["dec"];
                    double ra = (double)reader["ra"];
                    point = new SpacePoint(ra, dec);
                }

                connection.Close();
                return point;
            }
        }

        public static SpacePoint FindNextDescendingDec(double DecUpperBounds)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subjects where dec < {DecUpperBounds} limit 1";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                SpacePoint point = null;

                while (reader.Read())
                {
                    double dec = (double)reader["dec"];
                    double ra = (double)reader["ra"];
                    point = new SpacePoint(ra, dec);
                }

                connection.Close();
                return point;
            }
        }
    }
}
