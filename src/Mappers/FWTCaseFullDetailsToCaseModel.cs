using System;
using System.Collections.Generic;
using System.Linq;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Mappers
{
    public static class FWTCaseFullDetailsToCaseMapper
    {
        public static Case MapFrom(FWTCaseFullDetails fwtCaseFullDetails)
        {
            var mappedCase = new Case
            {
                CaseReference = fwtCaseFullDetails.CoreDetails.CaseReference,
                EnquirySubject = fwtCaseFullDetails.CoreDetails.Classification[0],
                EnquiryReason = fwtCaseFullDetails.CoreDetails.Classification[1],
                EnquiryType = fwtCaseFullDetails.CoreDetails.Classification[2],
                CaseTitle = fwtCaseFullDetails.CoreDetails.Title,
                EventTitle = fwtCaseFullDetails.Events.First().EventTitle,
                EventDate = fwtCaseFullDetails.Events.First().Created,
                Description = fwtCaseFullDetails.CoreDetails.Description,
                Status = fwtCaseFullDetails.CoreDetails.Status,

            };

            for (int index = 0; index < fwtCaseFullDetails.CoreDetails.Classification.Length; index++)
            {
                var level = fwtCaseFullDetails.CoreDetails.Classification[index];

                mappedCase.Classification += index < fwtCaseFullDetails.CoreDetails.Classification.Length - 1 ? $"{level} > " : level;
            }

            if (fwtCaseFullDetails.CoreDetails.AssociatedObject != null)
            {
                switch (fwtCaseFullDetails.CoreDetails.AssociatedObject.ObjectID.ObjectType)
                {
                    case "D3":
                        mappedCase.Property = new Address
                        {
                            Description = fwtCaseFullDetails.CoreDetails.AssociatedObject.ObjectDescription
                        };
                        break;

                    case "D4":
                        mappedCase.Street = new Street
                        {
                            Description = fwtCaseFullDetails.CoreDetails.AssociatedObject.ObjectDescription
                        };
                        break;
                }
            }

            if (fwtCaseFullDetails.Form != null)
            {
                mappedCase.FormName = fwtCaseFullDetails.Form.FormName;
                foreach (var field in fwtCaseFullDetails.Form.FormField)
                {
                    if (!string.IsNullOrWhiteSpace(field.Label) && !string.IsNullOrWhiteSpace(field.Value))
                        mappedCase.CaseFormFields.Add(new CustomField(field.Key, field.Label, field.Value));
                }
            }

            if (fwtCaseFullDetails.EformData != null && fwtCaseFullDetails.EformData.Length > 0)
            {
                foreach (var eForm in fwtCaseFullDetails.EformData)
                {
                    if (eForm != null)
                    {
                        foreach (var dataItem in eForm.EformData)
                        {
                            if (!string.IsNullOrWhiteSpace(dataItem.FieldName) && !string.IsNullOrWhiteSpace(dataItem.FieldValue))
                                mappedCase.IntegrationFormFields.Add(new CustomField(dataItem.FieldName, dataItem.FieldValue));
                        }
                    }
                }
            }

            if (fwtCaseFullDetails.Eforms != null && fwtCaseFullDetails.Eforms.Length > 0)
            {
                var dateCreated = DateTime.MinValue;
                foreach (var eForm in fwtCaseFullDetails.Eforms)
                {
                    if (eForm.Created.CompareTo(dateCreated) > 0)
                    {
                        dateCreated = eForm.Created;
                        mappedCase.DefinitionName = eForm.Name;
                    }
                }
            }


            if (fwtCaseFullDetails.Notes != null)
            {
                foreach (var note in fwtCaseFullDetails.Notes)
                {
                    if (mappedCase.Notes == null)
                    {
                        mappedCase.Notes = new List<Note>();
                    }

                    mappedCase.Notes.Add(new Note(note.NoteID, note.Text, note.Created, note.CreatedBy.UserName));
                }
            }

            //if (fwtCaseFullDetails.CoreDetails.AssociatedObject != null)
            //{
            //    if (fwtCaseFullDetails.CoreDetails.AssociatedObject.ObjectID.ObjectType == TagHelperMetadata.Common.OrganisationObjectType)
            //    {
            //        var org = Lagan.retrieveOrganisation(details.CoreDetails.AssociatedObject.ObjectID);

            //        if (org != null)
            //        {
            //            mappedCase.Organisation = new Organisation();

            //            if (org.SocialContacts != null && org.SocialContacts.Length > 0)
            //            {
            //                mappedCase.Organisation.SocialContacts = new SocialContact[org.SocialContacts.Length];
            //                for (int i = 0; i < org.SocialContacts.Length; i++)
            //                {
            //                    mappedCase.Organisation.SocialContacts[i] = new SocialContact
            //                    {
            //                        Value = org.SocialContacts[i].SocialID,
            //                        Type = org.SocialContacts[i].SocialChannel
            //                    };
            //                }
            //            }

            //            if (org.Name != null && org.Name[0].FullName != null)
            //            {
            //                mappedCase.Organisation.Name = org.Name[0].FullName;
            //            }

            //            if (org.ContactEmails != null && org.ContactEmails.Length > 0)
            //                mappedCase.Organisation.Email = org.ContactEmails[0].EmailAddress;
            //        }
            //    }

            //    if (!string.IsNullOrWhiteSpace(details.Interactions[0].PartyID.ObjectReference[0]) && details.Interactions[0].PartyID.ObjectType == "C1")
            //    {
            //        var individual = Lagan.retrieveIndividual(details.Interactions[0].PartyID);

            //        if (individual != null)
            //        {

            //            mappedCase.Customer = new Customer
            //            {
            //                Forename = individual.Name[0].Forename[0],
            //                Surname = individual.Name[0].Surname
            //            };

            //            if (individual.ContactPostals != null && individual.ContactPostals.Length > 0)
            //            {
            //                var address = individual.ContactPostals.Any(_ => _.Preferred)
            //                    ? individual.ContactPostals.FirstOrDefault(_ => _.Preferred)
            //                    : individual.ContactPostals[0];

            //                mappedCase.Customer.Address = new Address
            //                {
            //                    UPRN = address.UPRN,
            //                    AddressLine1 = address.AddressLine[0],
            //                    AddressLine2 = address.AddressLine[1],
            //                    AddressLine3 = address.AddressLine[2],
            //                    City = address.City,
            //                    Number = address.AddressNumber,
            //                    Postcode = address.Postcode,
            //                    PropertyId = address.PropertyID,
            //                };
            //            }

            //            if (individual.SocialContacts != null && individual.SocialContacts.Length > 0)
            //            {
            //                mappedCase.Customer.SocialContacts = new SocialContact[individual.SocialContacts.Length];
            //                for (int i = 0; i < individual.SocialContacts.Length; i++)
            //                {
            //                    mappedCase.Customer.SocialContacts[i] = new SocialContact
            //                    {
            //                        Value = individual.SocialContacts[i].SocialID,
            //                        Type = individual.SocialContacts[i].SocialChannel
            //                    };
            //                }
            //            }


            //            if (individual.ContactEmails != null && individual.ContactEmails.Length > 0)
            //            {
            //                mappedCase.Customer.Email = individual.ContactEmails.Any(_ => _.Preferred)
            //                    ? individual.ContactEmails.FirstOrDefault(_ => _.Preferred).EmailAddress
            //                    : individual.ContactEmails[0].EmailAddress;
            //            }

            //        }
            //    }
            //}

            if (fwtCaseFullDetails.LinkCases != null)
            {
                foreach (var linkCase in fwtCaseFullDetails.LinkCases)
                {
                    if (mappedCase.LinkCases == null)
                    {
                        mappedCase.LinkCases = new List<string>();
                    }

                    mappedCase.LinkCases.Add(linkCase.LinkedCase);
                }
            }

            return mappedCase;
        }
    }
}
