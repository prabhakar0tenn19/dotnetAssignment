# Catalog.Api - ASP.NET Core Web API (.NET 10 & Swagger)

यह एक मिनिमल, क्लीन और कंट्रोलर-बेस्ड **ASP.NET Core Web API** असाइनमेंट प्रोजेक्ट है, जिसे **.NET 10** और **Swagger (Swashbuckle)** का उपयोग करके बनाया गया है। इसमें इन-मेमोरी स्टोरेज के ज़रिए प्रोडक्ट्स की CRUD ऑपरेशन्स परफॉर्म की जाती हैं।

---

## Technical Stack & Configuration
1. **Target Framework**: `.NET 10.0`
2. **API Documentation**: `Swashbuckle.AspNetCore` (Version 10.2.3) - Swagger UI (api/swagger)
3. **Data Storage**: In-Memory `Dictionary<int, Product>`
4. **Dependency Injection (DI)**: Singleton lifetime for Repository to persist data across HTTP requests.
5. **Configuration**: Options Pattern (`CatalogOptions`) mapped with `DefaultPageSize` in `appsettings.json`.

---

## Project Structure (प्रोजेक्ट फ़ोल्डर संरचना)

प्रोजेक्ट में निम्न फ़ोल्डर संरचना (Folder Structure) फॉलो की गई है:
* **[Catalog.Api](file:///c:/Users/USER/Desktop/dotnetAssignment/Catalog.Api)**: Main API Project directory.
  * **[Controllers](file:///c:/Users/USER/Desktop/dotnetAssignment/Catalog.Api/Controllers)**: HTTP requests हैंडल करने के लिए Controllers.
    * `ProductsController.cs` - CRUD endpoints (GET, POST, PUT, DELETE).
  * **[Models](file:///c:/Users/USER/Desktop/dotnetAssignment/Catalog.Api/Models)**: Domain entities.
    * `Product.cs` - Product record.
  * **[DTOs](file:///c:/Users/USER/Desktop/dotnetAssignment/Catalog.Api/DTOs)**: Data Transfer Objects.
    * `CreateProductDto.cs` - Request body schema for creation and updates.
  * **[Repositories](file:///c:/Users/USER/Desktop/dotnetAssignment/Catalog.Api/Repositories)**: Data Access Layer abstraction and implementation.
    * `IProductRepository.cs` - Asynchronous operations interface.
    * `InMemoryProductRepository.cs` - In-memory implementation using a plain Dictionary.
  * **[Options](file:///c:/Users/USER/Desktop/dotnetAssignment/Catalog.Api/Options)**: Configuration settings.
    * `CatalogOptions.cs` - Application configuration class.

---

## Step-by-Step Implementation Details (हमने क्या और कैसे किया)

### Step 1: Project Setup and Cleanup (सेटअप और क्लीनअप)
* `.NET 10` CLI का उपयोग करके एक कंट्रोलर-बेस्ड वेब एपीआई प्रोजेक्ट क्रिएट किया:
  ```bash
  dotnet new webapi --use-controllers -f net10.0 -n Catalog.Api
  ```
* डिफ़ॉल्ट टेम्पलेट बॉयलरप्लेट फाइलों (`WeatherForecast.cs` और `WeatherForecastController.cs`) को डिलीट कर दिया।
* .NET 10 की डिफ़ॉल्ट OpenAPI dependency को हटाकर, असाइनमेंट की आवश्यकतानुसार **Swagger (Swashbuckle.AspNetCore)** पैकेज इनस्टॉल किया:
  * `Microsoft.AspNetCore.OpenApi` को हटाया।
  * `Swashbuckle.AspNetCore` को प्रोजेक्ट में जोड़ा।
* **Program.cs** में `AddEndpointsApiExplorer()`, `AddSwaggerGen()`, `UseSwagger()`, और `UseSwaggerUI()` को कॉन्फ़िगर किया।

### Step 2: Models & DTOs Creation (मॉडल और DTO)
* **Product.cs**: `Product` record को `Id` (int), `Name` (string), `Price` (decimal), और `Category` (string) के साथ परिभाषित किया।
* **CreateProductDto.cs**: नया प्रोडक्ट क्रिएट या अपडेट करने के लिए रिक्वेस्ट पे-लोड क्लास बनाई (बिना `Id` के)।

### Step 3: In-Memory Data Storage (डाटा स्टोरेज और रिपोजिटरी)
* **IProductRepository.cs**: पूरी तरह से Asynchronous CRUD ऑपरेशन्स के लिए इंटरफ़ेस डिक्लेयर किया।
* **InMemoryProductRepository.cs**: `Dictionary<int, Product>` की मदद से इन-मेमोरी स्टोरेज इम्प्लीमेंट किया। 
* एक `_nextId` ऑटो-इन्क्रीमेंट इंटीजर काउंटर के ज़रिए हर नए प्रोडक्ट को यूनिक ID दी जाती है।
* टेस्टिंग आसान करने के लिए कंस्ट्रक्टर में 3 प्रोडक्ट्स को प्री-सीड (pre-seed) किया गया है।

### Step 4: DI & Configuration via Options Pattern (डिपेंडेंसी इंजेक्शन और कॉन्फ़िगरेशन)
* `InMemoryProductRepository` को `IProductRepository` के साथ **Singleton** लाइफटाइम में रजिस्टर किया ताकि इन-मेमोरी डाटा एपीआई रिक्वेस्ट्स के बीच डिलीट न हो और सेव रहे:
  ```csharp
  builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
  ```
* `appsettings.json` में `"Catalog": { "DefaultPageSize": 2 }` कॉन्फ़िगरेशन ब्लॉक जोड़ा।
* `CatalogOptions.cs` क्लास बनाई और उसे `Program.cs` में Options Pattern के ज़रिए बाइंड किया:
  ```csharp
  builder.Services.Configure<CatalogOptions>(builder.Configuration.GetSection(CatalogOptions.SectionName));
  ```
* `ProductsController` के कंस्ट्रक्टर में `IOptions<CatalogOptions>` इंजेक्ट किया और `GET /api/products` एंडपॉइंट पर रिटर्न होने वाले प्रोडक्ट्स की संख्या को `Take(_options.DefaultPageSize)` से लिमिट किया।

### Step 5: CRUD & Health Endpoints (कंट्रोलर एंडपॉइंट्स)
* **GET `/api/products/health`**: प्रोजेक्ट का हेल्थ स्टेटस (JSON) रिटर्न करता है (200 OK)।
* **GET `/api/products`**: कॉन्फ़िगरेशन साइज़ (DefaultPageSize) के हिसाब से प्रोडक्ट्स की लिस्ट रिटर्न करता है (200 OK)।
* **GET `/api/products/{id}`**: ID से सर्च करके प्रोडक्ट रिटर्न करता है। मिलने पर 200 OK, न मिलने पर 404 Not Found।
* **POST `/api/products`**: नया प्रोडक्ट इंसर्ट करता. इनपुट वैलिडेशन पर 400 Bad Request और सक्सेस पर `CreatedAtAction` का उपयोग करके `201 Created` स्टेटस कोड और लोकेशन हेडर रिटर्न करता है।
* **PUT `/api/products/{id}`**: स्पेसिफिक ID वाले प्रोडक्ट की डिटेल्स को DTO से अपडेट करता है। सक्सेस होने पर 204 No Content और न मिलने पर 404 Not Found।
* **DELETE `/api/products/{id}`**: Product को रिमूव करता है। सक्सेस पर 204 No Content और न मिलने पर 404 Not Found।

---

## How to Run & Test (प्रोजेक्ट को कैसे रन और टेस्ट करें)

### 1. Build the Project
प्रोजेक्ट डायरेक्टरी में जाएँ और रीबिल्ड कमांड चलाएँ:
```bash
dotnet build Catalog.Api
```

### 2. Run the Web API
एपीआई सर्वर स्टार्ट करने के लिए यह कमांड चलाएँ:
```bash
dotnet run --project Catalog.Api
```
सर्वर डिफ़ॉल्ट रूप से `http://localhost:5233` और `https://localhost:7053` पोर्ट्स पर सुनना शुरू कर देगा।

### 3. Open Swagger UI
अपने ब्राउज़र में नीचे दिए गए URL को खोलें:
* **[Swagger Documentation UI Page](http://localhost:5233/swagger)**

यहाँ से आप डायरेक्टली सभी एंडपॉइंट्स (GET, POST, PUT, DELETE) को टेस्ट कर सकते हैं और स्कीमा देख सकते हैं।

### 4. Verify Options Pattern (DefaultPageSize)
* डिफ़ॉल्ट रूप से इन-मेमोरी रिपोजिटरी में 3 प्रोडक्ट्स सीड हैं।
* जब आप `GET /api/products` पर रिक्वेस्ट भेजेंगे, तो यह सिर्फ **2** प्रोडक्ट्स रिटर्न करेगा, क्योंकि `appsettings.json` में `DefaultPageSize` की वैल्यू `2` सेट है।
* यह सत्यापित (verify) करता है कि Options Pattern सही तरीके से कॉन्फ़िगर और इंजेक्ट हुआ है।

### 5. Run Unit Tests (यूनिट टेस्ट चलाएं)
xUnit प्रोजेक्ट `Catalog.Api.Tests` में लिखे गए टेस्ट को रन करने के लिए:
```bash
dotnet test Catalog.Api.Tests
```
यह टेस्ट करता है कि जब कोई प्रोडक्ट ID मौजूद नहीं होती है, तो API सही तरीके से **404 Not Found** रिटर्न करता है।

