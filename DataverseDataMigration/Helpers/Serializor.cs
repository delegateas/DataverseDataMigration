using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Export.Mappers;
using DataRestoration.Mapper;
using static DataRestoration.Mapper.ContactMapper;
using static DataRestoration.Mapper.AddressMapper;
using static DataRestoration.Mapper.AnnotationMapper;
using static DataRestoration.Mapper.EmailMapper;
using static DataRestoration.Mapper.IncidentMapper;
using static DataRestoration.Mapper.LeadMapper;
using static DataRestoration.Mapper.OpportunitiesMapper;
using static DataRestoration.Mapper.PhoneCallMapper;
using static DataRestoration.Mapper.QueueItemMapper;
using ConsoleJobs.DataRestoration.Emun;

namespace Export.Helpers
{
    class Serializor
    {
        private static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = false,
        };

        public static string SerializeEntityToJson(DTO entity)
        {
            return JsonSerializer.Serialize(SerializeInternalEntityToJson(entity));
        }

        private static DTOFile SerializeInternalEntityToJson(DTO entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Account entity cannot be null");
            }

            try
            {
                //Serialize children first to keep the type structure
                var serializedChildren = entity.Children.Select(child => SerializeInternalEntityToJson(child)).ToList();
                entity.Children = new List<DTO>();
                var content = "";

                switch (entity.Type)
                {
                    case EntityType.Account:
                        content = JsonSerializer.Serialize((AccountDTO)entity, options);
                        break;
                    case EntityType.Address:
                        content = JsonSerializer.Serialize((AddressDTO)entity, options);
                        break;
                    case EntityType.Annotation:
                        content = JsonSerializer.Serialize((AnnotationDTO)entity, options);
                        break;
                    case EntityType.Contact:
                        content = JsonSerializer.Serialize((ContactDTO)entity, options);
                        break;
                    case EntityType.Email:
                        content = JsonSerializer.Serialize((EmailDTO)entity, options);
                        break;
                    case EntityType.Incident:
                        content = JsonSerializer.Serialize((IncidentDTO)entity, options);
                        break;
                    case EntityType.Lead:
                        content = JsonSerializer.Serialize((LeadDTO)entity, options);
                        break;
                    case EntityType.Opportunity:
                        content = JsonSerializer.Serialize((OpportunityDTO)entity, options);
                        break;
                    case EntityType.PhoneCall:
                        content = JsonSerializer.Serialize((PhoneCallDTO)entity, options);
                        break;
                    case EntityType.QueueItem:
                        content = JsonSerializer.Serialize((QueueItemDTO)entity, options);
                        break;
                    default:
                        throw new NotSupportedException($"Entity type {entity.Type} is not supported for serialization");
                }
                return new DTOFile
                {
                    Name = entity.Id,
                    Content = content,
                    Type = entity.Type,
                    Children = serializedChildren,
                    Relations = JsonSerializer.Serialize(entity.Relations, options)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to serialize Account entity to JSON", ex);
            }
        }

        public static DTO DeserializeJsonToEntity(string json)
        {
            try
            {
                var dtoFile = JsonSerializer.Deserialize<DTOFile>(json);
                if (dtoFile == null)
                {
                    throw new InvalidOperationException("Deserialized DTOFile is null");
                }
                return DeserializeInternalEntityFromJson(dtoFile);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to deserialize JSON to DTO", ex);
            }
        }

        private static DTO DeserializeInternalEntityFromJson(DTOFile dtoFile)
        {
            if (dtoFile == null)
            {
                throw new ArgumentNullException(nameof(dtoFile), "DTOFile cannot be null");
            }

            var dot = new DTO();
            switch (dtoFile.Type)
            {
                case EntityType.Account:
                    dot = JsonSerializer.Deserialize<AccountDTO>(dtoFile.Content, options);
                    break;
                case EntityType.Address:
                    dot = JsonSerializer.Deserialize<AddressDTO>(dtoFile.Content, options);
                    break;
                case EntityType.Annotation:
                    dot = JsonSerializer.Deserialize<AnnotationDTO>(dtoFile.Content, options);
                    break;
                case EntityType.Contact:
                    dot = JsonSerializer.Deserialize<ContactDTO>(dtoFile.Content, options);
                    break;
                case EntityType.Email:
                    dot = JsonSerializer.Deserialize<EmailDTO>(dtoFile.Content, options);
                    break;
                case EntityType.Incident:
                    dot = JsonSerializer.Deserialize<IncidentDTO>(dtoFile.Content, options);
                    break;
                case EntityType.Lead:
                    dot = JsonSerializer.Deserialize<LeadDTO>(dtoFile.Content, options);
                    break;
                case EntityType.Opportunity:
                    dot = JsonSerializer.Deserialize<OpportunityDTO>(dtoFile.Content, options);
                    break;
                case EntityType.PhoneCall:
                    dot = JsonSerializer.Deserialize<PhoneCallDTO>(dtoFile.Content, options);
                    break;
                case EntityType.QueueItem:
                    dot = JsonSerializer.Deserialize<QueueItemDTO>(dtoFile.Content, options);
                    break;
                default:
                    throw new NotSupportedException($"Entity type {dtoFile.Type} is not supported for deserialization");
            }
            dot.Children = dtoFile.Children.Select(child => DeserializeInternalEntityFromJson(child)).ToList();
            dot.Relations = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(dtoFile.Relations, options);
            return dot;
        }

        private class DTOFile
        {
            public string Name { get; set; }
            public string Content { get; set; }
            public EntityType Type { get; set; }
            public List<DTOFile> Children { get; set; }
            public string Relations { get; set; }
        }

    }
}
