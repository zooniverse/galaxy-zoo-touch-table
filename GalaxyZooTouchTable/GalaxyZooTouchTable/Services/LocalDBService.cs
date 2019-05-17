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
        string IncrementClassificationCountQuery(int count, string id) { return $"update Subjects set classifications_count = {count} where subject_id = {id}"; }

        public IGraphQLService _graphQLService { get; private set; }

        string SubjectsWithinBoundsQuery(SpaceNavigation location)
        {
            return $"select * from Subjects where dec > {location.MinDec} and dec < {location.MaxDec} and ra > {location.MinRa} and ra < {location.MaxRa}";
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

        public List<TableSubject> GetLocalSubjects(SpaceNavigation currentLocation)
        {
            string query = SubjectsWithinBoundsQuery(currentLocation);
            return GetSubjects(query, currentLocation);
        }

        public List<TableSubject> GetSubjects(string query, SpaceNavigation currentLocation = null)
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
                        TableSubject RetrievedSubject = new TableSubject(id, image, ra, dec, currentLocation);
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

        public int GetClassificationCount(string subjectId)
        {
            string query = SubjectByIdQuery(subjectId);
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                int count = 0;
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        count = reader.GetInt16(1); ;
                    }

                    connection.Close();
                }
                catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                return count;
            }
        }

        public int IncrementClassificationCount(int count, string subjectId)
        {
            int newCount = count += 1;
            string query = IncrementClassificationCountQuery(newCount, subjectId);
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                }
                catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                return newCount;
            }
        }

        public SpacePoint GetRandomPoint()
        {
            return GetPoint(RandomSubjectQuery) ?? new SpacePoint(0,0);
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

        public int IncrementClassificationCount(string subjectId)
        {
            int count = GetClassificationCount(subjectId);
            return IncrementClassificationCount(count, subjectId);
        }
    }
}
