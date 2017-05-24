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
        /// Récupération de la liste de tous les logiciels et version,  
        /// présentes dans les listes déroulantes de toutes les pages
        /// </summary>
        /// <returns>Liste de logiciel</returns>
        public static List<Logiciel> RecupererLogicielsVersions()
        {
            List<Logiciel> listLogiciel = new List<Logiciel>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            string queryString = @"select L.CodeLogiciel,Nom,NumeroVersion
                                 from jo.Logiciel L
                                 inner join jo.Version V on V.CodeLogiciel=L.CodeLogiciel
                                 order by CodeLogiciel,NumeroVersion";

            using (var connect = new SqlConnection(connectString))
            {
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
        /// <param name="listLogiciel">Liste de logiciel</param>
        /// <param name="reader">donnée du reader</param>
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
            vers.NumVersion = (float)reader["NumeroVersion"];
            log.Versions.Add(vers);
        }

        /// <summary>
        /// Récupération de la liste des modules en fonction du logiciel donné
        /// </summary>
        /// <param name="logiciel">Logiciel lié aux modules</param>
        /// <returns>Liste de de module</returns>
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

        /// <summary>
        /// Récupère la liste des logiciels avec leurs modules et versions pour la synthèse 
        /// </summary>
        /// <returns>Liste de tous les logiciels</returns>
        public static List<Logiciel> RecupererLogicielSynthese()
        {
            List<Logiciel> listLogiciel = new List<Logiciel>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            string queryString = @"select L.CodeLogiciel, L.Nom, M.CodeModule, M.Libelle, sum(TR.Heures) travaille
                                 from jo.Logiciel L
                                 inner join jo.Module M on M.CodeLogiciel=L.CodeLogiciel
                                 inner join jo.TacheProd TP on TP.CodeModule= M.CodeModule
                                 inner join jo.Tache T on T.IdTache=TP.IdTache
                                 inner join jo.Travail TR on TR.IdTache=T.IdTache
                                 group by L.CodeLogiciel, L.Nom, M.CodeModule, M.Libelle
                                 order by 1,3";

            using (var connect = new SqlConnection(connectString))
            {
                var command = new SqlCommand(queryString, connect);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecupererLogicielsModulesFromDataReader(listLogiciel, reader);
                    }

                }
                    foreach (var a in listLogiciel)
                    {
                        a.Versions = RecupererVersionsSynthese(a.CodeLogiciel, connect);
                    }
                return listLogiciel;
            }
        }


        /// <summary>
        /// Lecture du dataread retourné pr la requête SQL de récupération des logiciels et versions
        /// </summary>
        /// <param name="listLogiciel">Liste de logiciel</param>
        /// <param name="reader">donnée du reader</param>
        private static void RecupererLogicielsModulesFromDataReader(List<Logiciel> listLogiciel, SqlDataReader reader)
        {
            string codeLogiciel = (string)reader["CodeLogiciel"];
            Logiciel log = null;
            if ((listLogiciel.Count == 0) || (listLogiciel.Last().CodeLogiciel != codeLogiciel))
            {
                log = new Logiciel();
                log.CodeLogiciel = (string)reader["CodeLogiciel"];
                log.NomLogiciel = (string)reader["Nom"];

                log.Modules = new List<Module>();
                listLogiciel.Add(log);
            }

            log = listLogiciel.Last();
            Module mod = new Module();
            mod.CodeModule = (string)reader["CodeModule"];
            mod.NomModule = (string)reader["Libelle"];
            mod.TempsRealise =(double)reader["travaille"]/8; //Pour avoir le nombre de jours
            log.Modules.Add(mod);
        }

        /// <summary>
        /// Pour chaque logiciel entré en paramètres, récupère la liste des versions
        /// </summary>
        /// <param name="codeLogiciel">Identifiant du logiciel</param>
        /// <param name="connect">Connection à la base</param>
        /// <returns></returns>
        public static List<Entity.Version> RecupererVersionsSynthese(string codeLogiciel, SqlConnection connect)
        {
            List<Entity.Version> listVersion = new List<Entity.Version>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            string queryString = @"select V.CodeLogiciel,R.NumeroVersion, V.NumeroVersion,V.DateSortiePrevue, V.DateSortieReelle, sum(TR.Heures) travaille,COUNT(R.NumeroRelease) nbRelease
                                    from jo.Version V 
                                    inner join jo.Release R on R.NumeroVersion=V.NumeroVersion
                                    inner join jo.TacheProd TP on TP.NumeroVersion=V.NumeroVersion
                                    inner join jo.Tache T on T.IdTache=TP.IdTache
                                    inner join jo.Travail TR on TR.IdTache=T.IdTache
                                    where V.CodeLogiciel=@codeLogiciel
                                    group by  V.CodeLogiciel,R.NumeroVersion, V.NumeroVersion,V.DateSortiePrevue, V.DateSortieReelle
                                    order by 3";

            var param = new SqlParameter("@codeLogiciel", DbType.String);
            param.Value = codeLogiciel;

            var command = new SqlCommand(queryString, connect);
            command.Parameters.Add(param);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    RecupererVersionsFromDataReader(listVersion, reader);
                }
            }
            return listVersion;
        }

        private static void RecupererVersionsFromDataReader(List<Entity.Version> listVersion, SqlDataReader reader)
        {

            Entity.Version vers = new Entity.Version();

                vers.NumVersion = (float)reader["NumeroVersion"];
                vers.DateSortiePrevue = (DateTime)reader["DateSortiePrevue"];
                if(reader["DateSortieReelle"]!=DBNull.Value)
                    vers.DateSortieReelle = (DateTime)reader["DateSortieReelle"];
                vers.TempsTotalRealise = (double)reader["travaille"];
                vers.NombreReleases = (int)reader["nbRelease"];

            listVersion.Add(vers);

        }
    }
}
