using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimchaFund.Data
{
    public class SimchaFundManager
    {
        private string _connectionString;

        public SimchaFundManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddSimcha(Simcha simcha)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            using (var cmd = sqlConnection.CreateCommand())
            {
                sqlConnection.Open();
                cmd.CommandText = "INSERT INTO Simchas (Name, Date) VALUES (@name, @date); SELECT @@Identity";
                cmd.Parameters.AddWithValue("@name", simcha.Name);
                cmd.Parameters.AddWithValue("@date", simcha.Date);
                simcha.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public IEnumerable<Simcha> GetAllSimchas()
        {
            var simchas = new List<Simcha>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Simchas";
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var simcha = new Simcha();
                    simcha.Id = (int)reader["Id"];
                    simcha.Date = (DateTime)reader["Date"];
                    simcha.Name = (string)reader["Name"];
                    SetSimchaTotals(simcha);
                    simchas.Add(simcha);
                }
            }

            return simchas;
        }

        public int GetContributorCount()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
                connection.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void AddContributor(Contributor contributor)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            using (var cmd = sqlConnection.CreateCommand())
            {
                sqlConnection.Open();
                cmd.CommandText = @"INSERT INTO Contributors (FirstName, LastName, CellNumber, Date, AlwaysInclude) 
                                    VALUES (@firstName, @lastName, @cellNumber, @date, @alwaysInclude); SELECT @@Identity";
                cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
                cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
                cmd.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
                cmd.Parameters.AddWithValue("@date", contributor.Date);
                cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                contributor.Id = (int)(decimal)cmd.ExecuteScalar();

            }
        }

        public IEnumerable<Contributor> GetContributors()
        {
            var contributors = new List<Contributor>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                connection.Open();
                cmd.CommandText = "SELECT * FROM Contributors";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var contributor = new Contributor();
                    contributor.Id = (int)reader["Id"];
                    contributor.FirstName = (string)reader["FirstName"];
                    contributor.LastName = (string)reader["LastName"];
                    contributor.CellNumber = (string)reader["CellNumber"];
                    contributor.Date = (DateTime)reader["Date"];
                    contributor.AlwaysInclude = (bool)reader["AlwaysInclude"];
                    SetContributorBalance(contributor);
                    contributors.Add(contributor);
                }
            }

            return contributors;
        }

        private void SetSimchaTotals(Simcha simcha)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT ISNull(SUM(Amount), 0) as Total, Count(*) as ContributorAmount
                FROM Contributions
                WHERE SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simcha.Id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                reader.Read();
                simcha.ContributorAmount = (int)reader["ContributorAmount"];
                simcha.Total = (decimal)reader["Total"];
            }
        }

        private void SetContributorBalance(Contributor contributor)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                connection.Open();
                cmd.CommandText = "SELECT IsNull(SUM(Amount),0) FROM Deposits WHERE ContributorId = @contribId";
                cmd.Parameters.AddWithValue("@contribId", contributor.Id);
                decimal depositTotal = (decimal)cmd.ExecuteScalar();

                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT IsNull(SUM(Amount),0) FROM Contributions WHERE ContributorId = @contribId";
                cmd.Parameters.AddWithValue("@contribId", contributor.Id);
                decimal contibutionsTotal = (decimal)cmd.ExecuteScalar();
                contributor.Balance = depositTotal - contibutionsTotal;
            }
        }
    }
}