# Back-end Documentation

## [1. Azure Architecture](#azure-architecture)
   - [1.1 API Gateway](#api-gateway)
   - [1.2 Virtual Machine](#virtual-machine)
   - [1.3 AI Communication Service](#ai-communication-service)
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

---

## <a name="azure-architecture"></a>1. Azure Architecture
<img src="https://github.com/El-Technology/Ellogy_App_Backend/blob/pre-release/images/Azure%20architecture.vpd.png">
The entire backend of the application is hosted on Azure. Here's a breakdown of key services:

### <a name="api-gateway"></a>1.1 API Gateway

API Gateway, hosted on Azure and utilizing "Ocelot," simplifies communication with microservices. It handles routing, aggregation, load balancing, authentication, caching, and more.
Also, the gateway has its own domain `backend.ellogy.ai`, which means that all incoming requests are sent to the domain and automatically forwarded to the necessary services and methods,
for example, instead of `http://20.21.124.185:5281/api/auth/login`, you only need to send a request to `backend.ellogy.ai/gateway/auth/login`,
which greatly simplifies the work of front-end developers when they do not need to know on which port a particular service is running.

### <a name="virtual-machine"></a>1.2 Virtual Machine

The Virtual Machine runs multiple services independently. It uses Ubuntu 22.04.2 LTS and allows access via Git Bash. Services are accessed on different ports and documented via Swagger.
All five services are running on different ports:
1) `5281` - UserManager
2) `5041` - TicketsManager
3) `53053` – AICommunication
4) `8080` – PlantUML
5) `5157` - AdminPanel
   
The same virtual machine IP address is used for all of them.
For example.
`http://20.21.124.185:5281/swagger/index.html`

