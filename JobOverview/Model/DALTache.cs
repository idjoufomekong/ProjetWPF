using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobOverview.Entity;

namespace JobOverview.Model
{
    public class DALTache
    {
        //Liste tâches pour visualiser
        public static List<TacheBrut> RecupererTachesBrut(string codeLogiciel,float version,string codeManager)
        {
            List<TacheBrut> listTache = new List<TacheBrut>();

            var connectString = Properties.Settings.Default.JobOverviewConnectionString;
            //Sélection de toutes les tâches (production et annexes) avec leurs travaux et l'employé affecté
            //Les tâches sans travaux sont incluses ainsi que les employés sans tâches
            //On récupère les 50 dernières modifications
            //Pour la version et le logiciel sélectionnés
            //Pour le manager connecté
            string queryString = @"select top(50) P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,P.Manager,T.IdTache,T.Libelle,
T.CodeActivite,
T.Description, T.Annexe,
TP.Numero,TP.DureePrevue,TP.DureeRestanteEstimee,TP.CodeLogicielVersion,TP.CodeModule,TP.NumeroVersion,TR.DateTravail,TR.Heures,
TR.TauxProductivite
from jo.Personne P
left outer join jo.Tache T on T.Login=P.Login
left outer join jo.TacheProd TP on T.IdTache=TP.IdTache
left outer join jo.Travail TR on  TR.IdTache=T.IdTache 
where CodeLogicielVersion=@codeLogiciel and NumeroVersion=@numVersion and Manager=@manager
UNION
select top(50) P.Login, P.Prenom+' '+P.Nom NomComplet,P.CodeMetier,P.Manager,T.IdTache,T.Libelle,
T.CodeActivite,
T.Description, T.Annexe,
TP.Numero,TP.DureePrevue,TP.DureeRestanteEstimee,TP.CodeLogicielVersion,TP.CodeModule,TP.NumeroVersion,TR.DateTravail,TR.Heures,
TR.TauxProductivite
from jo.Personne P
left outer join jo.Tache T on T.Login=P.Login
left outer join jo.TacheProd TP on T.IdTache=TP.IdTache
left outer join jo.Travail TR on  TR.IdTache=T.IdTache 
where CodeLogicielVersion=@codeLogiciel and NumeroVersion=@numVersion and P.Login=@manager
order by DateTravail desc,login, Numero"; //TODO: Epurer la requête et retirer les champs inutiles

            var codeLog = new SqlParameter("@codeLogiciel", DbType.String);
            var numVersion = new SqlParameter("@numVersion", DbType.Double);
            var codeMng = new SqlParameter("@manager", DbType.Double);
            codeLog.Value = codeLogiciel;
            numVersion.Value = version;
            codeMng.Value = codeManager;

            using (var connect = new SqlConnection(connectString))
            {
                //Récupération liste des tâches de production
                var command = new SqlCommand(queryString, connect);
                command.Parameters.Add(codeLog);
                command.Parameters.Add(numVersion);
                command.Parameters.Add(codeMng);
                connect.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecupererTachesBrutFromDataReader(listTache, reader);
                    }
                }
            }

            return listTache;
        }

        private static void RecupererTachesBrutFromDataReader(List<TacheBrut> listTache, SqlDataReader reader)
        {
            TacheBrut tache = new TacheBrut();

            if (reader["IdTache"] != DBNull.Value)
                tache.IdTache = (Guid)reader["IdTache"];
            if (reader["Libelle"] != DBNull.Value)
                tache.NomTache = (string)reader["Libelle"];
            if (reader["Annexe"] != DBNull.Value)
                tache.Annexe = (bool)reader["Annexe"];
            if (reader["CodeActivite"] != DBNull.Value)
                tache.CodeActivite = (string)reader["CodeActivite"];
            if (reader["Login"] != DBNull.Value)
                tache.Login = (string)reader["Login"];
            if (reader["Description"] != DBNull.Value)
                tache.Description = (string)reader["Description"];

            if (reader["Numero"] != DBNull.Value)
                tache.NumTache = (int)reader["Numero"];
            if (reader["DureePrevue"] != DBNull.Value)
                tache.DureePrevue = (float)reader["DureePrevue"];
            if (reader["DureeRestanteEstimee"] != DBNull.Value)
                tache.DureeRestante = (float)reader["DureeRestanteEstimee"];
            if (reader["CodeModule"] != DBNull.Value)
                tache.CodeModule = (string)reader["CodeModule"];
            if (reader["NumeroVersion"] != DBNull.Value)
                tache.CodeVersion = (float)reader["NumeroVersion"];
            if (reader["CodeLogicielVersion"] != DBNull.Value)
                tache.CodeLogiciel = (string)reader["CodeLogicielVersion"];
            if (reader["DateTravail"] != DBNull.Value)
                tache.Date = (DateTime)reader["DateTravail"];
            if (reader["Heures"] != DBNull.Value)
                tache.Heures = (float)reader["Heures"];
            if (reader["TauxProductivite"] != DBNull.Value)
                tache.TauxProductivite = (float)reader["TauxProductivite"];

            listTache.Add(tache);
        }
    }
}
