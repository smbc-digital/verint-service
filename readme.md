<h1 align="center">Verint Service</h1>

<div align="center">
  :open_file_folder::pencil2::books:
</div>
<div align="center">
Service to interact with our Case Management System
</div>

<br />

<div align="center">
  <img alt="Application version" src="https://img.shields.io/badge/version-1.0.0-brightgreen.svg?style=flat-square" />
  <img alt="Open Issues" src="https://img.shields.io/github/issues/smbc-digital/verint-service">
    <img alt="Stars" src="https://img.shields.io/github/stars/smbc-digital/verint-service">
  <img alt="Stability for application" src="https://img.shields.io/badge/stability-stable-brightgreen.svg?style=flat-square" />
</div>

<div align="center">
  <h3>
    External Links
    <a href="https://github.com/smbc-digital">
      GitHub
    </a>
    <span> | </span>
    <a href="https://www.nuget.org/profiles/Stockport-Council">
      NuGet
    </a>
  </h3>
</div>

<div align="center">
  <sub>Built with ❤︎ by
  <a href="https://www.stockport.gov.uk">Stockport Council</a> and
  <a href="">
    contributors
  </a>
</div>


## Table of Contents
- [Requirments](#requirments)
- [Installation](#installation)
- [Server Integration](#server_integration) 
- [Confirm/Verint Online Form integrations](#Confirm-and-Verint-Online-Form-integrations)
- [A note about matching individuals](#a-note-about-matching-individuals)

## Requirments
- dotnet core 3.1
- editor of your choice
- gpg key added to accepted contributors


## Installation
```console
$ git clone git@github.com:smbc-digital/verint-service.git
$ cd verint-service/src
$ git-crypt unlock
$ dotnet run
```

## Server_Integration

[For server integration documentation please go to our internal git](https://git.stockport.gov.uk/devs/dts-documentation/wikis/Verint-Service-Integration)

## A note about matching individuals

When cases are created if user information is passed to the verint-service a matching algorithm is used to match users to individuals already stored within verint. Information about the expected behaviour of this user matching can be found in [Sharepoint](https://stockportcouncil.sharepoint.com/:w:/r/sites/col/dbd/_layouts/15/doc2.aspx?sourcedoc=%7B42D5148B-1BB4-4C1A-BCEE-F4C490C39FC8%7D&file=Verint%20user%20matching%20scoring%20.docx&action=default&mobileredirect=true&cid=c521fe92-43fa-4708-b88d-6b3e856f33a6)

## Verint online forms & Confirm integration

The verint-service can be used to create cases with attached verint online forms. Primarily this is to power an integration between verint and the Confirm place management system.

Previously integrations such as this relied on the older verint e-forms technology, the data required to enable this was stored or at least was expected to be provided as part of the `Case` object. This is no longer the case.

To enable "Verint Online Forms" integration we have a new object `VerintOnlineFormRequest`, this is part of the `StockportGovUk.NetStandard.Models` nuget package, it wraps the pre-existing `Case` object and adds `FormName`, a string specifiying which VerintOnlineForm to use (this is pretty much always `confirm_universalform`), and `FormData` a key/value dictionary which maps the VerintOnlineForm fields and their values for the standard integration.

### Extension methods

Extension methods help correctly create `VerintOnlineForRequest`, these can be found in the `StockportGovUK.NetStandard.Extensions` nuget package.

If a case has no attributes (extra pieces of information required by confirm) you should be able to use the `Case.ToConfirmIntegrationFormCase` extension method in order to create your Verint Online Form request. This will take all the standard information from the `Case` map it and configuration date to the correct `FormData` fields.

For reference this looks like:

```
            var formData = new Dictionary<string, string>
                {
                    {"CONF_SERVICE_CODE", configuration.ServiceCode},
                    {"CONF_SUBJECT_CODE", configuration.SubjectCode},
                    {"FOLLOW_UP_BY", configuration.FollowUp},
                    {"CboClassCode", configuration.ClassCode},
                    {"le_eventcode", configuration.EventId.ToString()},
                    {"le_queue_complete", "AppsConfirmQueuePending"},
                    {"CONF_CASE_ID", crmCase.CaseReference}
                    ...
```

and is used per the example below (configuration can be stored elsewhere):

```
            var confirmIntegrationFormOptions = new ConfirmIntegrationFormOptions
            {
                ServiceCode = "HWAY",
                SubjectCode = "CWGU",
                ClassCode = "SERV",
                EventId = crmCase.EventCode,
            };

            // Turn case into a VOF request
            VerintOnlineFormRequest verintOnlineFormRequest = crmCase.ToConfirmIntegrationFormCase(confirmIntegrationFormOptions);
```

### Forms that require attributes

Forms that require atrributes request extra information to be provided in `FormData` in isolated cases this could be acheived very simply as follows 

```
            var verintOnlineFormRequest = crmCase.ToConfirmIntegrationFormCase(confirmIntegrationFormOptions);
            verintOnlineFormRequest.FormData.Add("CONF_ATTRIBUTE_XXXX", "ABCDE");
            verintOnlineFormRequest.FormData.Add("CONF_ATTRIBUTE_YYYY", "ABCDE");
```

However it may be preffereable to create a new extension method for that which themselves call the base behaviour and then add the attributes on, for example

```
        public static VerintOnlineFormRequest ToConfirmExampleCustomIntegrationFormCase(this Case crmCase, ConfirmCustomIntegrationFormOptions configuration)
        {
            var baseCase = crmCase.ToConfirmIntegrationFormCase(configuration);
            baseCase.FormData.Add("CONF_ATTRIBUTE_XXXX", configuration.ValueOfXXXX);
            baseCase.FormData.Add("CONF_ATTRIBUTE_YYYY", configuration.ValueOfYYYY;
            return baseCase;
        }
 ```
 
 And call that when create our `VerintOnlineFormRequest` instead:
 
 ```
 var verintOnlineFormRequest = crmCase.ToConfirmExampleCustomIntegrationFormCase(confirmIntegrationFormOptions);
 ```
 
 ### Making the VOF request
 
In order to create the Verint Online Form and attach to a case we can use the `VerintServiceGateway` which is in the `StockportGovUk.NetStandard.Gateways` package. This will orchestrate the process of setting up a verint case, attaching the Verint Online Form and ensuring the required data is added.

```
            var result = await _verintServiceGateway.CreateVerintOnlineFormCase(verintOnlineFormRequest);
```

If we want to post directly to the service you can do this by making a POST request to the service endpoint, where the body is the request serialized to a json string

```https://my-service-url:port/api/v1/VerintOnlineForm```

## Attaching documents and notes to cases

Documents can now be attached to verint cases as part of case creation by adding them to the `Case.NotesWithAttachments` list. If the `Case.UploadNotesWithAttachmentsAfterCaseCreation` flag is set to `true` the uploaded notes will be cached until they are requested to be uploaded using the [Upload Cached Notes Webhook](#Upload-Cached-Notes-Webhook). 

Caching note/uploads is useful when other back office automated processes need to complete before the files can be attached; for example: the Confirm integration needs to fire before uploads are added if the files are to be subsequently passed to Confirm.

Notes can also be attached on request at any time using the `api/v1/case/add-note-with-attachments` endpoint.

NOTE: For confirm file uploads via the webhook - each uploaded document need be added to a seperate note. i.e. 1 note per document.

### Upload cached notes webhook

The upload cached notes webhook will retrieve notes from the cache and then attach them to the requested case. The webhooks endpoint can be found at the address below.

```/api/v1/webhooks/upload-cached-notes/{id}```

**Note**: This uses the same mechanism as `add-note-with-attachments`, The `NoteWithAttachments` object must have the correct `CaseRef` value set for the case. If `UploadNotesWithAttachmentsAfterCaseCreation` is set to true this should be done for you automatically after the base case has been created.

## License
[MIT](https://tldrlegal.com/license/mit-license)
