using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobOverview.Entity;
using System.Data;

namespace JobOverview.Model
{
    public class DALLogiciel
    {
        /// <summary>
        /// Récupération de la liste de tous les logiciels et versions qui seront dans les listes déroulantes de toutes 
        /// les pages
        /// </summary>
        /// <returns></returns>
        public static List<Logiciel> RecupererLogicielsVersions()
        {
            List<Logiciel> listLogiciel = new List<Logiciel>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            string queryString = @"select L.CodeLogiciel,Nom,NumeroVersion
from jo.Logiciel L
inner join jo.Version V on V.CodeLogiciel=L.CodeLogiciel
order by CodeLogiciel,NumeroVersion";//TODO Mettre la version et le logiciel en paramètres  Manager='BNORMAND' and

            using (var connect = new SqlConnection(connectString))
            {
                //Récupération liste des tâches de production
                var command = new SqlCommand(queryString, connect);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecupererLogicielsVersionsFromDataReader(listLogiciel, reader);
                    }
                }
            }

            return listLogiciel;
        }

        /// <summary>
        /// Lecture du dataread retourné pr la requête SQL de récupération des logiciels et versions
        /// </summary>
        /// <param name="listLogiciel"></param>
        /// <param name="reader"></param>
        private static void RecupererLogicielsVersionsFromDataReader(List<Logiciel> listLogiciel, SqlDataReader reader)
        {
            string codeLogiciel = (string)reader["CodeLogiciel"];
            Logiciel log = null;
            if ((listLogiciel.Count == 0) || (listLogiciel.Last().CodeLogiciel != codeLogiciel))
            {
                log = new Logiciel();
                log.CodeLogiciel = (string)reader["CodeLogiciel"];
                log.NomLogiciel = (string)reader["Nom"];

                log.Versions = new List<Entity.Version>();
                listLogiciel.Add(log);
            }
            else
            {
                log = listLogiciel.Last();
            }
            Entity.Version vers = new Entity.Version();
            vers.NumVersion= (float)reader["NumeroVersion"];
            log.Versions.Add(vers);
        }

        public static List<Module> RecupererModules(string logiciel)
        {
            List<Module> listModule = new List<Module>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            string queryString = @"select CodeModule,Libelle from jo.Module
 where CodeLogiciel=@logiciel
order by 2";

            var param = new SqlParameter("@logiciel", DbType.String);
            param.Value = logiciel;

            using (var connect = new SqlConnection(connectString))
            {
                //Récupération liste des tâches de production
                var command = new SqlCommand(queryString, connect);
                command.Parameters.Add(param);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Module mod = new Entity.Module();
                        mod.CodeModule = (string)reader["CodeModule"];
                        mod.NomModule = (string)reader["Libelle"];
                        listModule.Add(mod);
                    }
                }
            }

            return listModule;
        }

    }
}
