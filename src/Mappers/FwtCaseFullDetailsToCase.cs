using System;
using System.Collections.Generic;
using System.Linq;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Mappers
{
    public static class FwtCaseFullDetailsToCase
    {
        public static Case MapToCase(this FWTCaseFullDetails fwtCaseFullDetails)
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
                    if (eForm != null && eForm.EformData != null)
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
