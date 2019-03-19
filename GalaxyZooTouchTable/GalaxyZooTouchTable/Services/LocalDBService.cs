using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GalaxyZooTouchTable.Services
{
    public class LocalDBService : ILocalDBService
    {
        string HighestRaQuery = "select * from Subjects order by ra desc limit 1";
        string HighestDecQuery = "select * from Subjects order by dec desc limit 1";
        string LowestRaQuery = "select * from Subjects order by ra asc limit 1";
        string LowestDecQuery = "select * from Subjects order by dec asc limit 1";
        string QueuedSubjectsQuery = "select * from Subjects order by classifications_count asc limit 10";
        string RandomSubjectQuery = "select * from Subjects order by random() limit 1";
        string SubjectByIdQuery(string id) { return $"select * from Subjects where subject_id = {id}"; }
        string NextAscendingRaQuery(double bounds) { return $"select * from Subjects where ra > {bounds} order by ra asc limit 1"; }
        string NextDescendingRaQuery(double bounds) { return $"select * from Subjects where ra < {bounds} order by ra desc limit 1"; }
        string NextAscendingDecQuery(double bounds) { return $"select * from Subjects where dec > {bounds} order by dec asc limit 1"; }
        string NextDescendingDecQuery(double bounds) { return $"select * from Subjects where dec < {bounds} order by dec desc limit 1"; }

        string SubjectsWithinBoundsQuery(double minRa, double minDec, double maxRa, double maxDec)
        {
            return $"select * from Subjects where dec > {minDec} and dec < {maxDec} and ra > {minRa} and ra < {maxRa}";
        }

        public TableSubject GetLocalSubject(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                TableSubject RetrievedSubject = null;
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(SubjectByIdQuery(id), connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string image = reader["image"] as string;
                        double ra = (double)reader["ra"];
                        double dec = (double)reader["dec"];
                        RetrievedSubject = new TableSubject(id, image, ra, dec);
                    }

                    connection.Close();
                } catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                return RetrievedSubject;
            }
        }

        public List<TableSubject> GetQueuedSubjects()
        {
            return GetSubjects(QueuedSubjectsQuery);
        }

        public List<TableSubject> GetLocalSubjects(double minRa = 0, double maxRa = 360, double minDec = -90, double maxDec = 90)
        {
            string query = SubjectsWithinBoundsQuery(minRa, minDec, maxRa, maxDec);
            return GetSubjects(query);
        }

        public List<TableSubject> GetSubjects(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                List<TableSubject> Subjects = new List<TableSubject>();
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

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
                } catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                return Subjects;
            }
        }

        public SpacePoint GetPoint(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                SpacePoint point = null;
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        double ra = (double)reader["ra"];
                        double dec = (double)reader["dec"];
                        point = new SpacePoint(ra, dec);
                    }

                    connection.Close();
                } catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                return point;
            }
        }

        public SpacePoint GetRandomPoint()
        {
            return GetPoint(RandomSubjectQuery);
        }

        public SpacePoint FindNextAscendingRa(double bounds)
        {
            return GetPoint(NextAscendingRaQuery(bounds)) ?? GetPoint(LowestRaQuery);
        }

        public SpacePoint FindNextDescendingRa(double bounds)
        {
            return GetPoint(NextDescendingRaQuery(bounds)) ?? GetPoint(HighestRaQuery);
        }

        public SpacePoint FindNextAscendingDec(double bounds)
        {
            return GetPoint(NextAscendingDecQuery(bounds)) ?? GetPoint(LowestDecQuery);
        }

        public SpacePoint FindNextDescendingDec(double bounds)
        {
            return GetPoint(NextDescendingDecQuery(bounds)) ?? GetPoint(HighestDecQuery);
        }
    }
}
