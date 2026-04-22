HotelLakeview

HotelLakeview on hotellin varausjärjestelmän backend API, joka on toteutettu .NET Web API:na käyttäen modernia arkkitehtuuria ja hyväksi todettuja suunnittelumalleja.

Projektin tavoitteena on tarjota skaalautuva ja laajennettava backend, joka tukee:

- asiakashallintaa  
- huonehallintaa  
- varauksia ja saatavuushakua  
- hinnoittelulogiikkaa (sesongit)  
- myöhemmin autentikointia ja rooleja  

---

## Teknologiat

Projektissa käytetyt keskeiset teknologiat:

- .NET 10 Web API  
- Entity Framework Core  
- PostgreSQL (Npgsql)  
- Clean Architecture  
- CQRS  
- MediatR  
- FluentValidation  
- Result Pattern  
- GitHub Actions  

---

## Arkkitehtuuri

Projektissa käytetään Clean Architecturea, jossa vastuut on jaettu selkeästi eri kerroksiin.

### Domain

Sisältää liiketoiminnan ydinkäsitteet ja säännöt:

- Customer  
- Room  
- Reservation  
- RoomImage  
- User  

Enumit:

- RoomType  
- ReservationStatus  
- UserRole  

---

### Application

Sisältää sovelluksen käyttötapaukset:

- commandit ja queryt  
- handlerit  
- DTO:t  
- repository-rajapinnat  
- validointi  
- Result Pattern  
- pagination  

---

### Infrastructure

Sisältää tekniset toteutukset:

- EF Core DbContext  
- PostgreSQL  
- repositoryt  
- seed data  

---

### API

API-kerros vastaanottaa HTTP-pyynnöt:

- vastaanottaa pyynnön  
- luo command/query  
- lähettää MediatR:lle  
- palauttaa vastauksen  

---

## Pyyntöjen kulku

1. HTTP-pyyntö saapuu controllerille  
2. Controller luo commandin tai queryn  
3. Pyyntö lähetetään MediatR:lle  
4. Handler käsittelee pyynnön  
5. Repository hakee datan  
6. Tulos palautetaan  

---

## Tämänhetkinen tila

Projektissa on toteutettu:

- PostgreSQL EF Coren kautta  
- repositoryt EF:llä  
- varauslogiikka (overlap estetty)  
- hinnoittelu (sesongit)  
- validointi  
- testit  
- health check `/health`  
- CI/CD GitHub Actionsilla  

---

## Käynnistys lokaalisti

### 1. Vaatimukset

Varmista, että koneellasi on asennettuna:

- .NET SDK (versio 10)
- PostgreSQL

---

### 2. Kloonaa projekti



### 3. Asenna riippuvuudet
dotnet restore

Tämä lataa kaikki tarvittavat NuGet-paketit automaattisesti.

### 4. Aseta tietokantayhteys

Muokkaa tiedostoa:

src/HotelLakeview.Api/appsettings.json

Lisää oma PostgreSQL connection string:

"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=HotelLakeviewDb;Username=postgres;Password=YOUR_PASSWORD"
}
### 5. Luo tietokanta ja taulut
dotnet ef database update --project src/HotelLakeview.Infrastructure --startup-project src/HotelLakeview.Api

Tämä:
luo tietokannan
ajaa migrationit
luo taulut
### 6. Käynnistä sovellus
dotnet run --project src/HotelLakeview.Api
### 7. API käytössä

API löytyy:

http://localhost:5268

Health check:

http://localhost:5268/health

Swagger (jos käytössä):

http://localhost:5268/swagger

salasana = salasana12345!