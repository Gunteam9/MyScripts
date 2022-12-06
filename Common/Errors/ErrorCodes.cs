using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Errors
{
    public static class ErrorCodes
    {
        /// <summary>
        /// Cette fonction renvoie le message à afficher au joueur
        /// </summary>
        public static string GetMessage(int code)
        {
            switch (code)
            {
                case SQL_ERROR:
                case BAD_TRANSACTION_TYPE: 
                    return $"Une erreur est survenue. Code : {code}";
                default:
                    return $"Une erreur inconnue est survenue";
            }
        }

        /// <summary>
        /// Erreur SQL
        /// </summary>
        public const int SQL_ERROR = 0x00000001;

        /// <summary>
        /// Erreur de type de transaction dans la fonction spécificée
        /// </summary>
        public const int BAD_TRANSACTION_TYPE = 0x00000010;
    }
}
