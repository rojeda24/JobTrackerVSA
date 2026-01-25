@description('The location for all resources.')
param location string = resourceGroup().location

@description('The name prefix for the app. Must be unique.')
param appNamePrefix string = 'jtracker'

@description('The SQL Admin Login.')
param sqlAdminLogin string = 'sqladmin'

@description('The SQL Admin Password.')
@secure()
param sqlAdminPassword string

@description('Auth0 Domain')
param auth0Domain string

@description('Auth0 Client ID')
param auth0ClientId string

@description('Auth0 Client Secret')
@secure()
param auth0ClientSecret string

var appName = 'jobtracker-raul'
var sqlServerName = 'sql-${appName}'
var databaseName = 'JobTrackerVSA'

// 1. App Service Plan (Linux Free Tier F1)
resource appServicePlan 'Microsoft.Web/serverfarms@2025-03-01' = {
  name: 'plan-${appName}'
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
  }
  kind: 'linux'
  properties: {
    reserved: true // Required for Linux
  }
}

// 2. Web App (Linux)
resource webApp 'Microsoft.Web/sites@2025-03-01' = {
  name: appName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0' // Linux Runtime Stack
      alwaysOn: false
      connectionStrings: [
        {
          name: 'DefaultConnection'
          connectionString: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${databaseName};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
          type: 'SQLAzure'
        }
      ]
      appSettings: [
        {
          name: 'Auth0__Domain'
          value: auth0Domain
        }
        {
          name: 'Auth0__ClientId'
          value: auth0ClientId
        }
        {
          name: 'Auth0__ClientSecret'
          value: auth0ClientSecret
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
      ]
    }
  }
}

// 3. SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-08-01' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
  }
}

// 4. SQL Database (S0 - Standard 10 DTUs - 12 Months Free Offer compatible)
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
    capacity: 10
  }
}

// 5. Firewall Rule (Allow Azure Services)
resource allowAzureIps 'Microsoft.Sql/servers/firewallRules@2023-08-01' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output webAppName string = webApp.name
output webAppUrl string = webApp.properties.defaultHostName
output sqlServerName string = sqlServer.name