Just for PlantUML we don`t need to use ```/swagger/index.html```


### <a name="ai-communication-service"></a>1.3 AI Communication Service

Recent updates include transitioning to Azure OpenAI models, utilizing GPT-4, creating a custom communication library, implementing functional communication, and enabling streaming of AI responses.

### <a name="database"></a>1.4 Database

PostgreSQL single server hosts the database, providing access via connection strings for data management, backups, and edits.

### <a name="blob-storage"></a>1.5 Blob Storage

Blob Storage temporarily stores files for NotificationService and Azure Functions, categorized into templates and images.

### <a name="message-bus"></a>1.6 Message Bus

Azure Message Bus, often referred to as Azure Service Bus, is a cloud-based messaging service provided by Microsoft Azure. It facilitates communication and integration
between various components of distributed applications, allowing them to exchange data reliably and asynchronously. One of the key features of Azure Service Bus is its
support for messaging patterns like publish-subscribe, request-response, and queuing.

1) Sending Messages: In your service that needs to send notifications (the sender), you will use the Azure Service Bus SDK to send messages to the NotificationQueue.
The sender will serialize the notification data into a message and send it to the queue. This can be triggered by certain events, actions, or on a schedule.
2) Receiving Messages: In the receiving service (the consumer), you'll use the same SDK to read messages from the NotificationQueue. This service will be constantly
listening to the queue for new messages. When a message arrives, it can process the notification or take any other appropriate action.
4) Message Structure: Messages sent through the queue can be structured in various ways. Typically, they consist of a message body that holds the data you want to share between services.
This could be a JSON payload, XML, or any other format that fits your needs. You can also include metadata like message identifiers, timestamps, and other relevant information.
Using a message queue like NotificationQueue in Azure Service Bus allows your services to communicate asynchronously, making them more resilient to failures and capable of handling varying
workloads. It decouples the sender and receiver, enabling them to evolve independently and ensuring smoother integration between different components of your application.
In our case, when sending messages between services, they contain only basic information and links to third-party resources, for example, to templates or files from blob storage, this is
done in order not to overload the message with heavy information and to optimize performance.


### <a name="email-communication"></a>1.7 Email Communication

For email communication specifically, Azure Communication Services enables you to send transactional and marketing emails directly from your application using REST APIs or SDKs. 
This can be particularly useful for scenarios where you need to send notifications, updates, or other types of messages to users via email. Some key points about email communication in Azure Communication Services include:
1) Transactional and Marketing Emails: You can send both transactional emails (such as account verification, and password reset) and marketing emails (promotions, and newsletters) through the service.
2) Templates and Personalization: Azure Communication Services allows you to create email templates and personalize them with dynamic content. This helps maintain a consistent look and feel across emails while tailoring content to individual recipients.
3) API Integration: You can interact with the email communication service using REST APIs or SDKs provided by Azure. This makes it easier to integrate email functionality into your existing applications.
4) Tracking and Reporting: The service offers tracking and reporting capabilities, allowing you to monitor the delivery status of your emails and gather insights on user engagement.
5) Security and Compliance: Azure Communication Services takes security and compliance seriously. It provides features like encryption and data residency options to help you meet regulatory requirements.
6) Scalability and Reliability: Since Azure Communication Services is built on Microsoft Azure, it benefits from the scalability and reliability of the Azure platform. This ensures that your email communications can handle varying loads and maintain high availability.
7) Integration with Other Communication Channels: Azure Communication Services goes beyond email and also provides capabilities for other communication channels like SMS, voice, and chat. This enables you to create unified and consistent communication experiences across different channels.


### <a name="azure-function"></a>1.8 Azure Function

This is the main service that combines all the separate functionality of Blob Storage, MessageBus, and Email Communication into a single entity. 
This is an event listener. When a new message appears in the message bus queue, the openwork function immediately retrieves it, reads it, and 
decides what needs to be done with it next, which files need to be uploaded to fill this message, to whom and from whom to send it, and in what way.

---

## <a name="github-structure"></a>2. GitHub Structure

GitHub orchestrates the entire code deployment workflow, ensuring efficient and controlled software delivery.

### <a name="version-control"></a>2.1 Version Control

GitHub hosts all source code, allowing collaboration, tracking changes, and maintaining version history via Git.

### <a name="git-actions"></a>2.2 Git Actions

Git Actions for Continuous Integration (CI): Git Actions are employed to automate various tasks whenever new code is pushed or changes are made to the repository. This includes running tests, code quality checks, and creating build artifacts.

### <a name="docker-containerization"></a>2.3 Docker Containerization

For some services, the code is packaged into Docker containers. Docker allows you to encapsulate the application and its dependencies, ensuring consistency across different environments.
Git Actions can include steps to build Docker images, pushing them to a container registry like Docker Hub or Azure Container Registry.

### <a name="deployment-to-azure"></a>2.4 Deployment to Azure

Depending on the service's nature, there are two deployment paths:
1)	For some services, the Docker containers are deployed to Azure's Kubernetes Service (AKS) or Azure App Service, enabling scalable and managed deployments.
2)	For other services, the built code is directly deployed to Azure through Git, using services like Azure Web Apps or Azure Functions.


### <a name="git-secrets-importance"></a>2.5 Git Secrets Importance

Git secrets are sensitive pieces of information such as API keys, passwords, or access tokens required for the application to function.
Managing secrets securely is vital to prevent unauthorized access to sensitive data. Git Actions can interact with Git secrets to access
these credentials securely during the build or deployment process. Leaking secrets can lead to security breaches, data leaks, or unauthorized access to resources.

### <a name="summary-about-git"></a>2.6 Summary about Git

In summary, the entire code pipeline starts with versioned code maintained on GitHub. Git Actions automate processes like testing, building Docker images,
and deploying to Azure services. Docker containers offer consistent deployment across environments, and Git secrets ensure that sensitive information is
managed securely. This comprehensive approach streamlines development, enhances code quality, and ensures robust deployment to Azure services.

### <a name="unit-tests"></a>2.7 Unit Tests

We covered most of the unit's services with tests but with an approach similar to integration tests, where we created a test environment through which we ran all the functionality.
In the unit tests section, tests are placed through the deploy settings. Before we start downloading and deploying the code, tests of the service we plan to update are
automatically run on the Azure services. If at least one of the test cases fails, the deployment is canceled and the stable version is rolled back.
This is done to prevent human error when someone forgets to run tests after making changes to the code.

---

## <a name="additional-information"></a>3. Additional Information

### <a name="azure-templates"></a>3.1 Azure Templates

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


### <a name="obtaining-a-template"></a>3.2 Obtaining a Template

To obtain a template, navigate to the resource group, scroll to the bottom, and export the template.

### <a name="how-to-use-the-template"></a>3.3 How to Use the Template

1. Go to the resource group.
2. Click **Export template** and then **Deploy**.
3. Edit the template if necessary and follow Azure's deployment instructions.

### <a name="openai-functions"></a>3.4 OpenAI Functions

A new endpoint for communicating with artificial intelligence has been added that supports the use of the feature. What is it?
This is the ability to write code. It will set the response format in JSON. That is, we can say what structure to follow and it will always be so.
To do this, you need to add a value to the template stored in the database in the function field, for example: 
```[{'name':'Json','parameters':{'type':'object','required':['title','description'],'properties':{'title':{'type':'string'},'description':{'type':'string'}}}}]```
This example returns a Json with the title and description fields, which will be strings.
This greatly facilitates the exchange of returned information.

