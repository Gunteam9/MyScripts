using Common.Errors;
using MySql.Data.MySqlClient;

namespace CommonServer
{
    /// <summary>
    ///     Crud basique permettant des opérations de base sur la base de données
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ACrud<T> where T : new()
    {
        /// <summary>
        ///     String de connexion à la base de donnée
        /// </summary>
        private const string connString =
            "Server=51.91.98.193;Database=FMSQL;port=3306;User Id=FM_USER;password=FM_PASSWD";

        protected readonly MySqlConnection connection;

        protected ACrud()
        {
            connection = new MySqlConnection(connString);
        }

        /// <summary>
        ///     Retourne l'objet ayant cet identifiant dans la base
        /// </summary>
        /// <exception cref="RssException"></exception>
        /// <returns>
        ///     OK : L'objet sauvegardé dans la base <br />
        ///     KO : <see langword="null" />
        /// </returns>
        public abstract T Get(long id);

        /// <summary>
        ///     Sauvegarde l'objet en base
        /// </summary>
        /// <exception cref="RssException"></exception>
        /// <param name="objectToSave">L'objet à sauvegarder</param>
        /// <returns>L'objet sauvegardé en base</returns>
        public abstract T Insert(T objectToSave);

        /// <summary>
        ///     Met à jour l'objet correspond à l'id de l'objet en paramètre
        /// </summary>
        /// <exception cref="RssException"></exception>
        /// <param name="objectToSave">L'objet à mettre à jour</param>
        /// <returns>
        ///     <see langword="true" /> : La mise à jour a fonctionné correctement
        ///     <see langword="false" /> : Une erreur non bloquant est survenue
        /// </returns>
        public abstract bool Update(T objectToSave);

        /// <summary>
        ///     Supprime l'objet en base
        /// </summary>
        /// <exception cref="RssException"></exception>
        /// <returns>
        ///     OK : <see langword="true" /> <br />
        ///     KO : <see langword="false" />
        /// </returns>
        public abstract bool Remove(long id);
    }
}