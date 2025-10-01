using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace ConsoleJobs
{
    class Connector
    {
        public static IOrganizationService GetCRMDevService()
        {
            //Create the Dynamics 365 Connection:        
            var crmConnectionStringDEV = "";

            CrmServiceClient.MaxConnectionTimeout = new TimeSpan(24, 0, 0);
            CrmServiceClient client = new CrmServiceClient(crmConnectionStringDEV);

            if (client.IsReady)
            {
                return client.OrganizationWebProxyClient;//?? client.OrganizationServiceProxy; ;
            }
            else
            {
                Console.WriteLine(client.LastCrmError);
                throw new AccessViolationException("Could not connect to CRM");
            }
        }

        public static IOrganizationService GetCRMProdService()
        {
            //Create the Dynamics 365 Connection:   
            var crmConnectionStringProd = "Url=";

            CrmServiceClient.MaxConnectionTimeout = new TimeSpan(24, 0, 0);
            CrmServiceClient client = new CrmServiceClient(crmConnectionStringProd);

            if (client.IsReady)
            {
                return client.OrganizationWebProxyClient;//?? client.OrganizationServiceProxy; ;
            }
            else
            {
                Console.WriteLine(client.LastCrmError);
                throw new AccessViolationException("Could not connect to CRM");
            }
        }
        public static IOrganizationService GetCRMBackupProdService()
        {
            //Create the Dynamics 365 Connection:           
            var crmConnectionStringProdBackup = "";

            CrmServiceClient.MaxConnectionTimeout = new TimeSpan(24, 0, 0);
            CrmServiceClient client = new CrmServiceClient(crmConnectionStringProdBackup);

            if (client.IsReady)
            {
                return client.OrganizationWebProxyClient;//?? client.OrganizationServiceProxy; ;
            }
            else
            {
                Console.WriteLine(client.LastCrmError);
                throw new AccessViolationException("Could not connect to CRM");
            }
        }
        public static IOrganizationService GetCRMTestService()
        {
            var crmConnectionStringTest = "";

            CrmServiceClient.MaxConnectionTimeout = new TimeSpan(24, 0, 0);
            CrmServiceClient client = new CrmServiceClient(crmConnectionStringTest);

            if (client.IsReady)
            {
                return client.OrganizationWebProxyClient;//?? client.OrganizationServiceProxy; ;
            }
            else
            {
                Console.WriteLine(client.LastCrmError);
                throw new AccessViolationException("Could not connect to CRM");
            }
        }

        public static IOrganizationService GetCRMUATService()
        {
            var crmConnectionStringUAT = "";

            CrmServiceClient.MaxConnectionTimeout = new TimeSpan(24, 0, 0);
            CrmServiceClient client = new CrmServiceClient(crmConnectionStringUAT);

            if (client.IsReady)
            {
                return client.OrganizationWebProxyClient;//?? client.OrganizationServiceProxy; ;
            }
            else
            {
                Console.WriteLine(client.LastCrmError);
                throw new AccessViolationException("Could not connect to CRM");
            }
        }
    }
}
