using System;
using ConsoleJobs.DataRestoration.Emun;
using Export.Mappers;
using Helpers;

namespace Import.Creator
{
    public abstract class BaseCreator
    {
        abstract public void Create(DTO entity,EntityType ParentType = EntityType.None, string ParentId = "");
        public void HandleChildren(DTO entity, MyXrm context)
        {
            foreach (var child in entity.Children)
            {
                switch (child.Type)
                {
                    case EntityType.Account:
                        var accountCreater = new AccountCreator(context);
                        accountCreater.Create(child, entity.Type, entity.Id);
                        break;
                    case EntityType.Address:
                        var addressCreater = new AddressCreator(context);
                        addressCreater.Create(child, entity.Type, entity.Id);
                        break;
                    case EntityType.Annotation:
                        var annotationCreater = new AnnotationCreator(context);
                        annotationCreater.Create(child, entity.Type, entity.Id);
                        break;
                    case EntityType.Contact:
                        var contactCreater = new ContactCreator(context);
                        contactCreater.Create(child, entity.Type, entity.Id);
                        break;
                    case EntityType.Email:
                        var emailCreater = new EmailCreator(context);
                        emailCreater.Create(child, entity.Type, entity.Id);
                        break;
                    case EntityType.Incident:
                        var incidentCreater = new IncidentCreator(context);
                        incidentCreater.Create(child, entity.Type, entity.Id);
                        break;
                    case EntityType.Lead:
                        var leadCreater = new LeadCreator(context);
                        leadCreater.Create(child, entity.Type, entity.Id);
                        break;
                    case EntityType.Opportunity:
                        var opportunityCreater = new OpportunityCreator(context);
                        opportunityCreater.Create(child, entity.Type, entity.Id);
                        break;
                    case EntityType.PhoneCall:
                        var phoneCallCreater = new PhoneCallCreator(context);
                        phoneCallCreater.Create(child, entity.Type, entity.Id);
                        break;
                    case EntityType.QueueItem:
                        var queueItemCreater = new QueueItemCreator(context);
                        queueItemCreater.Create(child, entity.Type, entity.Id);
                        break;
                    default:
                        throw new NotImplementedException($"Creator for type {child.Type} is not implemented yet.");
                }
            }
        }
    }

}
