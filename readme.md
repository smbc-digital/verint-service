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


## Verint online forms & Confirm Integration

The verint-service can be used to create cases with attached verint online forms. Primarily this is to power an integration between verint and the Confirm place management system.



## Attaching documents and notes to cases

Documents can now be attached to verint cases as part of case creation by adding them to the `Case.NotesWithAttachments` list. If the `Case.UploadNotesWithAttachmentsAfterCaseCreation` flag is set to `true` the uploaded notes will be cached until they are requested to be uploaded using the [Upload Cached Notes Webhook](#Upload-Cached-Notes-Webhook). 

Caching note/uploads is useful when other back office automated processes need to complete before the files can be attached; for example: the Confirm integration needs to fire before uploads are added if the files are to be subsequently passed to Confirm.

Notes can also be attached on request at any time using the `api/v1/case/add-note-with-attachments` endpoint.

### Upload Cached Notes Webhook

The upload cached notes webhook will retrieve notes from the cache and then attach them to the requested case. The webhooks endpoint can be found at the address below.

```/api/v1/webhooks/upload-cached-notes/{id}```

**Note**: This used the same mechanism as `add-note-with-attachments`, The `NoteWithAttachments` object must have the correct `CaseRef` value set for the case. If `UploadNotesWithAttachmentsAfterCaseCreation` is set to true this should be done for you automatically after the base case has been created.

## License
[MIT](https://tldrlegal.com/license/mit-license)
