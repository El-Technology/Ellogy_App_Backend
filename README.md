# Back-end Documentation  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white">   <img src="https://img.shields.io/badge/microsoft%20azure-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white">

<details>
	<summary>Content table</summary>
	
## [1. Azure Architecture](#azure-architecture)
   - [1.1 API Gateway](#api-gateway)
   - [1.2 Virtual Machine](#virtual-machine)
   - [1.3.1 AI Communication Service](#ai-communication-service)
   - [1.3.2 User Manager Service](#user-manager-service)
   - [1.3.3 Tickets Manager Service](#tickets-manager-service)
   - [1.3.4 Payment Manager Service](#payment-manager-service)
   - [1.4 Database](#database)
   - [1.5 Blob Storage](#blob-storage)
   - [1.6 Message Bus](#message-bus)
   - [1.7 Email Communication](#email-communication)
   - [1.8 Azure Function](#azure-function)

## [2. GitHub Structure](#github-structure)
   - [2.1 Version Control](#version-control)
   - [2.2 Git Actions](#git-actions)
   - [2.3 Docker Containerization](#docker-containerization)
   - [2.4 Deployment to Azure](#deployment-to-azure)
   - [2.5 Git Secrets Importance](#git-secrets-importance)
   - [2.6 Summary about Git](#summary-about-git)
   - [2.7 Unit Tests](#unit-tests)

## [3. Additional Information](#additional-information)
   - [3.1 Azure Templates](#azure-templates)
   - [3.2 Obtaining a Template](#obtaining-a-template)
   - [3.3 How to Use the Template](#how-to-use-the-template)
   - [3.4 OpenAI Functions](#openai-functions)

</details>

---

## <a name="azure-architecture"></a>1. Azure Architecture
<img src="https://github.com/El-Technology/Ellogy_App_Backend/blob/pre-release/images/Azure%20architecture.vpd.png">
The entire backend of the application is hosted on Azure. Here's a breakdown of key services:

### <a name="api-gateway"></a><ins>1.1 API Gateway</ins>



API Gateway, hosted on Azure and utilizing "Ocelot," simplifies communication with microservices. It handles routing, aggregation, load balancing, authentication, caching, and more.
Also, the gateway has its own domain `backend.ellogy.ai`, which means that all incoming requests are sent to the domain and automatically forwarded to the necessary services and methods,
for example, instead of
```
http://20.21.124.185:5281/api/auth/login
``` 
you only need to send a request to
```
backend.ellogy.ai/gateway/auth/login
```

which greatly simplifies the work of front-end developers when they do not need to know on which port a particular service is running.

### <a name="virtual-machine"></a><ins>1.2 Virtual Machine</ins>

The Virtual Machine runs multiple services independently. It uses Ubuntu 22.04.2 LTS and allows access via Git Bash. Services are accessed on different ports and documented via Swagger.

<img src="https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white">

All five services are running on different ports:
1) `5281` - UserManager
2) `5041` - TicketsManager
3) `53053` – AICommunication
4) `8080` – PlantUML
5) `5021` - PaymentManager
   
The same virtual machine IP address is used for all of them.

Prod mahine IP

```
http://20.21.124.185:5281/swagger/index.html
```

Dev mahine IP

```
http://20.21.129.55:5281/swagger/index.html
```

Just for PlantUML we dont need to use `/swagger/index.html`


### <a name="ai-communication-service"></a><ins>1.3.1 AI Communication Service</ins>
<img src="https://img.shields.io/badge/ChatGPT-74aa9c?style=for-the-badge&logo=openai&logoColor=white">
AICommunication service is a software component that allows applications to interact with large language models (LLMs) and exchange information. It acts as the central communication hub, handling all the logic behind interacting with LLMs and using the retrieved information.

Here's a breakdown of its functionalities:

- Communication with LLMs: AICommunication service can send queries and receive responses from LLMs, facilitating the exchange of information.
- Document processing: The service can potentially process and extract information from its own documents, further enriching its knowledge base and enhancing its capabilities.

In essence, AICommunication service acts as a bridge between applications and LLMs, enabling them to communicate and leverage the power of LLMs for various purposes.

<details>
   <summary><code>Endpoints</code></summary>

   
Communication
------------------------------------------------------------------------------------------

<details>
 <summary><code>[POST]</code> <code><b>...</b></code> <code>/api/Communication/getSignalRStreamResponse</code></summary>
   
#### Endpoint for retrieving AI response as streaming using SignalR.
##### Parameters (body)

| Attribute                | Type     | Required | Description           |
|--------------------------|----------|----------|-----------------------|
| `aiModelEnum`            | int      | Yes      | An integer representing the type of AI model being used. |
| `templateName`           | string    | Yes      | The name of the template being used. |
| `userInput`              | string    | Yes      | The user input that will be processed by the AI model. |
| `temperature`            | int      | No       | An optional integer that can be used to control the temperature of the AI model. |
| `useRAG`                 | bool     | Yes      | A boolean value indicating whether to use RAG (Retrieval Augmented Generation) capabilities. |
| `fileName`               | string    | No       | An optional string representing the name of the file to be processed. |
| `connectionId`           | string    | No       | An optional string that can be used to identify the connection. |
| `signalMethodName`       | string    | No       | An optional string representing the name of the signal method to be used. |


##### Responses

| http code     | content-type                      | response                                                            |
|---------------|-----------------------------------|---------------------------------------------------------------------|
| `200`         | `application/json`        | `Success`                                                           |


</details>

<details>
 <summary><code>[POST]</code> <code><b>...</b></code> <code>/api/Communication/getChatResponse</code></summary>
   
#### Endpoint for retrieving AI response as string.
##### Parameters (body)

| Attribute                | Type     | Required | Description           |
|--------------------------|----------|----------|-----------------------|
| `aiModelEnum`            | int      | Yes      | An integer representing the type of AI model being used. |
| `templateName`           | string    | Yes      | The name of the template being used. |
| `userInput`              | string    | Yes      | The user input that will be processed by the AI model. |
| `temperature`            | int      | No       | An optional integer that can be used to control the temperature of the AI model. |
| `useRAG`                 | bool     | Yes      | A boolean value indicating whether to use RAG (Retrieval Augmented Generation) capabilities. |
| `fileName`               | string    | No       | An optional string representing the name of the file to be processed. |


##### Responses

| http code     | content-type                      | response                                                            |
|---------------|-----------------------------------|---------------------------------------------------------------------|
| `200`         | `application/json`        | `Success`                                                           |


</details>

<details>
 <summary><code>[POST]</code> <code><b>...</b></code> <code>/api/Communication/chatWithFunctions</code></summary>

#### Endpoint for retrieving AI response by Json Example.
##### Parameters (body)

| Attribute                | Type     | Required | Description           |
|--------------------------|----------|----------|-----------------------|
| `aiModelEnum`            | int      | Yes      | An integer representing the type of AI model being used. |
| `templateName`           | string    | Yes      | The name of the template being used. |
| `userInput`              | string    | Yes      | The user input that will be processed by the AI model. |
| `temperature`            | int      | No       | An optional integer that can be used to control the temperature of the AI model. |
| `useRAG`                 | bool     | Yes      | A boolean value indicating whether to use RAG (Retrieval Augmented Generation) capabilities. |
| `fileName`               | string    | No       | An optional string representing the name of the file to be processed. |


##### Responses

| http code     | content-type                      | response                                                            |
|---------------|-----------------------------------|---------------------------------------------------------------------|
| `200`         | `application/json`        | `Success`                                                           |


</details>


Prompt
------------------------------------------------------------------------------------------
<details>
 <summary><code>[GET]</code> <code><b>...</b></code> <code>/api/Prompt/getAllPrompts</code></summary>
   
#### Endpoint for retrieving AI response as streaming using SignalR.
##### Parameters empty


##### Responses

| http code     | content-type                      | response                                                            |
|---------------|-----------------------------------|---------------------------------------------------------------------|
| `200`         | `application/json`        | `Success`                                                           |


</details>

<details>
 <summary><code>[POST]</code> <code><b>...</b></code> <code>/api/Prompt/createPrompt</code></summary>
   
#### Endpoint for retrieving AI response by Json Example.
##### Parameters (body)

| Attribute                | Type     | Required | Description           |
|--------------------------|----------|----------|-----------------------|
| `templateName`            | string    | Yes      | The name of the template being used to generate the response. |
| `functions`               | string    | No       | A string representing the functions used in the template. |
| `description`             | string    | No       | A string describing the purpose of the model. |
| `input`                   | string    | Yes      | The user's input string. |
| `value`                   | string    | Yes      | The generated value by the model. |


##### Responses

| http code     | content-type                      | response                                                            |
|---------------|-----------------------------------|---------------------------------------------------------------------|
| `200`         | `application/json`        | `Success`                                                           |


</details>

<details>
 <summary><code>[PUT]</code> <code><b>...</b></code> <code>/api/Prompt/updatePrompt</code></summary>
   
#### Updates an existing prompt with the provided data.

##### Parameters (query) 
| Attribute                | Type     | Required | Description           |
|--------------------------|----------|----------|-----------------------|
| `templateName`            | string    | Yes      | The name of the template being used to generate the response. |

##### Parameters (body)

| Attribute                | Type     | Required | Description           |
|--------------------------|----------|----------|-----------------------|
| `functions`               | string    | No       | A string representing the functions used in the template. |
| `description`             | string    | No       | A string describing the purpose of the model. |
| `input`                   | string    | Yes      | The user's input string. |
| `value`                   | string    | Yes      | The generated value by the model. |


##### Responses

| http code     | content-type                      | response                                                            |
|---------------|-----------------------------------|---------------------------------------------------------------------|
| `200`         | `application/json`        | `Success`                                                           |


</details>

<details>
 <summary><code>[DELETE]</code> <code><b>...</b></code> <code>/api/Prompt/deletePrompt</code></summary>
   
#### Updates an existing prompt with the provided data.

##### Parameters (query) 
| Attribute                | Type     | Required | Description           |
|--------------------------|----------|----------|-----------------------|
| `promptName`            | string    | Yes      | The name of the template being used to generate the response. |

##### Responses

| http code     | content-type                      | response                                                            |
|---------------|-----------------------------------|---------------------------------------------------------------------|
| `200`         | `application/json`        | `Success`                                                           |

</details>


RAG
------------------------------------------------------------------------------------------
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code>/api/RAG/uploadDocumentUrl</code></summary>

  #### Endpoint for retrieving document upload URL.

  This method returns the URL for uploading a document.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | fileName           | string   | Yes      | The name of the file to be uploaded.       |

  ##### Responses

  | http code  | content-type                               | Description                                           |
  |-----------|-----------------------------------------------|-------------------------------------------------------|
  | `200`      | `application/json`                           | Success. The response body contains the URL for uploading the document. |

</details>

<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/RAG/getDocumentUrl</b></code></summary>

  #### Endpoint for retrieving document download URL.

  This method returns the url for downloading the document.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | fileName           | string   | Yes      | The name of the file to download.        |
  | ownerld            | string   | Yes      | The owner ID of the document.             |

  ##### Responses

  | http code  | content-type                               | Description                                           |
  |-----------|-----------------------------------------------|-------------------------------------------------------|
  | `200`      | `application/json`                           | Success. The response body contains the URL for downloading the document. |

</details>

<details>
  <summary><code>[DELETE]</code> <code><b>...</b></code> <code><b>/api/RAG/deleteDocument</b></code></summary>

  #### Endpoint for deleting a document.

  This method deletes the document from the storage.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | fileName           | string   | Yes      | The name of the file to delete.           |

  ##### Responses

  | http code | content-type | Description               |
  |-----------|--------------|----------------------------|
  | `200`     | `text/plain`  | Success                   |

</details>

<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/RAG/getUserDocuments</b></code></summary>

  #### Endpoint for retrieving a list of the user's documents.

  This method returns a list of the user's documents based on pagination parameters.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | currentPageNumber  | integer   | Yes      | The current page number of the results.     |
  | recordsPerPage     | integer   | Yes      | The number of records per page to return. |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. The response body contains an array of objects, each 


</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/RAG/verifyDocumentUpload</b></code></summary>

  This method verifies if the document was uploaded

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | fileName           | string   | Yes      | The name of the file to verify.           |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. 


</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/RAG/embedDocument</b></code></summary>

  This method embeds the document in the vector database

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | fileName           | string   | Yes      | The name of the file to embed.           |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. 


</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/RAG/findUser</b></code></summary>

  This method finds the user by email prefix

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | emailPrefix           | string   | Yes      | The prefix of an email           |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. 


</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/RAG/givePermission</b></code></summary>

  This method gives permission for using the document

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | receiverId  | guid   | Yes      | The id of user.     |
  | documentName     | string   | Yes      | The name of file. |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. 


</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/RAG/removePermission</b></code></summary>

  This method removes
permission for using the document

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | receiverId  | guid   | Yes      | The id of user.     |
  | documentName     | string   | Yes      | The name of file. |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. 


</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/RAG/getAllUsersWithPermission</b></code></summary>

  This method removes
permission for using the document

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | fileName  | string   | Yes      | The name of file.     |

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | currentPageNumber  | integer   | Yes      | The current page number of the results.     |
  | recordsPerPage     | integer   | Yes      | The number of records per page to return. |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. 


</details>
</details>

### <a name="user-manager-service"></a><ins>1.3.2 User Manager Service</ins>

This is a general term used for a service responsible for managing user accounts within a system. Its functionalities typically include:

- User creation and deletion: Adding and removing users from the system.
- User authentication: Verifying user credentials (e.g., username and password) during login attempts.
- User information management: Maintaining user profiles, including details like name, email, preferences, etc.
- Authorization: Controlling access to different functionalities or resources within the system based on user roles and permissions.

<details>
	 <summary><code>Endpoints</code></summary>


Auth
------------------------------------------------------------------------------------------

<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Auth/register</b></code></summary>

  Registers a new user. Returns HTTP 404 if user already exist.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | firstName  | string   | Yes      |    |
  | lastName  | string   | Yes      |    |
  | email  | string   | Yes      |    |
  | phoneNumber  | string   | No      |    |
  | password  | string   | Yes      |    |
  | organization  | string   | No      |    |
  | department  | string   | No      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
  | `404`     | `application/json` | Not Found. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Auth/login</b></code></summary>

  Logs in a user. Returns HTTP 404 if user does not exist.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | email  | string   | Yes      |    |
  | password  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
  | `404`     | `application/json` | Not Found. |
  | `500`     | `application/json` | Server Error. |
  | `400`     | `application/json` | Bad Request. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Auth/activateUserAccount</b></code></summary>

  Activates user account.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | token  | string   | Yes      |    |
  | userEmail  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
  | `500`     | `application/json` | Server Error. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Auth/sentVerificationEmail</b></code></summary>

  Sends a verification email to the user.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | redirectLink  | string   | Yes      |    |
  | userEmail  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
  | `500`     | `application/json` | Server Error. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Auth/refreshJwtToken</b></code></summary>

  Refresh jwt token if expires time has ended.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | jwt  | string   | Yes      |    |
  | refreshToken  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
  | `500`     | `application/json` | Server Error. |

</details>


Password
------------------------------------------------------------------------------------------
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Password/forgotPassword</b></code></summary>

  Initiates the Forgot Password process.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | email  | string   | Yes      |    |
  | redirectUrl  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
  | `500`     | `application/json` | Server Error. |
  | `404`     | `application/json` | Not Found. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Password/resetPassword</b></code></summary>

  Resets the user's password.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | id  | guid   | Yes      |    |
  | token  | string   | Yes      |    |
  | password  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
  | `500`     | `application/json` | Server Error. |
  | `404`     | `application/json` | Not Found. |

</details>

Profile
------------------------------------------------------------------------------------------
<details>
  <summary><code>[DELETE]</code> <code><b>...</b></code> <code><b>/api/Profile/deleteUser</b></code></summary>

  Deletes the user profile.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | userId  | guid   | Yes      |    |


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Profile/updateUserProfile</b></code></summary>

  Updates the user profile.
  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | userId  | guid   | Yes      |    |


  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | firstName  | string   | No      |    |
  | lastName  | string   | No      |    |
  | phoneNumber  | string   | No      |    |
  | organization  | string   | No      |    |
  | department  | string   | No      |    |
  | avatarLink  | string   | No      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Profile/changeUserEmail</b></code></summary>

  Changes the user email (sends email).

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | redirectLink  | string   | Yes      |    |
  | userEmail  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Profile/verifyUserEmail</b></code></summary>

  Verifies the user email.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | token  | string   | Yes      |    |
  | userEmail  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Profile/changeUserPassword</b></code></summary>

  Changes the user password.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | oldPassword  | string   | Yes      |    |
  | newPassword  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Profile/changeUserPassword</b></code></summary>

  Changes the user password.

   ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | userId  | guid   | Yes      |    |

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | oldPassword  | string   | Yes      |    |
  | newPassword  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Profile/uploadAvatar</b></code></summary>

  Uploads the user avatar.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | userId  | guid   | Yes      |    |
  | base64Avatar  | string   | Yes      |    |
  | imageExtension  | int   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/Profile/getUserProfile</b></code></summary>

  Gets the user profile.

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>

Report
------------------------------------------------------------------------------------------

<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Report/sendReport</b></code></summary>

  Endpoint for sending a report asynchronously.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | receiverEmail  | string   | Yes      |    |
  | userEmail  | string   | Yes      |    |
  | userText  | string   | Yes      |    |
  | option  | string   | Yes      |    |
  | category  | string   | Yes      |    |
  | base64JpgFiles  | string array   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>

</details>

### <a name="tickets-manager-service"></a><ins>1.3.3 Tickets Manager Service</ins>

The Tickets Manager Service is a component that stores all user interactions with AI, including requests, responses, generated information, diagrams, and more. It acts as a central repository for all AI-related communication, providing a comprehensive view of user activity and facilitating efficient information management.

Key features:

- Centralized storage: The service stores all AI-related data in a single location, making it easy to access and manage.
- Comprehensive history: It maintains a complete record of all user interactions with AI, providing valuable insights into user behavior and preferences.
- Efficient information retrieval: The service offers quick and easy access to specific information, such as past requests, responses, and generated content.
- Enhanced collaboration: It facilitates collaboration between users and AI systems by providing a shared space for communication and information exchange.

<details>
	 <summary><code>Endpoints</code></summary>

ActionHistory
------------------------------------------------------------------------------------------
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/ActionHistory/createActionHistory</b></code></summary>

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketId  | guid   | Yes      |    |
  | actionHistoryEnum  | int   | Yes      |    |
  | ticketCurrentStepEnum  | int   | Yes      |    |
  | userEmail  | string   | Yes      |    |
  | oldValue  | string   | Yes      |    |
  | newValue  | string  | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/ActionHistory/getActionHistory</b></code></summary>

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketId  | guid   | Yes      |    |

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketCurrentStepEnum  | int   | Yes      |    |
  | pagination  | object   | Yes      |    |
  | currentPageNumber  | int   | Yes      |    |
  | recordsPerPage  | int  | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>

Tickets
------------------------------------------------------------------------------------------
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Tickets/tickets/{userId}</b></code></summary>

  Retrieves all tickets associated with the specified user

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | userId  | guid   | Yes      |    |

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | currentPageNumber  | int   | Yes      |    |
  | recordsPerPage  | int   | Yes      |    |


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Tickets/search/{userId}</b></code></summary>

  Get all tickets by user and search criteria, which checks if tickets title contains some string

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | userId  | guid   | Yes      |    |

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketTitle  | string   | Yes      |    |
  | pagination  | object   | Yes      |    |
  | currentPageNumber  | int   | Yes      |    |
  | recordsPerPage  | int   | Yes      |    |


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Tickets/{userId}</b></code></summary>

  Creates a new ticket for the specified user.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | userId  | guid   | Yes      |    |

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticket  | object   | Yes      |    |

  ```
  {
  "title": "string",
  "description": "string",
  "context": "string",
  "createdDate": "2024-03-05T09:08:48.617Z",
  "status": 0,
  "currentStep": 0,
  "bannersJson": "string",
  "messages": [
    {
      "sender": "string",
      "content": "string",
      "sendTime": "2024-03-05T09:08:48.617Z",
      "action": {
        "state": 0,
        "type": 0
      },
      "stage": 0,
      "subStage": 0
    }
  ],
  "notifications": [
    {
      "title": "string",
      "description": "string",
      "sms": true,
      "email": true,
      "push": true
    }
  ]
}
```


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[DELETE]</code> <code><b>...</b></code> <code><b>/api/Tickets/{id}</b></code></summary>

  Deletes a ticket.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | id  | guid   | Yes      |    |


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[PUT]</code> <code><b>...</b></code> <code><b>/api/Tickets/{id}</b></code></summary>

  Updates a ticket.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | id  | guid   | Yes      |    |

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticket  | object   | Yes      |    |

  ```
  {
  "title": "string",
  "description": "string",
  "context": "string",
  "createdDate": "2024-03-05T09:08:48.617Z",
  "status": 0,
  "currentStep": 0,
  "bannersJson": "string",
  "messages": [
    {
      "sender": "string",
      "content": "string",
      "sendTime": "2024-03-05T09:08:48.617Z",
      "action": {
        "state": 0,
        "type": 0
      },
      "stage": 0,
      "subStage": 0
    }
  ],
  "notifications": [
    {
      "title": "string",
      "description": "string",
      "sms": true,
      "email": true,
      "push": true
    }
  ]
}
```


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Tickets/generateDoc</b></code></summary>

  ##### Parameters (body)

  An array of base64-encoded data to be included in the DOC.
    ```
    [
      "string"
    ]
    ```


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>

TicketSummary
------------------------------------------------------------------------------------------
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/TicketSummary/getTicketSummariesByTicketId</b></code></summary>

  Get all ticket summaries by ticket id

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketId  | guid   | Yes      |    |


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/TicketSummary/createTicketSummaries</b></code></summary>

  Create ticket summaries

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketSummary  | object   | Yes      |    |

  ```
  [
  {
    "ticketId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "data": "string",
    "isPotential": true,
    "subStage": 0,
    "summaryScenarios": [
      {
        "title": "string",
        "description": "string"
      }
    ],
    "summaryAcceptanceCriteria": [
      {
        "title": "string",
        "description": "string"
      }
    ]
  }
]
  ```

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[PUT]</code> <code><b>...</b></code> <code><b>/api/TicketSummary/updateTicketSummaries</b></code></summary>

  Updates ticket summaries

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketSummary  | object   | Yes      |    |

  ```
  [
  {
    "data": "string",
    "isPotential": true,
    "subStage": 0,
    "summaryScenarios": [
      {
        "title": "string",
        "description": "string",
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
      }
    ],
    "summaryAcceptanceCriteria": [
      {
        "title": "string",
        "description": "string",
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
      }
    ],
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "ticketId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
]
  ```

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[DELETE]</code> <code><b>...</b></code> <code><b>/api/TicketSummary/deleteTicketSummaries</b></code></summary>

  Delete ticket summaries by ticket id

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketId  | guid   | Yes      |    |


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>

Usecases
------------------------------------------------------------------------------------------

<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Usecases/create</b></code></summary>

  Endpoint to create diagrams and tables in use cases.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | usecases  | object   | Yes      |    |

  ```
  [
  {
    "usecase": {
      "title": "string",
      "description": "string",
      "tables": [
        {
          "table": "string"
        }
      ],
      "diagrams": [
        {
          "title": "string",
          "description": "string",
          "pictureLink": "string"
        }
      ]
    },
    "ticketId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
]
  ```

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Usecases/get</b></code></summary>

  Endpoint to get diagrams and tables in use cases.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | getObject  | object   | Yes      |    |

  ```
  {
  "paginationRequest": {
    "currentPageNumber": 0,
    "recordsPerPage": 0
  },
  "ticketId": "string guid"
}
  ```

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[PUT]</code> <code><b>...</b></code> <code><b>/api/Usecases/update</b></code></summary>

  Endpoint to update diagrams and tables in use cases.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | usecaseId  | guid   | Yes      |    |

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | updateObject  | object   | Yes      |    |

  ```
    {
      "title": "string",
      "description": "string",
      "diagrams": [
        {
          "title": "string",
          "description": "string",
          "pictureLink": "string",
          "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        }
      ],
      "tables": [
        {
          "table": "string",
          "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        }
      ]
    }
  ```

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>
<details>
  <summary><code>[DELETE]</code> <code><b>...</b></code> <code><b>/api/Usecases/delete</b></code></summary>

  Endpoint to deletes diagrams and tables in use cases.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketId  | guid   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |

</details>

UserStoryTest
------------------------------------------------------------------------------------------

<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/UserStoryTest/createUserStoryTest</b></code></summary>

  Endpoint to create user story tests.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | userStoryTest  | object   | Yes      |    |

  ```
  [
  {
    "ticketSummaryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "testScenarios": "string",
    "testCases": [
      {
        "testCaseId": "string",
        "description": "string",
        "preConditions": "string",
        "testSteps": "string",
        "testData": "string",
        "expectedResult": "string"
      }
    ],
    "testPlan": {
      "objective": "string",
      "scope": "string",
      "resources": "string",
      "schedule": "string",
      "testEnvironment": "string",
      "riskManagement": "string"
    }
  }
  ]
  ```

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/UserStoryTest/getUserStoryTests</b></code></summary>

  Endpoint to get user story tests.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketId  | object   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[PUT]</code> <code><b>...</b></code> <code><b>/api/UserStoryTest/updateUserStoryTests</b></code></summary>

  Endpoint to create user story tests.

  ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | userStoryTest  | object   | Yes      |    |

  ```
  [
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "testScenarios": "string",
    "testCases": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "testCaseId": "string",
        "description": "string",
        "preConditions": "string",
        "testSteps": "string",
        "testData": "string",
        "expectedResult": "string"
      }
    ],
    "testPlan": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "objective": "string",
      "scope": "string",
      "resources": "string",
      "schedule": "string",
      "testEnvironment": "string",
      "riskManagement": "string"
    },
    "ticketSummaryData": "string",
    "ticketSummaryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
  ]
  ```

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[DELETE]</code> <code><b>...</b></code> <code><b>/api/UserStoryTest/deleteUserStoryTests</b></code></summary>

  Endpoint to deletes user story tests.

  ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | ticketId  | guid   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[DELETE]</code> <code><b>...</b></code> <code><b>/api/UserStoryTest/deleteUserStoryCases</b></code></summary>

  Endpoint to deletes user story cases.

  ##### Parameters (body)
  
  ```
  [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  ]
  ```

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
</details>

### <a name="payment-manager-service"></a><ins>1.3.4 Payment Manager Service</ins>

The Payment Manager Service is a component that enables in-app purchases. It facilitates two types of purchases:

1. One-time purchases: These involve the purchase of tokens that users can use to access the app's features.

2. Subscriptions: These provide users with a monthly allocation of tokens and unlock additional features within the app.

<img src="https://img.shields.io/badge/G%20pay-2875E3?style=for-the-badge&logo=googlepay&logoColor=white">   <img src="https://img.shields.io/badge/apple%20pay-007AFF?style=for-the-badge&logo=apple%20pay&logoColor=white">
<img src="https://img.shields.io/badge/PayPal-00457C?style=for-the-badge&logo=paypal&logoColor=white">   <img src="https://img.shields.io/badge/Stripe-626CD9?style=for-the-badge&logo=Stripe&logoColor=white">

Key features:

- Secure payment processing: The service uses secure payment gateways to process payments safely and reliably.
- Flexible payment options: It supports various payment methods, such as credit cards, debit cards, and PayPal, to accommodate user preferences.
- Subscription management: The service allows users to easily manage their subscriptions, including viewing their subscription history, canceling subscriptions, and updating payment information.
- Fraud prevention: The service employs fraud prevention measures to protect users from fraudulent transactions.

<details>
	 <summary><code>Endpoints</code></summary>

CheckOut
------------------------------------------------------------------------------------------
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/CheckOut/getUserBalance</b></code></summary>

  Retrieves user balance.

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/CheckOut/createPayment</b></code></summary>

  Sends request to create one time payment.

   ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | connectionId  | string   | Yes      |    |
  | signalMethodName  | string   | Yes      |    |
  | successUrl  | string   | Yes      |    |
  | cancelUrl  | string   | Yes      |    |
  | amountOfPoints  | int   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/CheckOut/createFreeSubscription</b></code></summary>

  Sends request to create new subscription.

   ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | connectionId  | string   | Yes      |    |
  | signalMethodName  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/CheckOut/upgradeSubscription</b></code></summary>

   ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | newPriceId  | string   | Yes      |    |


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/CheckOut/downgradeSubscription</b></code></summary>

   ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | newPriceId  | string   | Yes      |    |


  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/CheckOut/cancelSubscription</b></code></summary>

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>

Customer
------------------------------------------------------------------------------------------
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/Customer/createCustomer</b></code></summary>

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/Customer/updateCustomerData</b></code></summary>

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Customer/addCustomerPaymentMethod</b></code></summary>

   ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | connectionId  | string   | Yes      |    |
  | signalMethodName  | string   | Yes      |    |
  | successUrl  | string   | Yes      |    |
  | cancelUrl  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Customer/getAllPaymentMethods</b></code></summary>

   ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | recordsPerPage  | int   | Yes      |    |
  | startAfter  | string   | Yes      |    |
  | endBefore  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/Customer/setDefaultPaymentMethod</b></code></summary>

   ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | paymentMethodId  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Customer/getCustomerPayments</b></code></summary>

   ##### Parameters (body)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | recordsPerPage  | int   | Yes      |    |
  | startAfter  | string   | Yes      |    |
  | endBefore  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/Customer/getActiveSubscription</b></code></summary>

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/Customer/updateSubscriptionPreview</b></code></summary>

   ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | newPriceId  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[GET]</code> <code><b>...</b></code> <code><b>/api/Customer/detachPaymentMethod</b></code></summary>

   ##### Parameters (query)

  | Attribute          | Type     | Required | Description                               |
  |--------------------|----------|----------|--------------------------------------------|
  | paymentMethodId  | string   | Yes      |    |

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
<details>
  <summary><code>[DELETE]</code> <code><b>...</b></code> <code><b>/api/Customer/deleteCustomer</b></code></summary>

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>

Product
------------------------------------------------------------------------------------------
<details>
  <summary><code>[DELETE]</code> <code><b>...</b></code> <code><b>/api/Product/getProducts</b></code></summary>

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>

Webhook
------------------------------------------------------------------------------------------
<details>
  <summary><code>[POST]</code> <code><b>...</b></code> <code><b>/api/Webhook</b></code></summary>

  ##### Responses

  | http code | content-type | Description |
  |-----------|--------------|-------------|
  | `200`     | `application/json` | Success. |
</details>
</details>


### <a name="database"></a><ins>1.4 Database</ins>
PostgreSQL Flexible Server with Multiple Databases
Here's an expanded description of the information you provided:

Service: PostgreSQL Flexible Server on Azure: https://azure.microsoft.com/

<img src="https://img.shields.io/badge/microsoft%20azure-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white">   <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white">

Purpose: Hosts and manages relational databases.

Benefits:

- Scalability: Easily scale storage and computing resources based on your needs.
- High availability: Provides options for ensuring continuous database operation even during failures.
- Security: Offers built-in security features to protect your data.
- Cost-effectiveness: Pay only for the resources you use.
- Connection Strings: Secure strings are used to connect applications to the databases on the server. These strings typically include information like server address, port, database name, username, and password.

Data Management:

<details>
	<summary><code>Main Database (ellogy)</code> : stores the core application data used for regular operations.</summary>
<img src="https://github.com/El-Technology/Ellogy_App_Backend/blob/pre-release/images/ellogyDatabase.png">
</details>
<details>
	<summary><code>Payment Database</code> : stores payment-related information such as transactions, user payment methods, and potentially financial data (ensures secure storage and access controls for such data).</summary>
<img src="https://github.com/El-Technology/Ellogy_App_Backend/blob/pre-release/images/paymentDatabase.png">
</details>
<details>
	<summary><code>Vector Database</code> : Machine learning models or other vector-based data structures for communicating with AI models using your data that are stored in files.</summary>
<img src="https://github.com/El-Technology/Ellogy_App_Backend/blob/pre-release/images/vectorDatabase.png">
</details>

### <a name="blob-storage"></a><ins>1.5 Blob Storage</ins>

Blob Storage temporarily stores files for NotificationService and Azure Functions, categorized into templates and images.

Also, have private and public folders for storing user avatars and files which used with RAG functionality.

### <a name="message-bus"></a><ins>1.6 Message Bus</ins>

Azure Message Bus, often called Azure Service Bus, is a cloud-based messaging service provided by Microsoft Azure. It facilitates communication and integration
between various components of distributed applications, allowing them to exchange data reliably and asynchronously. One of the key features of Azure Service Bus is its
support for messaging patterns like publish-subscribe, request-response, and queuing.

1) Sending Messages: In your service that needs to send notifications (the sender), you will use the Azure Service Bus SDK to send messages to the NotificationQueue.
The sender will serialize the notification data into a message and send it to the queue. Certain events, actions, or on a schedule can trigger this.
2) Receiving Messages: In the receiving service (the consumer), you'll use the same SDK to read messages from the NotificationQueue. This service will be constantly
listening to the queue for new messages. When a message arrives, it can process the notification or take any other appropriate action.
4) Message Structure: Messages sent through the queue can be structured in various ways. Typically, they consist of a message body that holds the data you want to share between services.
This could be a JSON payload, XML, or any other format that fits your needs. You can also include metadata like message identifiers, timestamps, and other relevant information.
Using a message queue like NotificationQueue in Azure Service Bus allows your services to communicate asynchronously, making them more resilient to failures and capable of handling varying
workloads. It decouples the sender and receiver, enabling them to evolve independently and ensuring smoother integration between different components of your application.
In our case, when sending messages between services, they contain only basic information and links to third-party resources, for example, to templates or files from blob storage, this is
done in order not to overload the message with heavy information and to optimize performance.


### <a name="email-communication"></a><ins>1.7 Email Communication</ins>

For email communication specifically, Azure Communication Services enables you to send transactional and marketing emails directly from your application using REST APIs or SDKs. 
This can be particularly useful for scenarios where you need to send notifications, updates, or other types of messages to users via email. Some key points about email communication in Azure Communication Services include:
1) Transactional and Marketing Emails: You can send both transactional emails (such as account verification, and password reset) and marketing emails (promotions, and newsletters) through the service.
2) Templates and Personalization: Azure Communication Services allows you to create email templates and personalize them with dynamic content. This helps maintain a consistent look and feel across emails while tailoring content to individual recipients.
3) API Integration: You can interact with the email communication service using REST APIs or SDKs provided by Azure. This makes it easier to integrate email functionality into your existing applications.
4) Tracking and Reporting: The service offers tracking and reporting capabilities, allowing you to monitor the delivery status of your emails and gather insights on user engagement.
5) Security and Compliance: Azure Communication Services takes security and compliance seriously. It provides features like encryption and data residency options to help you meet regulatory requirements.
6) Scalability and Reliability: Since Azure Communication Services is built on Microsoft Azure, it benefits from the scalability and reliability of the Azure platform. This ensures that your email communications can handle varying loads and maintain high availability.
7) Integration with Other Communication Channels: Azure Communication Services goes beyond email and also provides capabilities for other communication channels like SMS, voice, and chat. This enables you to create unified and consistent communication experiences across different channels.


### <a name="azure-function"></a><ins>1.8 Azure Function</ins>

This is the main service that combines all the separate functionality of Blob Storage, MessageBus, and Email Communication into a single entity. 
This is an event listener. When a new message appears in the message bus queue, the openwork function immediately retrieves it, reads it, and 
decides what needs to be done with it next, which files need to be uploaded to fill this message, to whom and from whom to send it, and in what way.

---

## <a name="github-structure"></a>2. GitHub Structure   <img src="https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white">

GitHub orchestrates the entire code deployment workflow, ensuring efficient and controlled software delivery.

### <a name="version-control"></a><ins>2.1 Version Control</ins>

GitHub hosts all source code, allowing collaboration, tracking changes, and maintaining version history via Git.

### <a name="git-actions"></a><ins>2.2 Git Actions</ins>

Git Actions for Continuous Integration (CI): Git Actions are employed to automate various tasks whenever new code is pushed or changes are made to the repository. This includes running tests, code quality checks, and creating build artifacts.

### <a name="docker-containerization"></a><ins>2.3 Docker Containerization</ins>

<img src="https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white">

For some services, the code is packaged into Docker containers. Docker allows you to encapsulate the application and its dependencies, ensuring consistency across different environments.
Git Actions can include steps to build Docker images, pushing them to a container registry like Docker Hub or Azure Container Registry.

### <a name="deployment-to-azure"></a><ins>2.4 Deployment to Azure</ins>

Depending on the service's nature, there are two deployment paths:
1)	For some services, the built code is directly deployed to Azure through Git, using services like Azure Web Apps or Azure Functions.
2)	For other services, the CI/CD pipeline packs application instances into containers first and then uploads them to a virtual machine running on Azure, managed by Azure infrastructure.


### <a name="git-secrets-importance"></a><ins>2.5 Git Secrets Importance</ins>

Git secrets are sensitive pieces of information such as API keys, passwords, or access tokens required for the application to function.
Managing secrets securely is vital to prevent unauthorized access to sensitive data. Git Actions can interact with Git secrets to access
these credentials securely during the build or deployment process. Leaking secrets can lead to security breaches, data leaks, or unauthorized access to resources.

### <a name="summary-about-git"></a><ins>2.6 Summary about Git</ins>

In summary, the entire code pipeline starts with versioned code maintained on GitHub. Git Actions automate processes like testing, building Docker images,
and deploying to Azure services. Docker containers offer consistent deployment across environments, and Git secrets ensure that sensitive information is
managed securely. This comprehensive approach streamlines development, enhances code quality, and ensures robust deployment to Azure services.

### <a name="unit-tests"></a><ins>2.7 Unit Tests</ins>

We covered most of the unit's services with tests but with an approach similar to integration tests, where we created a test environment through which we ran all the functionality.
In the unit tests section, tests are placed through the deploy settings. Before we start downloading and deploying the code, tests of the service we plan to update are
automatically run on the Azure services. If at least one of the test cases fails, the deployment is canceled and the stable version is rolled back.
This is done to prevent human error when someone forgets to run tests after making changes to the code.

---

## <a name="additional-information"></a>3. Additional Information

### <a name="azure-templates"></a><ins>3.1 Azure Templates</ins>

Azure Templates, also known as Azure Resource Manager (ARM) templates, are a powerful way to define and deploy your Azure infrastructure as code. They allow you to define the resources, configurations, and dependencies for your application or solution in a declarative JSON format. With ARM templates, you can automate the provisioning and management of your Azure resources, ensuring consistency, repeatability, and scalability.
Key features and concepts of Azure Templates include:
•	Declarative Syntax: ARM templates use a declarative syntax to describe the desired state of your Azure resources. You define what resources you want, their properties, and the relationships between them, without specifying the exact steps to create them.

•	Infrastructure as Code (IaC): Templates enable you to treat your infrastructure as code, allowing you to version, test, and manage it just like your application code. This helps eliminate manual configuration and reduces the chance of errors.


•	Resource Definitions: Templates can define a wide range of Azure resources, such as virtual machines, storage accounts, databases, virtual networks, and more. You specify properties, such as resource names, sizes, locations, and configurations.

•	Resource Dependencies: ARM templates allow you to define dependencies between resources. For example, you can specify that a virtual machine should only be created after a storage account is provisioned.

•	Parameters and Variables: You can use parameters to make your templates reusable across different environments or scenarios. Variables allow you to store values that can be referenced throughout the template.

•	Outputs: Templates can also define outputs, which are values generated by the deployment that can be useful for other processes or scripts.

•	Template Functions: ARM templates offer a wide range of built-in functions that help you manipulate data, generate unique names, concatenate strings, and perform other tasks within the template.

•	Template Validation: Azure provides tools for validating ARM templates before deployment, which helps catch errors and issues early in the development process.

•	Azure Resource Manager: ARM templates are used by Azure Resource Manager to orchestrate the deployment and management of your resources. Azure Resource Manager ensures that the resources are created, updated, and deleted in the right order and with the correct configurations.

•	Azure Portal Integration: You can create, deploy, and manage ARM templates directly through the Azure Portal, making it easy to visualize, modify, and track your infrastructure deployments.

Overall, Azure Templates empower you to automate the provisioning and management of your Azure resources, resulting in consistent and repeatable deployments. They are a fundamental tool for implementing Infrastructure as Code practices and are well-suited for managing complex and scalable applications in the cloud.


### <a name="obtaining-a-template"></a><ins>3.2 Obtaining a Template</ins>

To obtain a template, navigate to the resource group, scroll to the bottom, and export the template.

### <a name="how-to-use-the-template"></a><ins>3.3 How to Use the Template</ins>

1. Go to the resource group.
2. Click **Export template** and then **Deploy**.
3. Edit the template if necessary and follow Azure's deployment instructions.

### <a name="openai-functions"></a><ins>3.4 OpenAI Functions</ins>

A new endpoint for communicating with artificial intelligence has been added that supports the use of the feature. What is it?
This is the ability to write code. It will set the response format in JSON. That is, we can say what structure to follow and it will always be so.
To do this, you need to add a value to the template stored in the database in the function field, for example: 
```[{'name':'Json','parameters':{'type':'object','required':['title','description'],'properties':{'title':{'type':'string'},'description':{'type':'string'}}}}]```
This example returns a Json with the title and description fields, which will be strings.
This greatly facilitates the exchange of returned information.

