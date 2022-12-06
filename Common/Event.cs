using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Event
    {
        /// <summary>
        /// Events partagés
        /// </summary>
        public struct Shared
        {
            public struct Common
            {
                /// <summary>
                /// Affiche une notification au joueur
                /// </summary>
                public const string ShowNotification = "shared::common::ShowNotification";
            }
        }

        /// <summary>
        /// Events client uniquement
        /// </summary>
        public struct Client
        {
            public struct FiveM
            {
                /// <summary>
                /// Event déclenché au chargement de la ressource
                /// </summary>
                public const string OnClientResourceStart = "onClientResourceStart";
            }
        }

        /// <summary>
        /// Events serveur uniquemnet
        /// </summary>
        public struct Server
        {
            public struct FiveM
            {
                /// <summary>
                /// Event déclenché par FiveM lorsqu'un joueur s'est connecté et possède un NetID définitif pour la session
                /// </summary>
                public const string OnPlayerJoining = "playerJoining";

                public const string OnResourceStart = "onResourceStart";
            }

            public struct Bank
            {
                /// <summary>
                /// Event à déclencher pour créer les données nécessaires au script de banque
                /// </summary>
                public const string CreateEmptyPlayerMoney = "server::bank::CreateEmptyPlayerMoney";
            }
        }

        /// <summary>
        /// Events envoyés par le serveur vers le client
        /// </summary>
        public struct Stoc
        {
            public struct Bank
            {
                /// <summary>
                /// Envoi les 10 dernières transactions bancaires du joueur <br />
                /// Event invoqué en retour de <see cref="Ctos.Bank.GetLast10Transactions"/>
                /// </summary>
                public const string GetLast10Transactions = "stoc::bank::GetLast10Transactions";

                /// <summary>
                /// Envoi les informations bancaires de l'entreprise <br />
                /// Event invoqué en retour de <see cref="Ctos.Bank.GetCompanyCurrentActionValue"/>
                /// </summary>
                public const string GetCompanyCurrentActionValue = "stoc::bank::GetCompanyCurrentActionValue";

                /// <summary>
                /// Envoi les informations bancaires de l'entreprise <br />
                /// Event invoqué en retour de <see cref="Ctos.Bank.GetCompaniesCurrentActionValue"/>
                /// </summary>
                public const string GetCompaniesCurrentActionValue = "stoc::bank::GetCompaniesCurrentActionValue";

                /// <summary>
                /// Envoi les informations bancaires du joueur <br />
                /// Event invoqué en retour de <see cref="Ctos.Bank.GetPlayerMoney"/>
                /// </summary>
                public const string GetPlayerMoney = "stoc::bank::GetPlayerMoney";

                /// <summary>
                /// Envoi les informations bancaires du joueur ainsi que celles des entreprises
                /// Event invoqué en retour de <see cref="Ctos.Bank.GetPlayerMoneyAndCompanies"/>
                /// </summary>
                public const string GetPlayerMoneyAndCompanies = "stoc::bank:GetPlayerMoneyAndCompanies";
            }
        }

        /// <summary>
        /// Events envoyés par un client vers le serveur
        /// </summary>
        public struct Ctos
        {
            public struct Bank
            {
                /// <summary>
                /// Demande la restitution des 10 dernières transactions du joueur <br />
                /// Invoque en retour l'event <see cref="Stoc.Bank.GetLast10Transactions"/>
                /// </summary>
                public const string GetLast10Transactions = "ctos::bank::GetLast10Transactions";

                /// <summary>
                /// Demande la restitution des informations bancaires de l'entreprise <br />
                /// Invoque en retour l'event <see cref="Stoc.Bank.GetCompanyCurrentActionValue"/>
                /// </summary>
                public const string GetCompanyCurrentActionValue = "ctos::bank::GetCompanyCurrentActionValue";

                /// <summary>
                /// Demande la restitution des informations bancaires de toutes les entreprise <br />
                /// Invoque en retour l'event <see cref="Stoc.Bank.GetCompaniesCurrentActionValue"/>
                /// </summary>
                public const string GetCompaniesCurrentActionValue = "ctos::bank::GetCompaniesCurrentActionValue";

                /// <summary>
                /// Demande la restitution des informations bancaires du joueur <br />
                /// Invoque en retour l'event <see cref="Stoc.Bank.GetPlayerMoney"/>
                /// </summary>
                public const string GetPlayerMoney = "ctos::bank::GetPlayerMoney";

                /// <summary>
                /// Demande la restitution des informations bancaires du joueur ainsi que les informations bancaires des entreprise <br />
                /// Invoque en retour l'event <see cref="Stoc.Bank.GetPlayerMoneyAndCompanies"/>
                /// </summary>
                public const string GetPlayerMoneyAndCompanies = "ctos::bank::GetPlayerMoneyAndCompanies";

                /// <summary>
                /// Demande au serveur d'effectuer une transaction en cash (entre le joueur et son compte)
                /// Pas d'invocation en retour
                /// </summary>
                public const string DoCashTransaction = "ctos::bank::DoCashTransaction";

                /// <summary>
                /// Demande au serveur d'effectuer un virement vers un autre joueur
                /// Pas d'invocation en retour
                /// </summary>
                public const string DoPlayerTransfert = "ctos::bank::DoPlayerTransfert";

                /// <summary>
                /// Demande au serveur d'effectuer un virement entre les comptes du joueur
                /// Pas d'invocation en retour
                /// </summary>
                public const string DoTransfert = "ctos::bank::DoTransfert";

                /// <summary>
                /// Demande au serveur d'acheter une action
                /// Pas d'invocation en retour
                /// </summary>
                public const string BuyAction = "ctos::bank::BuyAction";

                /// <summary>
                /// Demande au serveur de vendre une action
                /// Pas d'invocation en retour
                /// </summary>
                public const string SellAction = "ctos::bank::SellAction";
            }
        }
    }
}
